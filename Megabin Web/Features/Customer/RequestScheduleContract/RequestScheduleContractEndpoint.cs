using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Megabin_Web.Features.Customer.RequestScheduleContract
{
    public static class RequestScheduleContractEndpoint
    {
        public static IEndpointRouteBuilder MapRequestScheduleContractEndpoint(
            this IEndpointRouteBuilder app
        )
        {
            app.MapPost(
                "RequestScheduleContract",
                async Task<Results<Ok<RequestScheduleContractResponseDto>, NotFound<string>>> (
                    RequestScheduleContractCommand command,
                    ISender sender
                ) =>
                {
                    try
                    {
                        var result = await sender.Send(command);
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
