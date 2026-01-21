namespace Megabin_Web.Shared.DTOs.Routing
{
    /// <summary>
    /// Represents the complete result of daily collection route optimization across all drivers.
    /// </summary>
    /// <param name="Routes">Optimized routes for each driver, including ordered stops and metrics.</param>
    /// <param name="UnassignedJobs">Collection jobs that could not be assigned to any driver due to capacity or routing constraints.</param>
    /// <param name="TotalDistanceMeters">Aggregate distance across all driver routes in meters.</param>
    /// <param name="TotalDurationSeconds">Aggregate estimated duration across all driver routes in seconds.</param>
    public record DailyOptimizationResult(
        List<DriverRoute> Routes,
        List<CollectionJob> UnassignedJobs,
        double TotalDistanceMeters,
        double TotalDurationSeconds
    );
}
