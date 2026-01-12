namespace Megabin_Web.DTOs.Routing
{
    /// <summary>
    /// Represents the type of stop in a driver's route.
    /// </summary>
    public enum StopType
    {
        /// <summary>
        /// A collection stop where rubbish is picked up from a customer address.
        /// </summary>
        Collection,

        /// <summary>
        /// A depot stop where the driver dumps collected rubbish before continuing the route.
        /// </summary>
        Depot
    }
}
