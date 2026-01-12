using Megabin_Web.DTOs.Routing;

namespace Megabin_Web.Interfaces
{
    /// <summary>
    /// Service for integrating with OpenRouteService for geocoding and route optimization.
    /// Supports address verification and daily collection route planning with capacity-aware vehicle routing.
    /// </summary>
    public interface IOpenRouteService
    {
        /// <summary>
        /// Provides autocomplete address suggestions as the user types.
        /// Returns multiple address matches for user selection, similar to Google Maps autocomplete.
        /// </summary>
        /// <param name="partialAddress">Partial address text entered by the user (e.g., "123 Main").</param>
        /// <param name="country">ISO 3166-1 alpha-2 country code to focus search (default: "ZA" for South Africa).</param>
        /// <param name="maxResults">Maximum number of suggestions to return (default: 10, max: 20).</param>
        /// <param name="cancellationToken">Cancellation token for the async operation.</param>
        /// <returns>
        /// A list of <see cref="AddressSuggestion"/> objects with address labels and coordinates.
        /// Returns empty list if no matches found or if input is too short (less than 3 characters recommended).
        /// </returns>
        /// <remarks>
        /// This method is optimized for interactive use cases where users need to select from valid addresses.
        /// Prevents typos and ensures geocoded addresses are accurate.
        /// Consider debouncing input on the frontend to avoid excessive API calls.
        /// </remarks>
        Task<List<AddressSuggestion>> AutocompleteAddressAsync(
            string partialAddress,
            string country = "ZA",
            int maxResults = 10,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Geocodes an address string to geographic coordinates using OpenRouteService geocoding API.
        /// Focused on South African addresses for address verification during user registration.
        /// </summary>
        /// <param name="address">The address string to geocode (e.g., "123 Main Road, Johannesburg, South Africa").</param>
        /// <param name="country">ISO 3166-1 alpha-2 country code to focus search (default: "ZA" for South Africa).</param>
        /// <param name="cancellationToken">Cancellation token for the async operation.</param>
        /// <returns>
        /// A <see cref="Location"/> object with coordinates if a match is found; otherwise, null.
        /// Returns the best match based on OpenRouteService's relevance scoring.
        /// </returns>
        Task<Location?> GeocodeAddressAsync(
            string address,
            string country = "ZA",
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Optimizes daily collection job assignments across all available drivers using OpenRouteService VROOM API.
        /// Handles capacity constraints, depot returns for dumping, and produces ordered stop sequences per driver.
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
        /// The optimization assigns each job to exactly one driver, prioritizing proximity to driver start locations.
        /// Routes are balanced to minimize total distance while respecting vehicle capacity constraints.
        /// When a driver reaches capacity, a depot stop is automatically inserted before continuing to remaining jobs.
        /// </remarks>
        Task<DailyOptimizationResult> OptimizeDailyCollectionsAsync(
            IReadOnlyList<CollectionJob> jobs,
            IReadOnlyList<DriverVehicle> vehicles,
            IReadOnlyList<DepotLocation> depots,
            CancellationToken cancellationToken = default);
    }
}
