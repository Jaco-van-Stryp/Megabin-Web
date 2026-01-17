namespace Megabin_Web.Configuration
{
    /// <summary>
    /// Configuration options for the WhatsApp Business API integration via Facebook Graph API.
    /// </summary>
    public class WhatsAppOptions
    {
        /// <summary>
        /// The base URL for the Facebook Graph API.
        /// Default: https://graph.facebook.com
        /// </summary>
        public string BaseUrl { get; set; } = "https://graph.facebook.com";

        /// <summary>
        /// The API version to use (e.g., "v22.0").
        /// Default: v22.0
        /// </summary>
        public string ApiVersion { get; set; } = "v22.0";

        /// <summary>
        /// The WhatsApp Business Phone Number ID from Meta Business Manager.
        /// This is required for sending messages.
        /// </summary>
        public required string PhoneNumberId { get; set; }

        /// <summary>
        /// The access token for authenticating with the Facebook Graph API.
        /// Should be stored in User Secrets for security.
        /// </summary>
        public required string AccessToken { get; set; }

        /// <summary>
        /// HTTP request timeout in seconds for WhatsApp API calls.
        /// Default: 30 seconds.
        /// </summary>
        public int TimeoutSeconds { get; set; } = 30;

        /// <summary>
        /// Maximum number of retry attempts for failed API calls.
        /// Default: 3 retries.
        /// </summary>
        public int MaxRetries { get; set; } = 3;
    }
}
