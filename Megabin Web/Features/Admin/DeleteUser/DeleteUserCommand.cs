using MediatR;

namespace Megabin_Web.Features.Admin.DeleteUser;

public record DeleteUserCommand(Guid UserId) : IRequest;
