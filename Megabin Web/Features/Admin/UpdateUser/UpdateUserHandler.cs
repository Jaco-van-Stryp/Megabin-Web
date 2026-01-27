using MediatR;
using Megabin_Web.Shared.Domain.Data;

namespace Megabin_Web.Features.Admin.UpdateUser
{
    public class UpdateUserHandler(AppDbContext dbContext) : IRequestHandler<UpdateUserCommand>
    {
        public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await dbContext.Users.FindAsync([request.UserId], cancellationToken);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {request.UserId} not found");
            }

            user.Name = request.Name;
            user.Email = request.Email;
            user.Role = request.Role;
            user.PhoneNumber = request.PhoneNumber;

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
