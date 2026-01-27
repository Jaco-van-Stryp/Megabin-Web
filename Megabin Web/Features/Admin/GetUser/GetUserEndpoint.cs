using MediatR;

namespace Megabin_Web.Features.Admin.GetUser
{
    public static class GetUserEndpoint
    {
        public static IEndpointRouteBuilder MapGetUserEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet(
                "GetUser/{userId}",
                async (Guid userId, ISender sender) =>
                {
                    try
                    {
                        var result = await sender.Send(new GetUserQuery(userId));
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
