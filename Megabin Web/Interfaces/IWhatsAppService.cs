namespace Megabin_Web.Interfaces
{
    /// <summary>
    /// Service for sending WhatsApp messages via Facebook Graph API.
    /// </summary>
    public interface IWhatsAppService
    {
        /// <summary>
        /// Sends a text message to a user via WhatsApp.
        /// </summary>
        /// <param name="userId">The user ID to send the message to.</param>
        /// <param name="message">The text message content.</param>
        /// <param name="previewUrl">Whether to show URL previews in the message.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The WhatsApp message ID if successful, null otherwise.</returns>
        Task<string?> SendTextMessageAsync(
            Guid userId,
            string message,
            bool previewUrl = false,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Sends a template message to a user via WhatsApp.
        /// </summary>
        /// <param name="userId">The user ID to send the message to.</param>
        /// <param name="templateName">The name of the template to send.</param>
        /// <param name="languageCode">The language code (e.g., "en_US").</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The WhatsApp message ID if successful, null otherwise.</returns>
        Task<string?> SendTemplateMessageAsync(
            Guid userId,
            string templateName,
            string languageCode = "en_US",
            CancellationToken cancellationToken = default
        );
    }
}
