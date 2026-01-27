using MediatR;

namespace Megabin_Web.Features.Admin.CreateDriver
{
    public static class CreateDriverEndpoint
    {
        public static IEndpointRouteBuilder MapCreateDriverEndpoint(
            this IEndpointRouteBuilder app
        )
        {
            app.MapPost(
                "CreateDriver",
                async (CreateDriverCommand command, ISender sender) =>
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
                    catch (InvalidOperationException ex)
                    {
                        return Results.BadRequest(ex.Message);
                    }
                }
            );
            return app;
        }
    }
}
