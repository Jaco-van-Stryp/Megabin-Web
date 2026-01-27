using MediatR;
using Megabin_Web.Shared.Domain.Data;
using Megabin_Web.Shared.DTOs.Users;

namespace Megabin_Web.Features.Admin.GetUser
{
    public class GetUserHandler(AppDbContext dbContext)
        : IRequestHandler<GetUserQuery, Shared.DTOs.Users.GetUser>
    {
        public async Task<Shared.DTOs.Users.GetUser> Handle(
            GetUserQuery request,
            CancellationToken cancellationToken
        )
        {
            var user = await dbContext.Users.FindAsync([request.UserId], cancellationToken);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {request.UserId} not found");
            }

            return new Shared.DTOs.Users.GetUser
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                PhoneNumber = user.PhoneNumber,
            };
        }
    }
}
