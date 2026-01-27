using MediatR;

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
                async (Guid userId, ISender sender) =>
                {
                    try
                    {
                        await sender.Send(new DisableDriverCommand(userId));
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
