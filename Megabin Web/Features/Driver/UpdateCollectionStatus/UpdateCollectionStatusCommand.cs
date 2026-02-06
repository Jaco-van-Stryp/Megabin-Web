using MediatR;

namespace Megabin_Web.Features.Driver.UpdateCollectionStatus;

public record UpdateCollectionStatusCommand(
    Guid CollectionId,
    bool? Collected,
    string? Notes
) : IRequest;
