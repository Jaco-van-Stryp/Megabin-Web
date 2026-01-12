namespace Megabin_Web.DTOs.Routing
{
    /// <summary>
    /// Represents a single driver's optimized collection route for the day.
    /// </summary>
    /// <param name="DriverId">The unique identifier of the driver assigned to this route.</param>
    /// <param name="Stops">Ordered list of stops (collections and depot visits) for optimal routing.</param>
    /// <param name="TotalDistanceMeters">Total distance of the route in meters. Null if not available.</param>
    /// <param name="TotalDurationSeconds">Estimated total duration of the route in seconds. Null if not available.</param>
    public record DriverRoute(
        string DriverId,
        List<RouteStop> Stops,
        double? TotalDistanceMeters,
        double? TotalDurationSeconds);
}
