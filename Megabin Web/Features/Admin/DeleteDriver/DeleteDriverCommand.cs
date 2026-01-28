using MediatR;

namespace Megabin_Web.Features.Admin.DeleteDriver;

public record DeleteDriverCommand(Guid UserId) : IRequest;
