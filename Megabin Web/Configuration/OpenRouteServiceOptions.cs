namespace Megabin_Web.Configuration
{
    /// <summary>
    /// Configuration options for the OpenRouteService integration.
    /// </summary>
    public class OpenRouteServiceOptions
    {
        /// <summary>
        /// The base URL of the self-hosted OpenRouteService instance.
        /// Default: http://localhost:8082
        /// </summary>
        public string BaseUrl { get; set; } = "http://localhost:8082";

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
