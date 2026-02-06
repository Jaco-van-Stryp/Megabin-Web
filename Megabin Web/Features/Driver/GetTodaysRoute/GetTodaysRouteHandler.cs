using MediatR;
using Megabin_Web.Shared.Domain.Data;
using Megabin_Web.Shared.Infrastructure.CurrentUserService;
using Microsoft.EntityFrameworkCore;

namespace Megabin_Web.Features.Driver.GetTodaysRoute;

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

        // Convert UTC to driver's local timezone to determine "today"
        var driverTimeZone = TimeZoneInfo.FindSystemTimeZoneById(driver.TimeZoneId);
        var driverLocalNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, driverTimeZone);
        var todayLocal = driverLocalNow.Date;
        var tomorrowLocal = todayLocal.AddDays(1);

        // Normalize date bounds to UTC for comparison with ScheduledFor (which is stored as UTC)
        var todayUtc = DateTime.SpecifyKind(todayLocal, DateTimeKind.Utc);
        var tomorrowUtc = DateTime.SpecifyKind(tomorrowLocal, DateTimeKind.Utc);

        var collections = await dbContext
            .ScheduledCollections.Include(sc => sc.Address)
            .Where(sc =>
                sc.UserId == driver.Id // UserId in ScheduledCollections stores DriverId
                && sc.ScheduledFor >= todayUtc
                && sc.ScheduledFor < tomorrowUtc
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
