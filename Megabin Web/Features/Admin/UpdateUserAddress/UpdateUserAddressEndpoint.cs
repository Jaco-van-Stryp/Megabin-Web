using MediatR;

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
                async (UpdateUserAddressCommand command, ISender sender) =>
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
