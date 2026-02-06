using Megabin_Web.Features.RouteOptimization.OptimizeDailyRoutes;
using Megabin_Web.Features.RouteOptimization.PreviewDailyRoutes;

namespace Megabin_Web.Features.RouteOptimization
{
    public static class RouteOptimizationEndpoints
    {
        public static IEndpointRouteBuilder MapRouteOptimizationEndpoints(
            this IEndpointRouteBuilder app
        )
        {
            var group = app.MapGroup("api/RouteOptimization")
                .WithTags("RouteOptimization")
                .RequireAuthorization(policy => policy.RequireRole("Admin"));

            group.MapOptimizeDailyRoutesEndpoint();
            group.MapPreviewDailyRoutesEndpoint();

            return app;
        }
    }
}
