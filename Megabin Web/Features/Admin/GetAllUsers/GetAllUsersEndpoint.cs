using MediatR;

namespace Megabin_Web.Features.Admin.GetAllUsers
{
    public static class GetAllUsersEndpoint
    {
        public static void MapGetAllUsersEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet(
                    "GetAllUsers",
                    async (ISender sender) =>
                    {
                        var result = await sender.Send(new GetAllUsersQuery());
                        return Results.Ok(result);
                    }
                )
                .WithTags("Admin")
                .RequireAuthorization("AdminPolicy");
        }
    }
}
