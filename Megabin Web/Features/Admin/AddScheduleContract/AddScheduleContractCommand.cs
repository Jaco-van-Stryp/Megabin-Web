using MediatR;
using Megabin_Web.Shared.Domain.Enums;

namespace Megabin_Web.Features.Admin.AddScheduleContract;

public record AddScheduleContractCommand(
    Guid AddressId,
    Shared.Domain.Enums.DayOfWeek DayOfWeek,
    Frequency Frequency
) : IRequest;
