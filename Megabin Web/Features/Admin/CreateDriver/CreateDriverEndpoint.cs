using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

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
                async Task<Results<Ok, NotFound<string>, BadRequest<string>>> (CreateDriverCommand command, ISender sender) =>
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
                    catch (InvalidOperationException ex)
                    {
                        return TypedResults.BadRequest(ex.Message);
                    }
                }
            );
            return app;
        }
    }
}
