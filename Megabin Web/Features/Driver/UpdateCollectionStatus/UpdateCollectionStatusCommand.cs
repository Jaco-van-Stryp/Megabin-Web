using MediatR;

namespace Megabin_Web.Features.DriverDashboard.UpdateCollectionStatus;

public record UpdateCollectionStatusCommand(
    Guid CollectionId,
    bool? Collected,
    string? Notes
) : IRequest;
