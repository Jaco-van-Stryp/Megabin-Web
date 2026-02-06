using Megabin_Web.Shared.Domain.Enums;

namespace Megabin_Web.Features.RouteOptimization.PreviewDailyRoutes;

/// <summary>
/// Preview of scheduled collections for a specific date before route optimization is triggered.
/// </summary>
public class RoutePreviewDto
{
    public required DateTime TargetDate { get; set; }
    public required Megabin_Web.Shared.Domain.Enums.DayOfWeek DayOfWeek { get; set; }
    public required int ActiveDriverCount { get; set; }
    public required int ExistingCollectionsCount { get; set; }
    public required List<ScheduleContractPreviewDto> ScheduleContracts { get; set; }
}

/// <summary>
/// Preview of a schedule contract that will be included in route optimization.
/// </summary>
public class ScheduleContractPreviewDto
{
    public required Guid ContractId { get; set; }
    public required Guid AddressId { get; set; }
    public required string Address { get; set; }
    public required string CustomerName { get; set; }
    public required string CustomerEmail { get; set; }
    public required Frequency Frequency { get; set; }
    public required int TotalBins { get; set; }
}
