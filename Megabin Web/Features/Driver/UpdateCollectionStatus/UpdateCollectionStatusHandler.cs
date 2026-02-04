using MediatR;
using Megabin_Web.Shared.Domain.Data;
using Megabin_Web.Shared.Infrastructure.CurrentUserService;
using Microsoft.EntityFrameworkCore;

namespace Megabin_Web.Features.DriverDashboard.UpdateCollectionStatus;

public class UpdateCollectionStatusHandler(
    AppDbContext dbContext,
    ICurrentUserService currentUserService
) : IRequestHandler<UpdateCollectionStatusCommand>
{
    public async Task Handle(
        UpdateCollectionStatusCommand request,
        CancellationToken cancellationToken
    )
    {
        var userId = currentUserService.GetUserId();

        // Find the driver record for this user
        var driver = await dbContext
            .Drivers.FirstOrDefaultAsync(d => d.UserId == userId, cancellationToken);

        if (driver == null)
        {
            throw new KeyNotFoundException("Driver profile not found for current user");
        }

        var collection = await dbContext
            .ScheduledCollections.FirstOrDefaultAsync(
                sc => sc.Id == request.CollectionId && sc.UserId == driver.Id,
                cancellationToken
            );

        if (collection == null)
        {
            throw new KeyNotFoundException(
                $"Collection with ID {request.CollectionId} not found or does not belong to you"
            );
        }

        collection.Collected = request.Collected;
        if (request.Notes != null)
        {
            collection.Notes = request.Notes;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
