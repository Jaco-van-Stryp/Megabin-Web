using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Megabin_Web.Features.Admin.UpdateDriver
{
    public static class UpdateDriverEndpoint
    {
        public static IEndpointRouteBuilder MapUpdateDriverEndpoint(
            this IEndpointRouteBuilder app
        )
        {
            app.MapPost(
                "UpdateDriver",
                async Task<Results<Ok, NotFound<string>>> (UpdateDriverCommand command, ISender sender) =>
                {
                    try
                    {
                        await sender.Send(command);
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
