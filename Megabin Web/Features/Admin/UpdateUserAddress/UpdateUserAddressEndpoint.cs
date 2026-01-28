using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Megabin_Web.Features.Admin.UpdateUserAddress
{
    public static class UpdateUserAddressEndpoint
    {
        public static IEndpointRouteBuilder MapUpdateUserAddressEndpoint(
            this IEndpointRouteBuilder app
        )
        {
            app.MapPost(
                "UpdateUserAddress",
                async Task<Results<Ok, NotFound<string>>> (UpdateUserAddressCommand command, ISender sender) =>
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
