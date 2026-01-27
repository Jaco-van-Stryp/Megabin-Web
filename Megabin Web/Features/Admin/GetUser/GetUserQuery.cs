using MediatR;
using Megabin_Web.Shared.DTOs.Users;

namespace Megabin_Web.Features.Admin.GetUser;

public record GetUserQuery(Guid UserId) : IRequest<Shared.DTOs.Users.GetUser>;
