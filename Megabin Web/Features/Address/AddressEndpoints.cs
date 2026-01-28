using Megabin_Web.Features.Address.AutoComplete;
using Megabin_Web.Features.Address.CreateAddress;
using Megabin_Web.Features.Address.DeleteAddress;
using Megabin_Web.Features.Address.GetAllAddresses;

namespace Megabin_Web.Features.Address
{
    public static class AddressEndpoints
    {
        public static IEndpointRouteBuilder MapAddressEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/Address").WithTags("Address").RequireAuthorization();
            group.MapAutoCompleteEndpoint();
            group.MapDeleteAddressEndpoint();
            group.MapCreateAddressEndpoint();
            group.MapGetAllAddressesEndpoint();
            return app;
        }
    }
}
