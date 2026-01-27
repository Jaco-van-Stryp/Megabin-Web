using MediatR;

namespace Megabin_Web.Features.Admin.UpdateUser
{
    public static class UpdateUserEndpoint
    {
        public static IEndpointRouteBuilder MapUpdateUserEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost(
                "UpdateUser",
                async (UpdateUserCommand command, ISender sender) =>
                {
                    try
                    {
                        await sender.Send(command);
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
