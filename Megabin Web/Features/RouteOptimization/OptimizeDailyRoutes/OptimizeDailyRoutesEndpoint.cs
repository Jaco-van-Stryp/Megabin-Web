using MediatR;

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
                async (DateTime? targetDate, ISender sender) =>
                {
                    try
                    {
                        var command = new OptimizeDailyRoutesCommand(targetDate);
                        var result = await sender.Send(command);
                        return Results.Ok(result);
                    }
                    catch (InvalidOperationException ex)
                    {
                        return Results.BadRequest(ex.Message);
                    }
                }
            );
            return app;
        }
    }
}
