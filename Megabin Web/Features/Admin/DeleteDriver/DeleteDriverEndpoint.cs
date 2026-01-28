using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

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
                async Task<Results<Ok, NotFound<string>>> (Guid userId, ISender sender) =>
                {
                    try
                    {
                        await sender.Send(new DeleteDriverCommand(userId));
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
