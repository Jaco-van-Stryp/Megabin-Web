using System.Text.RegularExpressions;
using Megabin_Web.Features.Address.AutoComplete;
using Megabin_Web.Features.Address.DeleteAddress;

namespace Megabin_Web.Features.Address
{
    public static class AddressEndpoints
    {
        public static IEndpointRouteBuilder MapAddressEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/Address").WithTags("Address");
            group.MapAutoCompleteEndpoint();
            group.MapDeleteAddressEndpoint();
            return app;
        }
    }
}
