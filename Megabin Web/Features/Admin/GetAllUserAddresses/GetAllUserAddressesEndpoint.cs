using MediatR;

namespace Megabin_Web.Features.Admin.GetAllUserAddresses
{
    public static class GetAllUserAddressesEndpoint
    {
        public static IEndpointRouteBuilder MapGetAllUserAddressesEndpoint(
            this IEndpointRouteBuilder app
        )
        {
            app.MapGet(
                "GetAllUserAddresses/{userId}",
                async (Guid userId, ISender sender) =>
                {
                    try
                    {
                        var result = await sender.Send(new GetAllUserAddressesQuery(userId));
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
