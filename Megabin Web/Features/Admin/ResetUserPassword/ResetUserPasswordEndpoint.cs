using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Megabin_Web.Features.Admin.ResetUserPassword
{
    public static class ResetUserPasswordEndpoint
    {
        public static IEndpointRouteBuilder MapResetUserPasswordEndpoint(
            this IEndpointRouteBuilder app
        )
        {
            app.MapPost(
                "ResetUserPassword",
                async Task<Results<Ok, NotFound<string>>> (ResetUserPasswordCommand command, ISender sender) =>
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
