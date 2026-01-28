using MediatR;
using Megabin_Web.Shared.DTOs.Users;

namespace Megabin_Web.Features.Admin.GetAllUsers
{
    public record GetAllUsersQuery() : IRequest<List<Shared.DTOs.Users.GetUser>>;
}
