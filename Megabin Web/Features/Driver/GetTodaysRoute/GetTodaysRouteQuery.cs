using MediatR;

namespace Megabin_Web.Features.DriverDashboard.GetTodaysRoute;

public record GetTodaysRouteQuery() : IRequest<List<ScheduledCollectionDto>>;

public record ScheduledCollectionDto
{
    public required Guid Id { get; init; }
    public required int RouteSequence { get; init; }
    public required string Address { get; init; }
    public required double Latitude { get; init; }
    public required double Longitude { get; init; }
    public required string? AddressNotes { get; init; }
    public required bool Collected { get; init; }
    public required string Notes { get; init; }
    public required DateTime ScheduledFor { get; init; }
}
