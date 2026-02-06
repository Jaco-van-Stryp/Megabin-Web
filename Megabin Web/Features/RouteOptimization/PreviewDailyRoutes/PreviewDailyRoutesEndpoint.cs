using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Megabin_Web.Features.RouteOptimization.PreviewDailyRoutes;

public static class PreviewDailyRoutesEndpoint
{
    public static IEndpointRouteBuilder MapPreviewDailyRoutesEndpoint(
        this IEndpointRouteBuilder app
    )
    {
        app.MapGet(
            "PreviewDailyRoutes",
            async Task<Ok<RoutePreviewDto>> (DateTime? targetDate, ISender sender) =>
            {
                var query = new PreviewDailyRoutesQuery(targetDate);
                var result = await sender.Send(query);
                return TypedResults.Ok(result);
            }
        ).WithDescription("Preview which schedule contracts will be included in route optimization for a given date");

        return app;
    }
}
