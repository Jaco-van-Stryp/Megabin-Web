using MediatR;
using Megabin_Web.Shared.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Megabin_Web.Features.Admin.DeleteDriver
{
    public class DeleteDriverHandler(AppDbContext dbContext)
        : IRequestHandler<DeleteDriverCommand>
    {
        public async Task Handle(DeleteDriverCommand request, CancellationToken cancellationToken)
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

            dbContext.Drivers.Remove(driver.DriverProfile);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
