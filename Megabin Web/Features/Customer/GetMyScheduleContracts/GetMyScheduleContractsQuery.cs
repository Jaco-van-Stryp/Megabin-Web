using MediatR;

namespace Megabin_Web.Features.Customer.GetMyScheduleContracts;

public record GetMyScheduleContractsQuery() : IRequest<List<CustomerScheduleContractDto>>;

public record CustomerScheduleContractDto
{
    public required Guid Id { get; init; }
    public required Guid AddressId { get; init; }
    public required string Address { get; init; }
    public required Shared.Domain.Enums.Frequency Frequency { get; init; }
    public required Shared.Domain.Enums.DayOfWeek DayOfWeek { get; init; }
    public required DateTime StartingDate { get; init; }
    public required bool Active { get; init; }
    public required bool ApprovedExternally { get; init; }
}
