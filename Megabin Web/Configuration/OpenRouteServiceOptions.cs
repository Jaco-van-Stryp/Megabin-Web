namespace Megabin_Web.Configuration
{
    /// <summary>
    /// Configuration options for the OpenRouteService integration.
    /// </summary>
    public class OpenRouteServiceOptions
    {
        /// <summary>
        /// The base URL of the OpenRouteService instance.
        /// Default: https://api.openrouteservice.org (cloud API)
        /// </summary>
        public string BaseUrl { get; set; } = "https://api.openrouteservice.org";

        /// <summary>
        /// API key for OpenRouteService cloud API authentication.
        /// Required for cloud API access. Should be stored in User Secrets for security.
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// HTTP request timeout in seconds for OpenRouteService API calls.
        /// Default: 300 seconds (5 minutes) to accommodate large optimization requests.
        /// </summary>
        public int TimeoutSeconds { get; set; } = 300;

        /// <summary>
        /// Maximum number of retry attempts for failed API calls.
        /// Default: 3 retries.
        /// </summary>
        public int MaxRetries { get; set; } = 3;
    }
}
