using MediatR;
using Megabin_Web.Shared.Domain.Data;
using Megabin_Web.Shared.DTOs.Drivers;
using Microsoft.EntityFrameworkCore;

namespace Megabin_Web.Features.Admin.GetDriver
{
    public class GetDriverHandler(AppDbContext dbContext)
        : IRequestHandler<GetDriverQuery, Shared.DTOs.Drivers.GetDriver>
    {
        public async Task<Shared.DTOs.Drivers.GetDriver> Handle(
            GetDriverQuery request,
            CancellationToken cancellationToken
        )
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

            return new Shared.DTOs.Drivers.GetDriver
            {
                DriverId = driver.DriverProfile.Id,
                HomeAddressLabel = driver.DriverProfile.HomeAddressLabel,
                HomeAddressLong = driver.DriverProfile.HomeAddressLong,
                HomeAddressLat = driver.DriverProfile.HomeAddressLat,
                DropoffLocationLabel = driver.DriverProfile.DropoffLocationLabel,
                DropoffLocationLong = driver.DriverProfile.DropoffLocationLong,
                DropoffLocationLat = driver.DriverProfile.DropoffLocationLat,
                VehicleCapacity = driver.DriverProfile.VehicleCapacity,
                LicenseNumber = driver.DriverProfile.LicenseNumber,
                Active = driver.DriverProfile.Active,
                UserId = driver.Id,
            };
        }
    }
}
