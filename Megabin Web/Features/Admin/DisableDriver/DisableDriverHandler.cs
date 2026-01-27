using MediatR;
using Megabin_Web.Shared.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Megabin_Web.Features.Admin.DisableDriver
{
    public class DisableDriverHandler(AppDbContext dbContext)
        : IRequestHandler<DisableDriverCommand>
    {
        public async Task Handle(DisableDriverCommand request, CancellationToken cancellationToken)
        {
            var driver = await dbContext
                .Users.Include(x => x.DriverProfile)
                .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

            if (driver == null || driver.DriverProfile == null)
            {
                throw new KeyNotFoundException(
                    $"Driver profile not found for user ID {request.UserId}"
                );
            }

            driver.DriverProfile.Active = false;
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
