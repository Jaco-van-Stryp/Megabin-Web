namespace Megabin_Web.Configuration
{
    /// <summary>
    /// Configuration options for Mapbox API integration.
    /// </summary>
    public class MapboxOptions
    {
        /// <summary>
        /// The base URL for Mapbox APIs.
        /// Default: https://api.mapbox.com
        /// </summary>
        public string BaseUrl { get; set; } = "https://api.mapbox.com";

        /// <summary>
        /// Mapbox access token (API key) for authentication.
        /// Required for all API requests. Should be stored in User Secrets for security.
        /// Get your token at: https://account.mapbox.com/access-tokens/
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// HTTP request timeout in seconds for Mapbox API calls.
        /// Default: 30 seconds.
        /// </summary>
        public int TimeoutSeconds { get; set; } = 30;
    }
}
