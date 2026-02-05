using MediatR;
using Megabin_Web.Shared.Domain.Data;
using Megabin_Web.Shared.Domain.Entities;
using Megabin_Web.Shared.DTOs.Routing;
using Megabin_Web.Shared.Infrastructure.OpenRouteService;
using Microsoft.EntityFrameworkCore;

namespace Megabin_Web.Features.RouteOptimization.OptimizeDailyRoutes
{
    public class OptimizeDailyRoutesHandler(
        AppDbContext dbContext,
        IRouteOptimizationService routeOptimizer,
        ILogger<OptimizeDailyRoutesHandler> logger
    ) : IRequestHandler<OptimizeDailyRoutesCommand, DailyOptimizationResult>
    {
        public async Task<DailyOptimizationResult> Handle(
            OptimizeDailyRoutesCommand request,
            CancellationToken cancellationToken
        )
        {
            // Ensure we're working with UTC dates for PostgreSQL compatibility
            var targetDate = request.TargetDate.HasValue
                ? DateTime.SpecifyKind(request.TargetDate.Value.Date, DateTimeKind.Utc)
                : DateTime.UtcNow.Date;

            logger.LogInformation(
                "Starting route optimization for date {Date}",
                targetDate
            );

            // Clear existing schedules for the target date
            var existingSchedules = await dbContext
                .ScheduledCollections.Where(sc => sc.ScheduledFor.Date == targetDate.Date)
                .ToListAsync(cancellationToken);

            if (existingSchedules.Count > 0)
            {
                logger.LogInformation(
                    "Clearing {Count} existing schedules for {Date}",
                    existingSchedules.Count,
                    targetDate
                );
                dbContext.ScheduledCollections.RemoveRange(existingSchedules);
            }

            // Get day of week for the target date
            var dayOfWeekEnum = targetDate.DayOfWeek;

            // Get all active schedule contracts that need collection on this day of week
            var activeContracts = await dbContext
                .ScheduledContract.Include(sc => sc.Addresses)
                .Where(sc =>
                    sc.Active
                    && sc.ApprovedExternally
                    && (DayOfWeek)sc.DayOfWeek == dayOfWeekEnum
                )
                .ToListAsync(cancellationToken);

            if (activeContracts.Count == 0)
            {
                logger.LogInformation(
                    "No active schedule contracts found for {DayOfWeek}",
                    dayOfWeekEnum
                );
                return new DailyOptimizationResult(
                    new List<DriverRoute>(),
                    new List<CollectionJob>(),
                    0,
                    0
                );
            }

            logger.LogInformation(
                "Found {Count} active schedule contracts for {DayOfWeek}",
                activeContracts.Count,
                dayOfWeekEnum
            );

            // Get all active drivers with their user information
            var drivers = await dbContext
                .Drivers.Include(d => d.User)
                .Where(d => d.Active)
                .ToListAsync(cancellationToken);

            if (drivers.Count == 0)
            {
                logger.LogWarning("No active drivers found. Cannot optimize routes.");
                throw new InvalidOperationException(
                    "No active drivers available for route optimization"
                );
            }

            logger.LogInformation("Found {Count} active drivers", drivers.Count);

            // TODO: Get depot locations from configuration or database
            // For now, using a placeholder depot in Johannesburg
            var depots = new List<DepotLocation>
            {
                new DepotLocation(
                    "depot-1",
                    new Location(28.0473, -26.2041), // Johannesburg placeholder
                    "Main Depot, Johannesburg"
                ),
            };

            // Build collection jobs from schedule contracts
            var jobs = activeContracts
                .Select(sc => new CollectionJob(
                    sc.Id.ToString(),
                    new Location(sc.Addresses.Long, sc.Addresses.Lat),
                    sc.Addresses.Address
                ))
                .ToList();

            // Build driver vehicles
            var vehicles = drivers
                .Select(d => new DriverVehicle(
                    d.Id.ToString(),
                    new Location(d.HomeAddressLong, d.HomeAddressLat),
                    d.VehicleCapacity
                ))
                .ToList();

            // Optimize routes
            logger.LogInformation(
                "Calling route optimization for {JobCount} jobs and {DriverCount} drivers",
                jobs.Count,
                drivers.Count
            );

            var result = await routeOptimizer.OptimizeMultiVehicleRoutesAsync(
                jobs,
                vehicles,
                depots
            );

            logger.LogInformation(
                "Route optimization complete: {RouteCount} routes created, {UnassignedCount} unassigned jobs, Total distance: {Distance}m",
                result.Routes.Count,
                result.UnassignedJobs.Count,
                result.TotalDistanceMeters
            );

            // Build a lookup dictionary for schedule contracts to get address IDs
            var contractLookup = activeContracts.ToDictionary(c => c.Id);

            // Save optimized routes to database as ScheduledCollections
            foreach (var route in result.Routes)
            {
                var driverId = Guid.Parse(route.DriverId);
                var driver = drivers.First(d => d.Id == driverId);

                // Get collection stops only and track sequence
                var collectionStops = route.Stops.Where(s => s.Type == StopType.Collection).ToList();
                var routeSequence = 1;

                // Create scheduled collections for each stop in the route
                foreach (var stop in collectionStops)
                {
                    var scheduleContractId = Guid.Parse(stop.JobId!);
                    var contract = contractLookup[scheduleContractId];

                    var scheduledCollection = new ScheduledCollections
                    {
                        Id = Guid.NewGuid(),
                        ScheduledFor = targetDate,
                        UserId = driverId, // This is actually the driver ID
                        User = driver.User,
                        AddressId = contract.AddressesId,
                        Address = contract.Addresses,
                        RouteSequence = routeSequence,
                        Collected = false,
                        Notes = string.Empty,
                    };

                    dbContext.ScheduledCollections.Add(scheduledCollection);
                    routeSequence++;
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Route optimization completed successfully. Created {Count} scheduled collections.",
                result.Routes.Sum(r => r.Stops.Count(s => s.Type == StopType.Collection))
            );

            return result;
        }
    }
}
