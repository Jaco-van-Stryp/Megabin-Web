namespace Megabin_Web.Shared.Infrastructure.WhatsAppService
{
    /// <summary>
    /// Pure integration service for sending WhatsApp messages via Facebook Graph API.
    /// Contains no database access or business logic.
    /// </summary>
    public interface IWhatsAppService
    {
        /// <summary>
        /// Sends a text message to a phone number via WhatsApp.
        /// </summary>
        /// <param name="phoneNumber">The phone number to send the message to (in international format).</param>
        /// <param name="message">The text message content.</param>
        /// <param name="previewUrl">Whether to show URL previews in the message.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The WhatsApp message ID if successful, null otherwise.</returns>
        Task<string?> SendTextMessageAsync(
            string phoneNumber,
            string message,
            bool previewUrl = false,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Sends a template message to a phone number via WhatsApp.
        /// </summary>
        /// <param name="phoneNumber">The phone number to send the message to (in international format).</param>
        /// <param name="templateName">The name of the template to send.</param>
        /// <param name="languageCode">The language code (e.g., "en_US").</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The WhatsApp message ID if successful, null otherwise.</returns>
        Task<string?> SendTemplateMessageAsync(
            string phoneNumber,
            string templateName,
            string languageCode = "en_US",
            CancellationToken cancellationToken = default
        );
    }
}
