namespace Megabin_Web.Configuration
{
    /// <summary>
    /// Configuration options for JWT token generation and validation.
    /// </summary>
    public class JwtOptions
    {
        /// <summary>
        /// Secret key for signing JWT tokens. MUST be at least 32 characters (256 bits).
        /// Should be stored in User Secrets for development and secure vaults in production.
        /// </summary>
        public required string Secret { get; set; }

        /// <summary>
        /// Token issuer identifier. Typically the application name or domain.
        /// Default: "MegabinWebAPI"
        /// </summary>
        public string Issuer { get; set; } = "MegabinWebAPI";

        /// <summary>
        /// Token audience identifier. Typically the client application or domain.
        /// Default: "MegabinWebClient"
        /// </summary>
        public string Audience { get; set; } = "MegabinWebClient";

        /// <summary>
        /// Token expiration time in minutes.
        /// Default: 480 minutes (8 hours)
        /// </summary>
        public int ExpirationMinutes { get; set; } = 480;
    }
}
