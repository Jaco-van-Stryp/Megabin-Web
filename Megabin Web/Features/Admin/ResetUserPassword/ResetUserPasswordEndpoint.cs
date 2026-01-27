using MediatR;

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
                async (ResetUserPasswordCommand command, ISender sender) =>
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
