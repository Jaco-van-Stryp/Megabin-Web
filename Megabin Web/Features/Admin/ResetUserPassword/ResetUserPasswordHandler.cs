using MediatR;
using Megabin_Web.Shared.Domain.Data;
using Megabin_Web.Shared.Infrastructure.PasswordService;

namespace Megabin_Web.Features.Admin.ResetUserPassword
{
    public class ResetUserPasswordHandler(AppDbContext dbContext, IPasswordService passwordService)
        : IRequestHandler<ResetUserPasswordCommand>
    {
        public async Task Handle(
            ResetUserPasswordCommand request,
            CancellationToken cancellationToken
        )
        {
            var user = await dbContext.Users.FindAsync([request.UserId], cancellationToken);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {request.UserId} not found");
            }

            // Hash the new password and update the user
            user.PasswordHash = passwordService.HashPassword(request.NewPassword);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
