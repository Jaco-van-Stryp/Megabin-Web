using System.Text.Json.Serialization;

namespace Megabin_Web.DTOs.OpenRouteService
{
    /// <summary>
    /// Internal model for OpenRouteService autocomplete API responses.
    /// Maps to ORS /geocode/autocomplete endpoint response format.
    /// </summary>
    internal class AutocompleteResponse
    {
        [JsonPropertyName("features")]
        public List<AutocompleteFeature> Features { get; set; } = new();
    }

    internal class AutocompleteFeature
    {
        [JsonPropertyName("geometry")]
        public AutocompleteGeometry? Geometry { get; set; }

        [JsonPropertyName("properties")]
        public AutocompleteProperties? Properties { get; set; }
    }

    internal class AutocompleteGeometry
    {
        [JsonPropertyName("coordinates")]
        public List<double> Coordinates { get; set; } = new();
    }

    internal class AutocompleteProperties
    {
        [JsonPropertyName("label")]
        public string? Label { get; set; }

        [JsonPropertyName("region")]
        public string? Region { get; set; }

        [JsonPropertyName("locality")]
        public string? Locality { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}
