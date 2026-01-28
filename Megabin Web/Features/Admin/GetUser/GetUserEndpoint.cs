using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Megabin_Web.Features.Admin.GetUser
{
    public static class GetUserEndpoint
    {
        public static IEndpointRouteBuilder MapGetUserEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet(
                "GetUser/{userId}",
                async Task<Results<Ok<Shared.DTOs.Users.GetUser>, NotFound<string>>> (Guid userId, ISender sender) =>
                {
                    try
                    {
                        var result = await sender.Send(new GetUserQuery(userId));
                        return TypedResults.Ok(result);
                    }
                    catch (KeyNotFoundException ex)
                    {
                        return TypedResults.NotFound(ex.Message);
                    }
                }
            );
            return app;
        }
    }
}
