using MediatR;

namespace Megabin_Web.Features.RouteOptimization.PreviewDailyRoutes;

/// <summary>
/// Query to preview what schedule contracts will be included in route optimization for a specific date.
/// If no date is provided, previews for today.
/// </summary>
public record PreviewDailyRoutesQuery(DateTime? TargetDate = null) : IRequest<RoutePreviewDto>;
