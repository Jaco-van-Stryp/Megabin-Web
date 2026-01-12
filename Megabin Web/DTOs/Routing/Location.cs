namespace Megabin_Web.DTOs.Routing
{
    /// <summary>
    /// Represents a geographic location with longitude and latitude coordinates.
    /// </summary>
    /// <param name="Longitude">The longitude coordinate (X-axis). Valid range: -180 to 180.</param>
    /// <param name="Latitude">The latitude coordinate (Y-axis). Valid range: -90 to 90.</param>
    public record Location(double Longitude, double Latitude);
}
