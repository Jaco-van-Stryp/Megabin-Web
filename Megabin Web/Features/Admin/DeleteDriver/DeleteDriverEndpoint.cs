using MediatR;

namespace Megabin_Web.Features.Admin.DeleteDriver
{
    public static class DeleteDriverEndpoint
    {
        public static IEndpointRouteBuilder MapDeleteDriverEndpoint(
            this IEndpointRouteBuilder app
        )
        {
            app.MapDelete(
                "DeleteDriver",
                async (Guid userId, ISender sender) =>
                {
                    try
                    {
                        await sender.Send(new DeleteDriverCommand(userId));
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
