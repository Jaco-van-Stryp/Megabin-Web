using Megabin_Web.DTOs.Routing;

namespace Megabin_Web.Interfaces
{
    /// <summary>
    /// Service for optimizing multi-vehicle routes using OpenRouteService VROOM engine.
    /// Handles capacity-constrained vehicle routing for daily waste collection scheduling.
    /// </summary>
    public interface IRouteOptimizationService
    {
        /// <summary>
        /// Optimizes collection routes across multiple drivers with vehicle capacity constraints.
        /// Assigns jobs to drivers, sequences stops efficiently, and inserts depot returns when vehicles reach capacity.
        /// </summary>
        /// <param name="jobs">Collection jobs to be assigned and routed (typically ~1000 daily jobs).</param>
        /// <param name="vehicles">Available drivers with their start locations and vehicle capacities (typically ~100 drivers).</param>
        /// <param name="depots">Depot locations where drivers can dump collected rubbish.</param>
        /// <param name="cancellationToken">Cancellation token for the async operation.</param>
        /// <returns>
        /// A <see cref="DailyOptimizationResult"/> containing optimized routes for each driver,
        /// any jobs that couldn't be assigned, and aggregate distance/duration metrics.
        /// </returns>
        /// <remarks>
        /// Uses VROOM (Vehicle Routing Open-source Optimization Machine) via OpenRouteService.
        /// The optimization assigns each job to exactly one driver, prioritizing proximity to driver start locations.
        /// Routes are balanced to minimize total distance while respecting vehicle capacity constraints.
        /// When a driver reaches capacity, a depot stop is automatically inserted before continuing to remaining jobs.
        /// </remarks>
        Task<DailyOptimizationResult> OptimizeMultiVehicleRoutesAsync(
            IReadOnlyList<CollectionJob> jobs,
            IReadOnlyList<DriverVehicle> vehicles,
            IReadOnlyList<DepotLocation> depots,
            CancellationToken cancellationToken = default);
    }
}
