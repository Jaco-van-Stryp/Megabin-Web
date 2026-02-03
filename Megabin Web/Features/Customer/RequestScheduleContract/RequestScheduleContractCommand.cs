using MediatR;
using Megabin_Web.Shared.Domain.Enums;

namespace Megabin_Web.Features.Customer.RequestScheduleContract;

public record RequestScheduleContractCommand(
    Guid AddressId,
    Shared.Domain.Enums.DayOfWeek DayOfWeek,
    Frequency Frequency
) : IRequest<RequestScheduleContractResponseDto>;

public record RequestScheduleContractResponseDto
{
    public required Guid ContractId { get; init; }
}
