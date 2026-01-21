using System.Text.Json.Serialization;

namespace Megabin_Web.Shared.DTOs.OpenRouteService
{
    /// <summary>
    /// Internal model for OpenRouteService VROOM optimization API response.
    /// Maps to ORS /optimization endpoint response format.
    /// </summary>
    internal class VroomResponse
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("summary")]
        public VroomSummary? Summary { get; set; }

        [JsonPropertyName("routes")]
        public List<VroomRoute> Routes { get; set; } = new();

        [JsonPropertyName("unassigned")]
        public List<VroomUnassigned> Unassigned { get; set; } = new();
    }

    internal class VroomSummary
    {
        [JsonPropertyName("cost")]
        public int Cost { get; set; }

        [JsonPropertyName("distance")]
        public double Distance { get; set; } // Total distance in meters

        [JsonPropertyName("duration")]
        public double Duration { get; set; } // Total duration in seconds
    }

    internal class VroomRoute
    {
        [JsonPropertyName("vehicle")]
        public string? Vehicle { get; set; }

        [JsonPropertyName("cost")]
        public int Cost { get; set; }

        [JsonPropertyName("distance")]
        public double Distance { get; set; }

        [JsonPropertyName("duration")]
        public double Duration { get; set; }

        [JsonPropertyName("steps")]
        public List<VroomStep> Steps { get; set; } = new();
    }

    internal class VroomStep
    {
        [JsonPropertyName("type")]
        public required string Type { get; set; } // "start", "job", "end"

        [JsonPropertyName("job")]
        public string? Job { get; set; } // Job ID for "job" type steps

        [JsonPropertyName("location")]
        public List<double>? Location { get; set; } // [lon, lat]

        [JsonPropertyName("distance")]
        public double Distance { get; set; } // Distance to this step

        [JsonPropertyName("duration")]
        public double Duration { get; set; } // Duration to this step
    }

    internal class VroomUnassigned
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }

        [JsonPropertyName("location")]
        public List<double>? Location { get; set; }
    }
}
