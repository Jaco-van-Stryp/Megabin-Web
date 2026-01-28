namespace Megabin_Web.Shared.DTOs.Routing
{
    /// <summary>
    /// Represents a single rubbish collection job for a specific address.
    /// </summary>
    /// <param name="Id">Unique identifier for this collection job. Links to ScheduledCollection.Id or Address.Id.</param>
    /// <param name="Location">Geographic coordinates of the collection address.</param>
    /// <param name="Address">Human-readable address string for driver display and navigation.</param>
    public record CollectionJob(string Id, Location Location, string Address);
}
