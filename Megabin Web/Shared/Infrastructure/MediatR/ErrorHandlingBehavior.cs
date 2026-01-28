using MediatR;

namespace Megabin_Web.Shared.Infrastructure.MediatR
{
    /// <summary>
    /// Pipeline behavior that catches and logs exceptions from handlers.
    /// </summary>
    public class ErrorHandlingBehavior<TRequest, TResponse>(
        ILogger<ErrorHandlingBehavior<TRequest, TResponse>> logger
    ) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken
        )
        {
            try
            {
                return await next();
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogWarning(
                    ex,
                    "Resource not found while handling {RequestName}: {Message}",
                    typeof(TRequest).Name,
                    ex.Message
                );
                throw;
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning(
                    ex,
                    "Invalid operation while handling {RequestName}: {Message}",
                    typeof(TRequest).Name,
                    ex.Message
                );
                throw;
            }
            catch (UnauthorizedAccessException ex)
            {
                logger.LogWarning(
                    ex,
                    "Unauthorized access while handling {RequestName}: {Message}",
                    typeof(TRequest).Name,
                    ex.Message
                );
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Unhandled exception while handling {RequestName}",
                    typeof(TRequest).Name
                );
                throw;
            }
        }
    }
}
