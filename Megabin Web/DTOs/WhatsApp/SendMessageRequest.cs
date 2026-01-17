using System.Text.Json.Serialization;
using Megabin_Web.Enums;

namespace Megabin_Web.DTOs.WhatsApp
{
    /// <summary>
    /// Request model for sending WhatsApp messages via Facebook Graph API.
    /// Supports both text messages and template messages.
    /// </summary>
    internal class SendMessageRequest
    {
        /// <summary>
        /// Must be "whatsapp"
        /// </summary>
        [JsonPropertyName("messaging_product")]
        public string MessagingProduct { get; set; } = "whatsapp";

        /// <summary>
        /// WhatsApp ID or phone number in international format (e.g., "+64277662005")
        /// </summary>
        [JsonPropertyName("to")]
        public required string To { get; set; }

        /// <summary>
        /// Type of message: "text" or "template"
        /// </summary>
        [JsonPropertyName("type")]
        public required WhatsAppMessageType Type { get; set; }

        /// <summary>
        /// Text message content (used when type is "text")
        /// </summary>
        [JsonPropertyName("text")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TextContent? Text { get; set; }

        /// <summary>
        /// Template message content (used when type is "template")
        /// </summary>
        [JsonPropertyName("template")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TemplateContent? Template { get; set; }
    }

    /// <summary>
    /// Text message content
    /// </summary>
    internal class TextContent
    {
        /// <summary>
        /// Whether to show a preview of URLs in the message
        /// </summary>
        [JsonPropertyName("preview_url")]
        public bool PreviewUrl { get; set; } = false;

        /// <summary>
        /// The text message body
        /// </summary>
        [JsonPropertyName("body")]
        public required string Body { get; set; }
    }

    /// <summary>
    /// Template message content
    /// </summary>
    internal class TemplateContent
    {
        /// <summary>
        /// The name of the template (e.g., "hello_world")
        /// </summary>
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        /// <summary>
        /// Language configuration for the template
        /// </summary>
        [JsonPropertyName("language")]
        public required TemplateLanguage Language { get; set; }
    }

    /// <summary>
    /// Template language configuration
    /// </summary>
    internal class TemplateLanguage
    {
        /// <summary>
        /// Language code (e.g., "en_US")
        /// </summary>
        [JsonPropertyName("code")]
        public required string Code { get; set; }
    }
}
