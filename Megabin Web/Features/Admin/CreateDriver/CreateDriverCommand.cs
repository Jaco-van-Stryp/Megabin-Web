using MediatR;

namespace Megabin_Web.Features.Admin.CreateDriver;

public record CreateDriverCommand(
    Guid UserId,
    bool Active,
    string LicenseNumber,
    string HomeAddressLabel,
    double HomeAddressLong,
    double HomeAddressLat,
    string DropoffLocationLabel,
    double DropoffLocationLong,
    double DropoffLocationLat,
    int VehicleCapacity,
    string TimeZoneId
) : IRequest;
