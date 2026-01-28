using MediatR;
using Megabin_Web.Shared.DTOs.Routing;

namespace Megabin_Web.Features.RouteOptimization.OptimizeDailyRoutes;

/// <summary>
/// Command to optimize routes for a specific date.
/// If no date is provided, optimizes for today.
/// </summary>
public record OptimizeDailyRoutesCommand(DateTime? TargetDate = null)
    : IRequest<DailyOptimizationResult>;
