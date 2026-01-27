using MediatR;

namespace Megabin_Web.Features.Admin.AddUserAddress
{
    public static class AddUserAddressEndpoint
    {
        public static IEndpointRouteBuilder MapAddUserAddressEndpoint(
            this IEndpointRouteBuilder app
        )
        {
            app.MapPost(
                "AddUserAddress",
                async (AddUserAddressCommand command, ISender sender) =>
                {
                    try
                    {
                        var result = await sender.Send(command);
                        return Results.Ok(result);
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
