using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Megabin_Web.Features.Admin.GetDriver
{
    public static class GetDriverEndpoint
    {
        public static IEndpointRouteBuilder MapGetDriverEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet(
                "GetDriver",
                async Task<Results<Ok<Shared.DTOs.Drivers.GetDriver>, NotFound<string>>> (Guid userId, ISender sender) =>
                {
                    try
                    {
                        var result = await sender.Send(new GetDriverQuery(userId));
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
}
