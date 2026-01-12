using System.Text.Json.Serialization;

namespace Megabin_Web.DTOs.OpenRouteService
{
    /// <summary>
    /// Internal model for OpenRouteService geocoding API responses.
    /// Maps to ORS /geocode/search endpoint response format.
    /// </summary>
    internal class GeocodingResponse
    {
        [JsonPropertyName("features")]
        public List<GeocodingFeature> Features { get; set; } = new();
    }

    internal class GeocodingFeature
    {
        [JsonPropertyName("geometry")]
        public GeocodingGeometry? Geometry { get; set; }

        [JsonPropertyName("properties")]
        public GeocodingProperties? Properties { get; set; }
    }

    internal class GeocodingGeometry
    {
        [JsonPropertyName("coordinates")]
        public List<double> Coordinates { get; set; } = new();
    }

    internal class GeocodingProperties
    {
        [JsonPropertyName("label")]
        public string? Label { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }
    }
}
