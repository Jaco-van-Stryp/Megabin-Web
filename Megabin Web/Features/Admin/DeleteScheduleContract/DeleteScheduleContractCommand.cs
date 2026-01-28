using MediatR;

namespace Megabin_Web.Features.Admin.DeleteScheduleContract;

public record DeleteScheduleContractCommand(Guid ContractId) : IRequest;
