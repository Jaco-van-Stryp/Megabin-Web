using MediatR;

namespace Megabin_Web.Features.Address.GetAllAddresses
{
    public static class GetAllAddressesEndpoint
    {
        public static IEndpointRouteBuilder MapGetAllAddressesEndpoint(
            this IEndpointRouteBuilder app
        )
        {
            app.MapGet(
                "GetAllAddresses",
                async (ISender sender) =>
                {
                    return await sender.Send(new GetAllAddressesQuery());
                }
            );
            return app;
        }
    }
}
