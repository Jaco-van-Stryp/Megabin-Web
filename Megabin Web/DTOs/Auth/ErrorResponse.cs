namespace Megabin_Web.DTOs.Auth
{
    /// <summary>
    /// Standardized error response for API operations.
    /// </summary>
    /// <param name="Message">The human-readable error message.</param>
    /// <param name="StatusCode">The HTTP status code.</param>
    public record ErrorResponse(string Message, int StatusCode);
}
