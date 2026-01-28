using MediatR;
using Megabin_Web.Shared.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Megabin_Web.Features.Admin.DeleteUser
{
    public class DeleteUserHandler(AppDbContext dbContext) : IRequestHandler<DeleteUserCommand>
    {
        public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            // Load user with all related entities
            var user = await dbContext
                .Users.Include(u => u.Addresss)
                .ThenInclude(a => a.Schedules)
                .Include(u => u.ApiUsageTracker)
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {request.UserId} not found");
            }

            // 1. Delete all ScheduleContracts for all user addresses
            foreach (var address in user.Addresss)
            {
                dbContext.ScheduledContract.RemoveRange(address.Schedules);
            }

            // 2. Delete Driver profile if user is a driver
            var driver = await dbContext
                .Drivers.FirstOrDefaultAsync(d => d.UserId == request.UserId, cancellationToken);
            if (driver != null)
            {
                dbContext.Drivers.Remove(driver);
            }

            // 3. Delete ScheduledCollections where user is assigned as driver
            var scheduledCollections = await dbContext
                .ScheduledCollections.Where(sc => sc.UserId == request.UserId)
                .ToListAsync(cancellationToken);
            dbContext.ScheduledCollections.RemoveRange(scheduledCollections);

            // 4. Delete all APIUsageTracker records
            dbContext.APIUsageTrackers.RemoveRange(user.ApiUsageTracker);

            // 5. Delete all Addresses (already loaded via Include)
            dbContext.Addresses.RemoveRange(user.Addresss);

            // 6. Finally, delete the user
            dbContext.Users.Remove(user);

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
