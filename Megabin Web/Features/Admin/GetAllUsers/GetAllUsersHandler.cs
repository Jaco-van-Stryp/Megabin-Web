using MediatR;
using Megabin_Web.Shared.Domain.Data;
using Megabin_Web.Shared.DTOs.Users;
using Microsoft.EntityFrameworkCore;

namespace Megabin_Web.Features.Admin.GetAllUsers
{
    public class GetAllUsersHandler(AppDbContext _context)
        : IRequestHandler<GetAllUsersQuery, List<Shared.DTOs.Users.GetUser>>
    {
        public async Task<List<Shared.DTOs.Users.GetUser>> Handle(
            GetAllUsersQuery request,
            CancellationToken cancellationToken
        )
        {
            var users = await _context.Users.ToListAsync();
            var userList = new List<Shared.DTOs.Users.GetUser>();
            foreach (var user in users)
            {
                userList.Add(
                    new Shared.DTOs.Users.GetUser
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Name = user.Name,
                        PhoneNumber = user.PhoneNumber,
                        Role = user.Role,
                    }
                );
            }
            return userList;
        }
    }
}
