using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Megabin_Web.Features.Customer.RequestBin
{
    public static class RequestBinEndpoint
    {
        public static IEndpointRouteBuilder MapRequestBinEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPatch(
                "RequestBin/{addressId:guid}",
                async Task<Results<Ok, NotFound<string>, BadRequest<string>>> (
                    Guid addressId,
                    ISender sender
                ) =>
                {
                    try
                    {
                        await sender.Send(new RequestBinCommand(addressId));
                        return TypedResults.Ok();
                    }
                    catch (KeyNotFoundException ex)
                    {
                        return TypedResults.NotFound(ex.Message);
                    }
                    catch (InvalidOperationException ex)
                    {
                        return TypedResults.BadRequest(ex.Message);
                    }
                }
            );
            return app;
        }
    }
}
