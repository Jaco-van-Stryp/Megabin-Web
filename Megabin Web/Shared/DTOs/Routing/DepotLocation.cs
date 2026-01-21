namespace Megabin_Web.Shared.DTOs.Routing
{
    /// <summary>
    /// Represents a depot location where drivers can dump collected rubbish.
    /// </summary>
    /// <param name="Id">Unique identifier for the depot.</param>
    /// <param name="Location">Geographic coordinates of the depot.</param>
    /// <param name="Address">Human-readable address of the depot for driver reference.</param>
    public record DepotLocation(string Id, Location Location, string Address);
}
