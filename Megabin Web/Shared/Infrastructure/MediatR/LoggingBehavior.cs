using System.Diagnostics;
using MediatR;

namespace Megabin_Web.Shared.Infrastructure.MediatR
{
    /// <summary>
    /// Pipeline behavior that logs request execution time and details.
    /// </summary>
    public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken
        )
        {
            var requestName = typeof(TRequest).Name;
            var stopwatch = Stopwatch.StartNew();

            logger.LogInformation(
                "Handling {RequestName}",
                requestName
            );

            try
            {
                var response = await next();

                stopwatch.Stop();

                logger.LogInformation(
                    "Handled {RequestName} in {ElapsedMilliseconds}ms",
                    requestName,
                    stopwatch.ElapsedMilliseconds
                );

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                logger.LogError(
                    ex,
                    "Error handling {RequestName} after {ElapsedMilliseconds}ms",
                    requestName,
                    stopwatch.ElapsedMilliseconds
                );

                throw;
            }
        }
    }
}
