using MediatR;

namespace Megabin_Web.Features.Admin.GetScheduledContracts
{
    public static class GetScheduledContractsEndpoint
    {
        public static IEndpointRouteBuilder MapGetScheduledContractsEndpoint(
            this IEndpointRouteBuilder app
        )
        {
            app.MapGet(
                "GetScheduledContracts/{addressId}",
                async (Guid addressId, ISender sender) =>
                {
                    try
                    {
                        var result = await sender.Send(new GetScheduledContractsQuery(addressId));
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
