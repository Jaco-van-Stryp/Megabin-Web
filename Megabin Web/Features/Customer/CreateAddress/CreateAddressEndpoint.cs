using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Megabin_Web.Features.Customer.CreateAddress
{
    public static class CustomerCreateAddressEndpoint
    {
        public static IEndpointRouteBuilder MapCustomerCreateAddressEndpoint(
            this IEndpointRouteBuilder app
        )
        {
            app.MapPost(
                "CreateAddress",
                async Task<Results<Ok<CustomerCreateAddressResponseDto>, NotFound<string>>> (
                    CustomerCreateAddressCommand command,
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
