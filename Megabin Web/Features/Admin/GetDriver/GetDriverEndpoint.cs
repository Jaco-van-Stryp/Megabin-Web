using MediatR;

namespace Megabin_Web.Features.Admin.GetDriver
{
    public static class GetDriverEndpoint
    {
        public static IEndpointRouteBuilder MapGetDriverEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet(
                "GetDriver",
                async (Guid userId, ISender sender) =>
                {
                    try
                    {
                        var result = await sender.Send(new GetDriverQuery(userId));
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
