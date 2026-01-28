using MediatR;

namespace Megabin_Web.Features.Admin.DisableDriver;

public record DisableDriverCommand(Guid UserId) : IRequest;
