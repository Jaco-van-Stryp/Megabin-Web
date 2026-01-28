using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Megabin_Web.Features.Admin.DisableDriver
{
    public static class DisableDriverEndpoint
    {
        public static IEndpointRouteBuilder MapDisableDriverEndpoint(
            this IEndpointRouteBuilder app
        )
        {
            app.MapPost(
                "DisableDriver",
                async Task<Results<Ok, NotFound<string>>> (Guid userId, ISender sender) =>
                {
                    try
                    {
                        await sender.Send(new DisableDriverCommand(userId));
                        return TypedResults.Ok();
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
