using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Megabin_Web.Configuration;
using Megabin_Web.Data;
using Megabin_Web.DTOs.WhatsApp;
using Megabin_Web.Enums;
using Megabin_Web.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Megabin_Web.Services
{
    /// <summary>
    /// Implementation of WhatsApp messaging service using Facebook Graph API.
    /// Sends text and template messages to users via WhatsApp Business API.
    /// </summary>
    public class WhatsAppService : IWhatsAppService
    {
        private readonly AppDbContext _dbContext;
        private readonly HttpClient _httpClient;
        private readonly WhatsAppOptions _options;
        private readonly ILogger<WhatsAppService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        /// <summary>
        /// Initializes a new instance of the WhatsAppService.
        /// </summary>
        /// <param name="dbContext">Database context for accessing user data.</param>
        /// <param name="httpClient">HTTP client for making requests to Facebook Graph API.</param>
        /// <param name="options">Configuration options for WhatsApp API credentials.</param>
        /// <param name="logger">Logger for debugging and monitoring operations.</param>
        public WhatsAppService(
            AppDbContext dbContext,
            HttpClient httpClient,
            IOptions<WhatsAppOptions> options,
            ILogger<WhatsAppService> logger
        )
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _httpClient.BaseAddress = new Uri($"{_options.BaseUrl}/{_options.ApiVersion}/");
            _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                _options.AccessToken
            );

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
            };
        }

        /// <inheritdoc/>
        public async Task<string?> SendTextMessageAsync(
            Guid userId,
            string message,
            bool previewUrl = false,
            CancellationToken cancellationToken = default
        )
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                _logger.LogWarning("SendTextMessageAsync called with empty message");
                return null;
            }

            try
            {
                // Get user's phone number from database
                var user = await _dbContext.Users.FirstOrDefaultAsync(
                    x => x.Id == userId,
                    cancellationToken
                );

                if (user == null)
                {
                    _logger.LogWarning("User not found with ID: {UserId}", userId);
                    return null;
                }

                if (string.IsNullOrWhiteSpace(user.PhoneNumber))
                {
                    _logger.LogWarning("User {UserId} has no phone number", userId);
                    return null;
                }

                _logger.LogInformation(
                    "Sending WhatsApp text message to user {UserId} ({PhoneNumber})",
                    userId,
                    user.PhoneNumber
                );

                // Create request payload
                var request = new SendMessageRequest
                {
                    To = user.PhoneNumber,
                    Type = WhatsAppMessageType.Text,
                    Text = new TextContent { Body = message, PreviewUrl = previewUrl },
                };

                return await SendMessageAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error sending WhatsApp text message to user {UserId}",
                    userId
                );
                return null; // Silently fail and return null on error
            }
        }

        /// <inheritdoc/>
        public async Task<string?> SendTemplateMessageAsync(
            Guid userId,
            string templateName,
            string languageCode = "en_US",
            CancellationToken cancellationToken = default
        )
        {
            if (string.IsNullOrWhiteSpace(templateName))
            {
                _logger.LogWarning("SendTemplateMessageAsync called with empty template name");
                return null;
            }

            try
            {
                // Get user's phone number from database
                var user = await _dbContext.Users.FirstOrDefaultAsync(
                    x => x.Id == userId,
                    cancellationToken
                );

                if (user == null)
                {
                    _logger.LogWarning("User not found with ID: {UserId}", userId);
                    return null;
                }

                if (string.IsNullOrWhiteSpace(user.PhoneNumber))
                {
                    _logger.LogWarning("User {UserId} has no phone number", userId);
                    return null;
                }

                _logger.LogInformation(
                    "Sending WhatsApp template message '{TemplateName}' to user {UserId} ({PhoneNumber})",
                    templateName,
                    userId,
                    user.PhoneNumber
                );

                // Create request payload
                var request = new SendMessageRequest
                {
                    To = user.PhoneNumber,
                    Type = WhatsAppMessageType.Template,
                    Template = new TemplateContent
                    {
                        Name = templateName,
                        Language = new TemplateLanguage { Code = languageCode },
                    },
                };

                return await SendMessageAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error sending WhatsApp template message to user {UserId}",
                    userId
                );
                throw new InvalidOperationException(
                    $"Failed to send WhatsApp template message to user {userId}",
                    ex
                );
            }
        }

        /// <summary>
        /// Internal method to send a message request to Facebook Graph API.
        /// </summary>
        private async Task<string?> SendMessageAsync(
            SendMessageRequest request,
            CancellationToken cancellationToken
        )
        {
            try
            {
                var requestUrl = $"{_options.PhoneNumberId}/messages";

                var jsonContent = JsonSerializer.Serialize(request, _jsonOptions);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                _logger.LogDebug(
                    "Sending WhatsApp API request to: {BaseUrl}{RequestUrl}. Payload: {Payload}",
                    _httpClient.BaseAddress,
                    requestUrl,
                    jsonContent
                );

                var response = await _httpClient.PostAsync(
                    requestUrl,
                    httpContent,
                    cancellationToken
                );

                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = JsonSerializer.Deserialize<WhatsAppErrorResponse>(
                        responseContent,
                        _jsonOptions
                    );

                    _logger.LogError(
                        "WhatsApp API returned status {StatusCode}. Error: {ErrorMessage} (Code: {ErrorCode}, Type: {ErrorType}, TraceId: {TraceId})",
                        response.StatusCode,
                        errorResponse?.Error?.Message ?? "Unknown error",
                        errorResponse?.Error?.Code ?? 0,
                        errorResponse?.Error?.Type ?? "Unknown",
                        errorResponse?.Error?.FbtraceId ?? "N/A"
                    );

                    return null;
                }

                var messageResponse = JsonSerializer.Deserialize<SendMessageResponse>(
                    responseContent,
                    _jsonOptions
                );

                var messageId = messageResponse?.Messages?.FirstOrDefault()?.Id;

                if (string.IsNullOrWhiteSpace(messageId))
                {
                    _logger.LogWarning(
                        "WhatsApp API response did not contain a message ID. Response: {Response}",
                        responseContent
                    );
                    return null;
                }

                _logger.LogInformation(
                    "Successfully sent WhatsApp message. Message ID: {MessageId}",
                    messageId
                );

                return messageId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message to WhatsApp API");
                throw;
            }
        }
    }
}
