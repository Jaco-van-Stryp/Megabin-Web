using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Megabin_Web.Features.Auth.GetProfile
{
    public static class GetProfileEndpoint
    {
        public static IEndpointRouteBuilder MapGetProfileEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet(
                    "me",
                    async Task<Results<Ok<ProfileResponse>, UnauthorizedHttpResult>> (ClaimsPrincipal user, ISender sender) =>
                    {
                        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        var email = user.FindFirst(ClaimTypes.Email)?.Value;
                        var role = user.FindFirst(ClaimTypes.Role)?.Value;

                        if (userId == null || email == null || role == null)
                        {
                            return TypedResults.Unauthorized();
                        }

                        var query = new GetProfileQuery(Guid.Parse(userId), email, role);
                        var result = await sender.Send(query);
                        return TypedResults.Ok(result);
                    }
                )
                .RequireAuthorization();
            return app;
        }
    }
}
