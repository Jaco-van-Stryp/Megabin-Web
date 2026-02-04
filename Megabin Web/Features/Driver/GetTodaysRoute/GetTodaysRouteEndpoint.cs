using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Megabin_Web.Features.DriverDashboard.GetTodaysRoute;

public static class GetTodaysRouteEndpoint
{
    public static IEndpointRouteBuilder MapGetTodaysRouteEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet(
            "TodaysRoute",
            async Task<Results<Ok<List<ScheduledCollectionDto>>, NotFound<string>>> (
                ISender sender
            ) =>
            {
                try
                {
                    var result = await sender.Send(new GetTodaysRouteQuery());
                    return TypedResults.Ok(result);
                }
                catch (KeyNotFoundException ex)
                {
                    return TypedResults.NotFound(ex.Message);
                }
            }
        );
        return app;
    }
}
