using MediatR;
using Megabin_Web.Shared.Domain.Enums;

namespace Megabin_Web.Features.Admin.UpdateUser;

public record UpdateUserCommand(
    Guid UserId,
    string Name,
    string Email,
    UserRoles Role,
    string PhoneNumber
) : IRequest;
