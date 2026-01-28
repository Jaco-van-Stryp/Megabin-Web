using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Megabin_Web.Shared.DTOs.ScheduleContracts;

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
                async Task<Results<Ok<List<GetScheduleContract>>, NotFound<string>>> (Guid addressId, ISender sender) =>
                {
                    try
                    {
                        var result = await sender.Send(new GetScheduledContractsQuery(addressId));
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
