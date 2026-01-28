using MediatR;
using Megabin_Web.Shared.Domain.Enums;

namespace Megabin_Web.Features.Admin.UpdateScheduleContract;

public record UpdateScheduleContractCommand(
    Guid ContractId,
    Frequency Frequency,
    Shared.Domain.Enums.DayOfWeek DayOfWeek,
    bool Active,
    bool ApprovedExternally
) : IRequest;
