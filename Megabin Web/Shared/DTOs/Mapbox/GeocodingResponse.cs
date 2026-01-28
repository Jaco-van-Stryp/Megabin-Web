using System.Text.Json.Serialization;

namespace Megabin_Web.Shared.DTOs.Mapbox
{
    /// <summary>
    /// Response from Mapbox Geocoding API.
    /// Internal DTO for deserializing Mapbox's API response.
    /// Documentation: https://docs.mapbox.com/api/search/geocoding/
    /// </summary>
    public class GeocodingResponse
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("query")]
        public List<string> Query { get; set; } = new();

        [JsonPropertyName("features")]
        public List<Feature> Features { get; set; } = new();

        [JsonPropertyName("attribution")]
        public string Attribution { get; set; } = string.Empty;
    }

    /// <summary>
    /// Individual geocoding result feature.
    /// </summary>
    public class Feature
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("place_type")]
        public List<string> PlaceType { get; set; } = new();

        [JsonPropertyName("relevance")]
        public double Relevance { get; set; }

        [JsonPropertyName("properties")]
        public Properties Properties { get; set; } = new();

        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;

        [JsonPropertyName("place_name")]
        public string PlaceName { get; set; } = string.Empty;

        [JsonPropertyName("center")]
        public List<double> Center { get; set; } = new();

        [JsonPropertyName("geometry")]
        public Geometry Geometry { get; set; } = new();

        [JsonPropertyName("context")]
        public List<Context>? Context { get; set; }
    }

    /// <summary>
    /// Properties of a feature (can contain additional metadata).
    /// </summary>
    public class Properties
    {
        [JsonPropertyName("accuracy")]
        public string? Accuracy { get; set; }
    }

    /// <summary>
    /// Geometry information for a feature.
    /// </summary>
    public class Geometry
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("coordinates")]
        public List<double> Coordinates { get; set; } = new();
    }

    /// <summary>
    /// Context provides hierarchical address components (region, country, etc.).
    /// </summary>
    public class Context
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;

        [JsonPropertyName("short_code")]
        public string? ShortCode { get; set; }
    }
}
