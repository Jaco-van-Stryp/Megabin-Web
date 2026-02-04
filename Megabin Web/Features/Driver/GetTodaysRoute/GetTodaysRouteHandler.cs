using MediatR;
using Megabin_Web.Shared.Domain.Data;
using Megabin_Web.Shared.Infrastructure.CurrentUserService;
using Microsoft.EntityFrameworkCore;

namespace Megabin_Web.Features.DriverDashboard.GetTodaysRoute;

public class GetTodaysRouteHandler(
    AppDbContext dbContext,
    ICurrentUserService currentUserService
) : IRequestHandler<GetTodaysRouteQuery, List<ScheduledCollectionDto>>
{
    public async Task<List<ScheduledCollectionDto>> Handle(
        GetTodaysRouteQuery request,
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

        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        var collections = await dbContext
            .ScheduledCollections.Include(sc => sc.Address)
            .Where(sc =>
                sc.UserId == driver.Id // UserId in ScheduledCollections stores DriverId
                && sc.ScheduledFor >= today
                && sc.ScheduledFor < tomorrow
            )
            .OrderBy(sc => sc.RouteSequence)
            .Select(sc => new ScheduledCollectionDto
            {
                Id = sc.Id,
                RouteSequence = sc.RouteSequence,
                Address = sc.Address.Address,
                Latitude = sc.Address.Lat,
                Longitude = sc.Address.Long,
                AddressNotes = sc.Address.AddressNotes,
                Collected = sc.Collected,
                Notes = sc.Notes,
                ScheduledFor = sc.ScheduledFor,
            })
            .ToListAsync(cancellationToken);

        return collections;
    }
}
