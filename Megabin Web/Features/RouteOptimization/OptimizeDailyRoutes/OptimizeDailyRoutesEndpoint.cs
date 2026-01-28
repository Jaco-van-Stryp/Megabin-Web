using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Megabin_Web.Shared.DTOs.Routing;

namespace Megabin_Web.Features.RouteOptimization.OptimizeDailyRoutes
{
    public static class OptimizeDailyRoutesEndpoint
    {
        public static IEndpointRouteBuilder MapOptimizeDailyRoutesEndpoint(
            this IEndpointRouteBuilder app
        )
        {
            app.MapPost(
                "OptimizeDailyRoutes",
                async Task<Results<Ok<DailyOptimizationResult>, BadRequest<string>>> (DateTime? targetDate, ISender sender) =>
                {
                    try
                    {
                        var command = new OptimizeDailyRoutesCommand(targetDate);
                        var result = await sender.Send(command);
                        return TypedResults.Ok(result);
                    }
                    catch (InvalidOperationException ex)
                    {
                        return TypedResults.BadRequest(ex.Message);
                    }
                }
            );
            return app;
        }
    }
}
