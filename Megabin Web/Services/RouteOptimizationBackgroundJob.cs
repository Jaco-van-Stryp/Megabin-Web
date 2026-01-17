using Megabin_Web.Data;
using Megabin_Web.DTOs.Routing;
using Megabin_Web.Entities;
using Megabin_Web.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Megabin_Web.Services
{
    /// <summary>
    /// Background job for daily route optimization.
    /// Runs once per day to optimize collection routes for all drivers.
    /// </summary>
    public class RouteOptimizationBackgroundJob
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RouteOptimizationBackgroundJob> _logger;

        public RouteOptimizationBackgroundJob(
            IServiceProvider serviceProvider,
            ILogger<RouteOptimizationBackgroundJob> logger
        )
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// Optimizes routes for today's scheduled collections.
        /// This method is called by Hangfire as a recurring job.
        /// </summary>
        public async Task OptimizeRoutesAsync()
        {
            _logger.LogInformation("Starting daily route optimization job at {Time}", DateTime.Now);

            try
            {
                // Create a new scope for dependency injection
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var routeOptimizer = scope.ServiceProvider.GetRequiredService<IRouteOptimizationService>();

                var today = DateTime.Today;
                var dayOfWeek = today.DayOfWeek.ToString();

                // Get all active schedule contracts that need collection today
                // Based on their frequency and day of week
                var activeContracts = await dbContext.ScheduledContract
                    .Include(sc => sc.Addresses)
                    .Where(sc => sc.Active &&
                           sc.ApprovedExternally &&
                           sc.DayOfWeek == dayOfWeek)
                    .ToListAsync();

                if (activeContracts.Count == 0)
                {
                    _logger.LogInformation("No active schedule contracts found for {DayOfWeek}", dayOfWeek);
                    return;
                }

                _logger.LogInformation("Found {Count} active schedule contracts for {DayOfWeek}",
                    activeContracts.Count, dayOfWeek);

                // Get all active drivers with their home addresses
                var drivers = await dbContext.Drivers
                    .Include(d => d.HomeAddress)
                    .Where(d => d.Active)
                    .ToListAsync();

                if (drivers.Count == 0)
                {
                    _logger.LogWarning("No active drivers found. Cannot optimize routes.");
                    return;
                }

                _logger.LogInformation("Found {Count} active drivers", drivers.Count);

                // TODO: Get depot locations from configuration or database
                // For now, using a placeholder depot in Johannesburg
                var depots = new List<DepotLocation>
                {
                    new DepotLocation(
                        "depot-1",
                        new Location(28.0473, -26.2041), // Johannesburg placeholder
                        "Main Depot, Johannesburg"
                    )
                };

                // Build collection jobs from schedule contracts
                var jobs = activeContracts.Select(sc => new CollectionJob(
                    sc.Id.ToString(),
                    new Location(
                        sc.Addresses.Long,
                        sc.Addresses.Lat
                    ),
                    sc.Addresses.Address
                )).ToList();

                // Build driver vehicles
                var vehicles = drivers.Select(d => new DriverVehicle(
                    d.Id.ToString(),
                    new Location(
                        d.HomeAddress.Long,
                        d.HomeAddress.Lat
                    ),
                    d.VehicleCapacity
                )).ToList();

                // Optimize routes
                _logger.LogInformation("Calling route optimization for {JobCount} jobs and {DriverCount} drivers",
                    jobs.Count, drivers.Count);

                var result = await routeOptimizer.OptimizeMultiVehicleRoutesAsync(jobs, vehicles, depots);

                _logger.LogInformation(
                    "Route optimization complete: {RouteCount} routes created, {UnassignedCount} unassigned jobs, Total distance: {Distance}m",
                    result.Routes.Count, result.UnassignedJobs.Count, result.TotalDistanceMeters);

                // Save optimized routes to database as ScheduledCollections
                foreach (var route in result.Routes)
                {
                    var driverId = Guid.Parse(route.DriverId);
                    var driver = drivers.First(d => d.Id == driverId);

                    // Create scheduled collections for each stop in the route
                    foreach (var stop in route.Stops.Where(s => s.Type == StopType.Collection))
                    {
                        var scheduleContractId = Guid.Parse(stop.JobId!);

                        var scheduledCollection = new ScheduledCollections
                        {
                            Id = Guid.NewGuid(),
                            ScheduledFor = today,
                            UserId = driverId, // This is actually the driver ID
                            User = driver.User,
                            Collected = false,
                            Notes = $"Route order: {route.Stops.IndexOf(stop) + 1}/{route.Stops.Count}"
                        };

                        dbContext.ScheduledCollections.Add(scheduledCollection);
                    }
                }

                await dbContext.SaveChangesAsync();

                _logger.LogInformation("Daily route optimization job completed successfully at {Time}. Created {Count} scheduled collections.",
                    DateTime.Now, result.Routes.Sum(r => r.Stops.Count(s => s.Type == StopType.Collection)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during daily route optimization job");
                throw; // Re-throw to mark job as failed in Hangfire
            }
        }
    }
}
