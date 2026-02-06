using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Megabin_Web.Features.Driver.UpdateCollectionStatus;

public static class UpdateCollectionStatusEndpoint
{
    public static IEndpointRouteBuilder MapUpdateCollectionStatusEndpoint(
        this IEndpointRouteBuilder app
    )
    {
        app.MapPatch(
            "Collection/{collectionId:guid}",
            async Task<Results<Ok, NotFound<string>, BadRequest<string>>> (
                Guid collectionId,
                UpdateCollectionStatusRequest request,
                ISender sender
            ) =>
            {
                try
                {
                    await sender.Send(
                        new UpdateCollectionStatusCommand(
                            collectionId,
                            request.Collected,
                            request.Notes
                        )
                    );
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

public record UpdateCollectionStatusRequest(bool? Collected, string? Notes);
