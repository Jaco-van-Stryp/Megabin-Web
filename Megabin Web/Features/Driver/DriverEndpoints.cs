using Megabin_Web.Features.DriverDashboard.GetTodaysRoute;
using Megabin_Web.Features.DriverDashboard.UpdateCollectionStatus;

namespace Megabin_Web.Features.DriverDashboard;

public static class DriverEndpoints
{
    public static IEndpointRouteBuilder MapDriverEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("api/Driver")
            .WithTags("Driver")
            .RequireAuthorization(policy => policy.RequireRole("Driver"));

        // Route Management
        group.MapGetTodaysRouteEndpoint();
        group.MapUpdateCollectionStatusEndpoint();

        return app;
    }
}
