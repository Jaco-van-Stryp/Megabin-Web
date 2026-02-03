using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Megabin_Web.Features.Customer.UpdateProfile
{
    public static class UpdateProfileEndpoint
    {
        public static IEndpointRouteBuilder MapUpdateProfileEndpoint(
            this IEndpointRouteBuilder app
        )
        {
            app.MapPatch(
                "UpdateProfile",
                async Task<Results<Ok, NotFound<string>>> (
                    UpdateProfileCommand command,
                    ISender sender
                ) =>
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
