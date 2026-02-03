using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Megabin_Web.Features.Customer.GetMyScheduleContracts
{
    public static class GetMyScheduleContractsEndpoint
    {
        public static IEndpointRouteBuilder MapGetMyScheduleContractsEndpoint(
            this IEndpointRouteBuilder app
        )
        {
            app.MapGet(
                "GetMyScheduleContracts",
                async Task<Ok<List<CustomerScheduleContractDto>>> (ISender sender) =>
                {
                    var result = await sender.Send(new GetMyScheduleContractsQuery());
                    return TypedResults.Ok(result);
                }
            );
            return app;
        }
    }
}
