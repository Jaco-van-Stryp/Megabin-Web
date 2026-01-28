using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Megabin_Web.Features.Admin.DeleteUser
{
    public static class DeleteUserEndpoint
    {
        public static IEndpointRouteBuilder MapDeleteUserEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapDelete(
                "DeleteUser/{userId}",
                async Task<Results<Ok, NotFound<string>>> (Guid userId, ISender sender) =>
                {
                    try
                    {
                        await sender.Send(new DeleteUserCommand(userId));
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
