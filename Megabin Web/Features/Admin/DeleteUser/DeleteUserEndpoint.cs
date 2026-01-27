using MediatR;

namespace Megabin_Web.Features.Admin.DeleteUser
{
    public static class DeleteUserEndpoint
    {
        public static IEndpointRouteBuilder MapDeleteUserEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapDelete(
                "DeleteUser/{userId}",
                async (Guid userId, ISender sender) =>
                {
                    try
                    {
                        await sender.Send(new DeleteUserCommand(userId));
                        return Results.Ok();
                    }
                    catch (KeyNotFoundException ex)
                    {
                        return Results.NotFound(ex.Message);
                    }
                }
            );
            return app;
        }
    }
}
