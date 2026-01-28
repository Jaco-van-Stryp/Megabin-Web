using System.Text.Json;
using Megabin_Web.Shared.DTOs.Mapbox;
using Megabin_Web.Shared.DTOs.Routing;
using Microsoft.Extensions.Options;

namespace Megabin_Web.Shared.Infrastructure.MapBoxService
{
    /// <summary>
    /// Implementation of address search services using Mapbox Geocoding API.
    /// Provides address autocomplete and geocoding with 100k free requests/month.
    /// </summary>
    public class MapboxService : IMapboxService
    {
        private readonly HttpClient _httpClient;
        private readonly MapboxOptions _options;
        private readonly ILogger<MapboxService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        /// <summary>
        /// Initializes a new instance of the AddressSearchService.
        /// </summary>
        /// <param name="httpClient">HTTP client for making requests to Mapbox API.</param>
        /// <param name="options">Configuration options for Mapbox access token and base URL.</param>
        /// <param name="logger">Logger for debugging and monitoring operations.</param>
        public MapboxService(
            HttpClient httpClient,
            IOptions<MapboxOptions> options,
            ILogger<MapboxService> logger
        )
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _httpClient.BaseAddress = new Uri(_options.BaseUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
            };
        }

        /// <inheritdoc/>
        public async Task<Location?> GeocodeAsync(
            string address,
            string country = "ZA",
            CancellationToken cancellationToken = default
        )
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                _logger.LogWarning("GeocodeAsync called with empty address");
                return null;
            }

            try
            {
                _logger.LogInformation(
                    "Geocoding address: {Address} in country: {Country}",
                    address,
                    country
                );

                var requestUrl =
                    $"/geocoding/v5/mapbox.places/{Uri.EscapeDataString(address)}.json?country={country}&access_token={_options.AccessToken}&limit=1";

                _logger.LogDebug(
                    "Requesting Mapbox Geocoding URL: {BaseUrl}{RequestUrl}",
                    _httpClient.BaseAddress,
                    requestUrl.Replace(_options.AccessToken, "***")
                );

                var response = await _httpClient.GetAsync(requestUrl, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogError(
                        "Geocoding API returned status {StatusCode} for address: {Address}. Response: {Response}",
                        response.StatusCode,
                        address,
                        errorContent
                    );
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                var geocodingResponse = JsonSerializer.Deserialize<GeocodingResponse>(
                    content,
                    _jsonOptions
                );

                if (
                    geocodingResponse == null
                    || geocodingResponse.Features == null
                    || geocodingResponse.Features.Count == 0
                )
                {
                    _logger.LogWarning(
                        "No geocoding results found for address: {Address}",
                        address
                    );
                    return null;
                }

                var feature = geocodingResponse.Features[0];

                // Mapbox returns coordinates as [longitude, latitude]
                if (feature.Center.Count < 2)
                {
                    _logger.LogWarning(
                        "Invalid geometry in geocoding result for address: {Address}",
                        address
                    );
                    return null;
                }

                var location = new Location(
                    feature.Center[0], // Longitude
                    feature.Center[1] // Latitude
                );

                _logger.LogInformation(
                    "Successfully geocoded address: {Address} to {Longitude}, {Latitude}",
                    address,
                    location.Longitude,
                    location.Latitude
                );

                return location;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error geocoding address: {Address}", address);
                throw new InvalidOperationException($"Failed to geocode address: {address}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<List<AddressSuggestion>> AutocompleteAsync(
            string partialAddress,
            string country = "ZA",
            int maxResults = 10,
            CancellationToken cancellationToken = default
        )
        {
            if (string.IsNullOrWhiteSpace(partialAddress))
            {
                _logger.LogWarning("AutocompleteAsync called with empty partial address");
                return new List<AddressSuggestion>();
            }

            if (partialAddress.Length < 2)
            {
                _logger.LogDebug(
                    "AutocompleteAsync called with short input: {Length} characters",
                    partialAddress.Length
                );
                return new List<AddressSuggestion>();
            }

            // Clamp maxResults to valid range (Mapbox allows up to 10)
            maxResults = Math.Clamp(maxResults, 1, 10);

            try
            {
                _logger.LogInformation(
                    "Autocomplete address search: {PartialAddress} in country: {Country}, max results: {MaxResults}",
                    partialAddress,
                    country,
                    maxResults
                );

                // Mapbox Geocoding API with autocomplete parameter
                var requestUrl =
                    $"/geocoding/v5/mapbox.places/{Uri.EscapeDataString(partialAddress)}.json?country={country}&autocomplete=true&limit={maxResults}&access_token={_options.AccessToken}";

                _logger.LogDebug(
                    "Requesting Mapbox Autocomplete URL: {BaseUrl}{RequestUrl}",
                    _httpClient.BaseAddress,
                    requestUrl.Replace(_options.AccessToken, "***")
                );

                var response = await _httpClient.GetAsync(requestUrl, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogError(
                        "Autocomplete API returned status {StatusCode} for input: {PartialAddress}. Response: {Response}",
                        response.StatusCode,
                        partialAddress,
                        errorContent
                    );
                    return new List<AddressSuggestion>();
                }

                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                _logger.LogDebug("Raw Mapbox API response: {ResponseContent}", content);

                var geocodingResponse = JsonSerializer.Deserialize<GeocodingResponse>(
                    content,
                    _jsonOptions
                );

                if (
                    geocodingResponse == null
                    || geocodingResponse.Features == null
                    || geocodingResponse.Features.Count == 0
                )
                {
                    _logger.LogWarning(
                        "No autocomplete results found for input: {PartialAddress}. Response: {Response}",
                        partialAddress,
                        content
                    );
                    return new List<AddressSuggestion>();
                }

                var suggestions = new List<AddressSuggestion>();

                foreach (var feature in geocodingResponse.Features)
                {
                    if (feature.Center == null || feature.Center.Count < 2)
                    {
                        _logger.LogDebug("Skipping feature with invalid geometry");
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(feature.PlaceName))
                    {
                        _logger.LogDebug("Skipping feature with no place name");
                        continue;
                    }

                    // Extract region, locality, and country from context
                    string? region = null;
                    string? locality = null;
                    string? countryName = null;

                    if (feature.Context != null)
                    {
                        foreach (var context in feature.Context)
                        {
                            if (context.Id.StartsWith("region"))
                            {
                                region = context.Text;
                            }
                            else if (context.Id.StartsWith("place"))
                            {
                                locality = context.Text;
                            }
                            else if (context.Id.StartsWith("country"))
                            {
                                countryName = context.Text;
                            }
                        }
                    }

                    var suggestion = new AddressSuggestion(
                        feature.PlaceName,
                        new Location(
                            feature.Center[0], // Longitude
                            feature.Center[1] // Latitude
                        ),
                        region,
                        locality,
                        countryName
                    );

                    suggestions.Add(suggestion);
                }

                _logger.LogInformation(
                    "Autocomplete returned {Count} suggestions for: {PartialAddress}",
                    suggestions.Count,
                    partialAddress
                );

                return suggestions;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error during autocomplete for input: {PartialAddress}",
                    partialAddress
                );
                throw new InvalidOperationException(
                    $"Failed to autocomplete address: {partialAddress}",
                    ex
                );
            }
        }
    }
}
