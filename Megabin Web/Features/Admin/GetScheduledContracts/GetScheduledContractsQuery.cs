using MediatR;
using Megabin_Web.Shared.DTOs.ScheduleContracts;

namespace Megabin_Web.Features.Admin.GetScheduledContracts;

public record GetScheduledContractsQuery(Guid AddressId) : IRequest<List<GetScheduleContract>>;
