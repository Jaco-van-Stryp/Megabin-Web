using System.Text.Json.Serialization;

namespace Megabin_Web.Shared.DTOs.OpenRouteService
{
    /// <summary>
    /// Internal model for OpenRouteService VROOM optimization API request.
    /// Maps to ORS /optimization endpoint request format.
    /// </summary>
    internal class VroomRequest
    {
        [JsonPropertyName("jobs")]
        public List<VroomJob> Jobs { get; set; } = new();

        [JsonPropertyName("vehicles")]
        public List<VroomVehicle> Vehicles { get; set; } = new();

        [JsonPropertyName("options")]
        public VroomOptions? Options { get; set; }
    }

    internal class VroomJob
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }

        [JsonPropertyName("location")]
        public required List<double> Location { get; set; } // [lon, lat]

        [JsonPropertyName("amount")]
        public List<int> Amount { get; set; } = new() { 1 }; // Capacity consumption
    }

    internal class VroomVehicle
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }

        [JsonPropertyName("start")]
        public required List<double> Start { get; set; } // [lon, lat]

        [JsonPropertyName("end")]
        public List<double>? End { get; set; } // [lon, lat] - depot location

        [JsonPropertyName("capacity")]
        public required List<int> Capacity { get; set; } // [max stops]

        [JsonPropertyName("profile")]
        public string Profile { get; set; } = "driving-car";
    }

    internal class VroomOptions
    {
        [JsonPropertyName("g")]
        public bool IncludeGeometry { get; set; } = true; // Include route geometry for distance calculation
    }
}
