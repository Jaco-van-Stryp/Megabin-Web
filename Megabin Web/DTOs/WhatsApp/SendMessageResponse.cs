using System.Text.Json.Serialization;

namespace Megabin_Web.DTOs.WhatsApp
{
    /// <summary>
    /// Response model from Facebook Graph API when sending WhatsApp messages.
    /// </summary>
    internal class SendMessageResponse
    {
        /// <summary>
        /// The messaging product used (always "whatsapp")
        /// </summary>
        [JsonPropertyName("messaging_product")]
        public string? MessagingProduct { get; set; }

        /// <summary>
        /// Array of contact information
        /// </summary>
        [JsonPropertyName("contacts")]
        public List<WhatsAppContact>? Contacts { get; set; }

        /// <summary>
        /// Array of message information
        /// </summary>
        [JsonPropertyName("messages")]
        public List<WhatsAppMessage>? Messages { get; set; }
    }

    /// <summary>
    /// Contact information in the response
    /// </summary>
    internal class WhatsAppContact
    {
        /// <summary>
        /// WhatsApp ID of the contact
        /// </summary>
        [JsonPropertyName("wa_id")]
        public string? WaId { get; set; }

        /// <summary>
        /// Input phone number or ID
        /// </summary>
        [JsonPropertyName("input")]
        public string? Input { get; set; }
    }

    /// <summary>
    /// Message information in the response
    /// </summary>
    internal class WhatsAppMessage
    {
        /// <summary>
        /// Unique message identifier
        /// </summary>
        [JsonPropertyName("id")]
        public string? Id { get; set; }
    }

    /// <summary>
    /// Error response from Facebook Graph API
    /// </summary>
    internal class WhatsAppErrorResponse
    {
        /// <summary>
        /// Error details
        /// </summary>
        [JsonPropertyName("error")]
        public WhatsAppError? Error { get; set; }
    }

    /// <summary>
    /// Error details
    /// </summary>
    internal class WhatsAppError
    {
        /// <summary>
        /// Error message
        /// </summary>
        [JsonPropertyName("message")]
        public string? Message { get; set; }

        /// <summary>
        /// Error type
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        /// <summary>
        /// Error code
        /// </summary>
        [JsonPropertyName("code")]
        public int Code { get; set; }

        /// <summary>
        /// Error subcode
        /// </summary>
        [JsonPropertyName("error_subcode")]
        public int? ErrorSubcode { get; set; }

        /// <summary>
        /// Facebook trace ID for debugging
        /// </summary>
        [JsonPropertyName("fbtrace_id")]
        public string? FbtraceId { get; set; }
    }
}
