using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Megabin_Web.Shared.DTOs.Addresses;

namespace Megabin_Web.Features.Admin.GetAllUserAddresses
{
    public static class GetAllUserAddressesEndpoint
    {
        public static IEndpointRouteBuilder MapGetAllUserAddressesEndpoint(
            this IEndpointRouteBuilder app
        )
        {
            app.MapGet(
                "GetAllUserAddresses/{userId}",
                async Task<Results<Ok<List<GetAddress>>, NotFound<string>>> (Guid userId, ISender sender) =>
                {
                    try
                    {
                        var result = await sender.Send(new GetAllUserAddressesQuery(userId));
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
