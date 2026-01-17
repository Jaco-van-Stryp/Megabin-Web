using Megabin_Web.DTOs.Routing;

namespace Megabin_Web.Interfaces
{
    /// <summary>
    /// Service for searching and validating addresses.
    /// Provides geocoding and autocomplete functionality using Mapbox.
    /// </summary>
    public interface IMapboxService
    {
        /// <summary>
        /// Converts an address string into geographic coordinates.
        /// </summary>
        /// <param name="address">The complete address to geocode.</param>
        /// <param name="country">ISO 3166-1 alpha-2 country code for filtering results (e.g., "ZA" for South Africa).</param>
        /// <param name="cancellationToken">Cancellation token for the async operation.</param>
        /// <returns>Location coordinates if found, null if address cannot be geocoded.</returns>
        Task<Location?> GeocodeAsync(
            string address,
            string country = "ZA",
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Provides address suggestions as user types (autocomplete).
        /// Used for interactive address input to prevent typos and ensure valid addresses.
        /// </summary>
        /// <param name="partialAddress">Partial address text entered by user (minimum 2 characters).</param>
        /// <param name="country">ISO 3166-1 alpha-2 country code for filtering results (e.g., "ZA" for South Africa).</param>
        /// <param name="maxResults">Maximum number of suggestions to return (1-20).</param>
        /// <param name="cancellationToken">Cancellation token for the async operation.</param>
        /// <returns>List of address suggestions with coordinates. Empty list if no matches found.</returns>
        Task<List<AddressSuggestion>> AutocompleteAsync(
            string partialAddress,
            string country = "ZA",
            int maxResults = 10,
            CancellationToken cancellationToken = default
        );
    }
}
