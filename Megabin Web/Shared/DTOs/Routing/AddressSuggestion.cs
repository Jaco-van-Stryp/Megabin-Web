namespace Megabin_Web.Shared.DTOs.Routing
{
    /// <summary>
    /// Represents a single address suggestion from autocomplete search.
    /// Used for providing address options to users as they type.
    /// </summary>
    /// <param name="Label">Full human-readable address label for display (e.g., "123 Main Street, Johannesburg, Gauteng, South Africa").</param>
    /// <param name="Location">Geographic coordinates of the address.</param>
    /// <param name="Region">Administrative region/province (e.g., "Gauteng"). Null if not available.</param>
    /// <param name="Locality">City or locality name (e.g., "Johannesburg"). Null if not available.</param>
    /// <param name="Country">Country name (e.g., "South Africa"). Null if not available.</param>
    public record AddressSuggestion(
        string Label,
        Location Location,
        string? Region,
        string? Locality,
        string? Country
    );
}
