namespace Megabin_Web.DTOs.Routing
{
    /// <summary>
    /// Represents a single stop in a driver's optimized route.
    /// </summary>
    /// <param name="Type">The type of stop (Collection or Depot).</param>
    /// <param name="Location">Geographic coordinates of the stop.</param>
    /// <param name="Address">Human-readable address for driver navigation and display.</param>
    /// <param name="JobId">Reference to the collection job ID. Null for depot stops.</param>
    public record RouteStop(
        StopType Type,
        Location Location,
        string Address,
        string? JobId);
}
