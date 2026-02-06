using MediatR;

namespace Megabin_Web.Features.Admin.UpdateDriver;

public record UpdateDriverCommand(
    Guid UserId,
    bool Active,
    string LicenseNumber,
    int VehicleCapacity,
    string HomeAddressLabel,
    double HomeAddressLong,
    double HomeAddressLat,
    string DropoffLocationLabel,
    double DropoffLocationLong,
    double DropoffLocationLat,
    string TimeZoneId
) : IRequest;
