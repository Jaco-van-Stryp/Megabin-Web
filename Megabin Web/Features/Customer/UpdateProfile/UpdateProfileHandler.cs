using MediatR;
using Megabin_Web.Shared.Domain.Data;
using Megabin_Web.Shared.Infrastructure.CurrentUserService;
using Microsoft.EntityFrameworkCore;

namespace Megabin_Web.Features.Customer.UpdateProfile
{
    public class UpdateProfileHandler(
        AppDbContext dbContext,
        ICurrentUserService currentUserService
    ) : IRequestHandler<UpdateProfileCommand>
    {
        public async Task Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var userId = currentUserService.GetUserId();

            var user = await dbContext
                .Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            user.Name = request.Name;
            user.PhoneNumber = request.PhoneNumber;

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
