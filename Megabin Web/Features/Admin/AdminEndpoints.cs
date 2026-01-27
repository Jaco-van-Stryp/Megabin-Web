using Megabin_Web.Features.Admin.AddScheduleContract;
using Megabin_Web.Features.Admin.AddUserAddress;
using Megabin_Web.Features.Admin.CreateDriver;
using Megabin_Web.Features.Admin.DeleteDriver;
using Megabin_Web.Features.Admin.DeleteScheduleContract;
using Megabin_Web.Features.Admin.DeleteUser;
using Megabin_Web.Features.Admin.DisableDriver;
using Megabin_Web.Features.Admin.GetAllUserAddresses;
using Megabin_Web.Features.Admin.GetAllUsers;
using Megabin_Web.Features.Admin.GetDriver;
using Megabin_Web.Features.Admin.GetScheduledContracts;
using Megabin_Web.Features.Admin.GetUser;
using Megabin_Web.Features.Admin.ResetUserPassword;
using Megabin_Web.Features.Admin.UpdateDriver;
using Megabin_Web.Features.Admin.UpdateScheduleContract;
using Megabin_Web.Features.Admin.UpdateUser;
using Megabin_Web.Features.Admin.UpdateUserAddress;

namespace Megabin_Web.Features.Admin
{
    public static class AdminEndpoints
    {
        public static IEndpointRouteBuilder MapAdminEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/Admin")
                .WithTags("Admin")
                .RequireAuthorization(policy => policy.RequireRole("Admin"));

            // User Management
            group.MapGetAllUsersEndpoint();
            group.MapGetUserEndpoint();
            group.MapUpdateUserEndpoint();
            group.MapDeleteUserEndpoint();
            group.MapResetUserPasswordEndpoint();

            // Address Management
            group.MapAddUserAddressEndpoint();
            group.MapUpdateUserAddressEndpoint();
            group.MapGetAllUserAddressesEndpoint();

            // Schedule Contract Management
            group.MapAddScheduleContractEndpoint();
            group.MapGetScheduledContractsEndpoint();
            group.MapUpdateScheduleContractEndpoint();
            group.MapDeleteScheduleContractEndpoint();

            // Driver Management
            group.MapCreateDriverEndpoint();
            group.MapUpdateDriverEndpoint();
            group.MapDeleteDriverEndpoint();
            group.MapDisableDriverEndpoint();
            group.MapGetDriverEndpoint();

            return app;
        }
    }
}
