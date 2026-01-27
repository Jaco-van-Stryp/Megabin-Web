using MediatR;
using Megabin_Web.Shared.DTOs.Drivers;

namespace Megabin_Web.Features.Admin.GetDriver;

public record GetDriverQuery(Guid UserId) : IRequest<Shared.DTOs.Drivers.GetDriver>;
