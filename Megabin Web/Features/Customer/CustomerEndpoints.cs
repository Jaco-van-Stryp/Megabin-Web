using Megabin_Web.Features.Customer.CreateAddress;
using Megabin_Web.Features.Customer.GetMyScheduleContracts;
using Megabin_Web.Features.Customer.RequestBin;
using Megabin_Web.Features.Customer.RequestScheduleContract;
using Megabin_Web.Features.Customer.UpdateProfile;

namespace Megabin_Web.Features.Customer
{
    public static class CustomerEndpoints
    {
        public static IEndpointRouteBuilder MapCustomerEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/Customer")
                .WithTags("Customer")
                .RequireAuthorization();

            // Address Management
            group.MapCustomerCreateAddressEndpoint();
            group.MapRequestBinEndpoint();

            // Schedule Contract Management
            group.MapGetMyScheduleContractsEndpoint();
            group.MapRequestScheduleContractEndpoint();

            // Profile Management
            group.MapUpdateProfileEndpoint();

            return app;
        }
    }
}
