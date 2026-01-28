using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Megabin_Web.Features.Admin.UpdateUser
{
    public static class UpdateUserEndpoint
    {
        public static IEndpointRouteBuilder MapUpdateUserEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost(
                "UpdateUser",
                async Task<Results<Ok, NotFound<string>>> (UpdateUserCommand command, ISender sender) =>
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
