using MediatR;
using Megabin_Web.Features.RouteOptimization.OptimizeDailyRoutes;

namespace Megabin_Web.Shared.Infrastructure.OpenRouteService
{
    /// <summary>
    /// Background job wrapper for daily route optimization.
    /// Delegates to the OptimizeDailyRoutes feature handler via MediatR.
    /// </summary>
    public class RouteOptimizationBackgroundJob
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RouteOptimizationBackgroundJob> _logger;

        public RouteOptimizationBackgroundJob(
            IServiceProvider serviceProvider,
            ILogger<RouteOptimizationBackgroundJob> logger
        )
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// Optimizes routes for today's scheduled collections.
        /// This method is called by Hangfire as a recurring job.
        /// </summary>
        public async Task OptimizeRoutesAsync()
        {
            _logger.LogInformation(
                "Starting daily route optimization background job at {Time}",
                DateTime.UtcNow
            );

            try
            {
                // Create a new scope for dependency injection
                using var scope = _serviceProvider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

                // Delegate to the feature handler
                var command = new OptimizeDailyRoutesCommand(DateTime.Today);
                await mediator.Send(command);

                _logger.LogInformation(
                    "Daily route optimization background job completed successfully at {Time}",
                    DateTime.UtcNow
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during daily route optimization background job");
                throw; // Re-throw to mark job as failed in Hangfire
            }
        }
    }
}
