namespace Megabin_Web.DTOs.Routing
{
    /// <summary>
    /// Represents a driver and their vehicle with capacity constraints for route optimization.
    /// </summary>
    /// <param name="Id">Unique identifier for the driver. Typically the User.Id of the driver.</param>
    /// <param name="StartLocation">The geographic location where the driver begins their route (e.g., home or depot).</param>
    /// <param name="Capacity">Maximum number of collection stops before requiring a depot return for dumping. Must be greater than 0.</param>
    public record DriverVehicle(
        string Id,
        Location StartLocation,
        int Capacity);
}
