using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Megabin_Web.Features.Address.CreateAddress;

namespace Megabin_Web.Features.Admin.AddUserAddress
{
    public static class AddUserAddressEndpoint
    {
        public static IEndpointRouteBuilder MapAddUserAddressEndpoint(
            this IEndpointRouteBuilder app
        )
        {
            app.MapPost(
                "AddUserAddress",
                async Task<Results<Ok<CreateAddressResponseDto>, NotFound<string>>> (AddUserAddressCommand command, ISender sender) =>
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
