using MediatR;
using Megabin_Web.Shared.Domain.Data;
using Megabin_Web.Shared.Domain.Entities;
using Megabin_Web.Shared.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Megabin_Web.Features.Admin.CreateDriver
{
    public class CreateDriverHandler(AppDbContext dbContext)
        : IRequestHandler<CreateDriverCommand>
    {
        public async Task Handle(CreateDriverCommand request, CancellationToken cancellationToken)
        {
            var user = await dbContext
                .Users.Include(x => x.DriverProfile)
                .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {request.UserId} not found");
            }

            if (user.DriverProfile != null)
            {
                throw new InvalidOperationException("User is already a driver");
            }

            var driver = new Driver
            {
                Active = request.Active,
                LicenseNumber = request.LicenseNumber,
                UserId = user.Id,
                User = user,
                HomeAddressLabel = request.HomeAddressLabel,
                HomeAddressLong = request.HomeAddressLong,
                HomeAddressLat = request.HomeAddressLat,
                DropoffLocationLabel = request.DropoffLocationLabel,
                DropoffLocationLong = request.DropoffLocationLong,
                DropoffLocationLat = request.DropoffLocationLat,
                VehicleCapacity = request.VehicleCapacity,
            };

            // Also set the user's role to Driver
            user.Role = UserRoles.Driver;

            dbContext.Drivers.Add(driver);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
