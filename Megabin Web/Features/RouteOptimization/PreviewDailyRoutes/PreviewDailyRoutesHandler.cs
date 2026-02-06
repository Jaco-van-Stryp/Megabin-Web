using MediatR;
using Megabin_Web.Shared.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Megabin_Web.Features.RouteOptimization.PreviewDailyRoutes;

public class PreviewDailyRoutesHandler(
    AppDbContext dbContext,
    ILogger<PreviewDailyRoutesHandler> logger
) : IRequestHandler<PreviewDailyRoutesQuery, RoutePreviewDto>
{
    public async Task<RoutePreviewDto> Handle(
        PreviewDailyRoutesQuery request,
        CancellationToken cancellationToken
    )
    {
        // Ensure we're working with UTC dates for PostgreSQL compatibility
        var targetDate = request.TargetDate.HasValue
            ? DateTime.SpecifyKind(request.TargetDate.Value.Date, DateTimeKind.Utc)
            : DateTime.UtcNow.Date;

        logger.LogInformation(
            "Previewing route optimization for date {Date}",
            targetDate
        );

        // Convert System.DayOfWeek to custom DayOfWeek enum
        // System.DayOfWeek: Sunday=0, Monday=1, ..., Saturday=6
        // Custom enum: Monday=0, Tuesday=1, ..., Sunday=6
        var customDayOfWeek = ConvertToCustomDayOfWeek(targetDate.DayOfWeek);

        // Get all active schedule contracts that need collection on this day of week
        var activeContracts = await dbContext
            .ScheduledContract.Include(sc => sc.Addresses)
            .ThenInclude(a => a.User)
            .Where(sc =>
                sc.Active
                && sc.ApprovedExternally
                && sc.DayOfWeek == customDayOfWeek
            )
            .Select(sc => new ScheduleContractPreviewDto
            {
                ContractId = sc.Id,
                AddressId = sc.AddressesId,
                Address = sc.Addresses.Address,
                CustomerName = sc.Addresses.User.Name ?? "Unknown",
                CustomerEmail = sc.Addresses.User.Email ?? "",
                Frequency = sc.Frequency,
                TotalBins = sc.Addresses.TotalBins,
            })
            .ToListAsync(cancellationToken);

        // Get count of active drivers
        var activeDriverCount = await dbContext
            .Drivers
            .Where(d => d.Active)
            .CountAsync(cancellationToken);

        // Get count of existing scheduled collections for this date
        var existingCollectionsCount = await dbContext
            .ScheduledCollections
            .Where(sc => sc.ScheduledFor.Date == targetDate.Date)
            .CountAsync(cancellationToken);

        logger.LogInformation(
            "Preview complete: {ContractCount} contracts for {DayOfWeek}, {DriverCount} active drivers, {ExistingCount} existing collections",
            activeContracts.Count,
            customDayOfWeek,
            activeDriverCount,
            existingCollectionsCount
        );

        return new RoutePreviewDto
        {
            TargetDate = targetDate,
            DayOfWeek = customDayOfWeek,
            ActiveDriverCount = activeDriverCount,
            ExistingCollectionsCount = existingCollectionsCount,
            ScheduleContracts = activeContracts,
        };
    }

    /// <summary>
    /// Converts System.DayOfWeek to custom DayOfWeek enum.
    /// System.DayOfWeek: Sunday=0, Monday=1, ..., Saturday=6
    /// Custom enum: Monday=0, Tuesday=1, ..., Sunday=6
    /// </summary>
    private static Shared.Domain.Enums.DayOfWeek ConvertToCustomDayOfWeek(System.DayOfWeek systemDayOfWeek)
    {
        return systemDayOfWeek switch
        {
            System.DayOfWeek.Monday => Shared.Domain.Enums.DayOfWeek.Monday,
            System.DayOfWeek.Tuesday => Shared.Domain.Enums.DayOfWeek.Tuesday,
            System.DayOfWeek.Wednesday => Shared.Domain.Enums.DayOfWeek.Wednesday,
            System.DayOfWeek.Thursday => Shared.Domain.Enums.DayOfWeek.Thursday,
            System.DayOfWeek.Friday => Shared.Domain.Enums.DayOfWeek.Friday,
            System.DayOfWeek.Saturday => Shared.Domain.Enums.DayOfWeek.Saturday,
            System.DayOfWeek.Sunday => Shared.Domain.Enums.DayOfWeek.Sunday,
            _ => throw new ArgumentOutOfRangeException(nameof(systemDayOfWeek))
        };
    }
}
