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

            // Convert System.DayOfWeek to custom DayOfWeek enum
            // System.DayOfWeek: Sunday=0, Monday=1, ..., Saturday=6
            // Custom enum: Monday=0, Tuesday=1, ..., Sunday=6
            var customDayOfWeek = ConvertToCustomDayOfWeek(targetDate.DayOfWeek);

            // Get all active schedule contracts that need collection on this day of week
            var activeContracts = await dbContext
                .ScheduledContract.Include(sc => sc.Addresses)
                .Where(sc =>
                    sc.Active
                    && sc.ApprovedExternally
                    && sc.DayOfWeek == customDayOfWeek
                )
                .ToListAsync(cancellationToken);

            if (activeContracts.Count == 0)
            {
                logger.LogInformation(
                    "No active schedule contracts found for {DayOfWeek}",
                    customDayOfWeek
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
                customDayOfWeek
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

            // Build a lookup dictionary for drivers
            var driverLookup = drivers.ToDictionary(d => d.Id);

            // Save optimized routes to database as ScheduledCollections
            foreach (var route in result.Routes)
            {
                if (!Guid.TryParse(route.DriverId, out var driverId))
                {
                    logger.LogWarning(
                        "Invalid DriverId format '{DriverId}' in optimization result, skipping route",
                        route.DriverId
                    );
                    continue;
                }

                if (!driverLookup.TryGetValue(driverId, out var driver))
                {
                    logger.LogWarning(
                        "Driver not found for DriverId '{DriverId}', skipping route",
                        route.DriverId
                    );
                    continue;
                }

                // Get collection stops only and track sequence
                var collectionStops = route.Stops.Where(s => s.Type == StopType.Collection).ToList();
                var routeSequence = 1;

                // Create scheduled collections for each stop in the route
                foreach (var stop in collectionStops)
                {
                    if (!Guid.TryParse(stop.JobId, out var scheduleContractId))
                    {
                        logger.LogWarning(
                            "Invalid JobId format '{JobId}' in optimization result, skipping stop",
                            stop.JobId
                        );
                        continue;
                    }

                    if (!contractLookup.TryGetValue(scheduleContractId, out var contract))
                    {
                        logger.LogWarning(
                            "Schedule contract not found for JobId '{JobId}' (ContractId: {ContractId}), skipping stop",
                            stop.JobId,
                            scheduleContractId
                        );
                        continue;
                    }

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

        /// <summary>
        /// Converts System.DayOfWeek to custom DayOfWeek enum.
        /// System.DayOfWeek: Sunday=0, Monday=1, ..., Saturday=6
        /// Custom enum: Monday=0, Tuesday=1, ..., Sunday=6
        /// </summary>
        private static Shared.Domain.Enums.DayOfWeek ConvertToCustomDayOfWeek(
            System.DayOfWeek systemDayOfWeek
        )
        {
            return systemDayOfWeek switch
            {
                System.DayOfWeek.Monday => Shared.Domain.Enums.DayOfWeek.Monday,
                System.DayOfWeek.Tuesday => Shared.Domain.Enums.DayOfWeek.Tuesday,
                System.DayOfWeek.Wednesday => Shared.Domain.Enums.DayOfWeek.Wednesday,
                System.DayOfWeek.Thursday => Shared.Domain.Enums.DayOfWeek.Thursday,
                System.DayOfWeek.Friday => Shared.Domain.Enums.DayOfWeek.Friday,
                System.DayOfWeek.Saturday => Shared.Domain.Enums.DayOfWeek.Saturday,
                System.DayOfWeek.Sunday => Shared.Domain.Enums.DayOfWeek.Sunday,
                _ => throw new ArgumentOutOfRangeException(nameof(systemDayOfWeek)),
            };
        }
    }
}
