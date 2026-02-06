using MediatR;
using Megabin_Web.Shared.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Megabin_Web.Features.Admin.UpdateDriver
{
    public class UpdateDriverHandler(AppDbContext dbContext)
        : IRequestHandler<UpdateDriverCommand>
    {
        public async Task Handle(UpdateDriverCommand request, CancellationToken cancellationToken)
        {
            // Validate TimeZoneId before processing
            if (string.IsNullOrWhiteSpace(request.TimeZoneId))
            {
                throw new ArgumentException("TimeZoneId is required", nameof(request.TimeZoneId));
            }

            try
            {
                TimeZoneInfo.FindSystemTimeZoneById(request.TimeZoneId);
            }
            catch (TimeZoneNotFoundException)
            {
                throw new ArgumentException(
                    $"Invalid timezone identifier: '{request.TimeZoneId}'",
                    nameof(request.TimeZoneId)
                );
            }

            var driver = await dbContext
                .Users.Include(x => x.DriverProfile)
                .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

            if (driver == null || driver.DriverProfile == null)
            {
                throw new KeyNotFoundException(
                    $"Driver profile not found for user ID {request.UserId}"
                );
            }

            driver.DriverProfile.Active = request.Active;
            driver.DriverProfile.LicenseNumber = request.LicenseNumber;
            driver.DriverProfile.VehicleCapacity = request.VehicleCapacity;
            driver.DriverProfile.HomeAddressLabel = request.HomeAddressLabel;
            driver.DriverProfile.HomeAddressLong = request.HomeAddressLong;
            driver.DriverProfile.HomeAddressLat = request.HomeAddressLat;
            driver.DriverProfile.DropoffLocationLabel = request.DropoffLocationLabel;
            driver.DriverProfile.DropoffLocationLong = request.DropoffLocationLong;
            driver.DriverProfile.DropoffLocationLat = request.DropoffLocationLat;
            driver.DriverProfile.TimeZoneId = request.TimeZoneId;

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
