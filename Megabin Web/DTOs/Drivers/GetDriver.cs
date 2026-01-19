namespace Megabin_Web.DTOs.Drivers
{
    public class GetDriver
    {
        public Guid DriverId { get; set; }
        public required string HomeAddressLabel { get; set; }
        public required double HomeAddressLong { get; set; }
        public required double HomeAddressLat { get; set; }
        public required string DropoffLocationLabel { get; set; }
        public required double DropoffLocationLong { get; set; }
        public required double DropoffLocationLat { get; set; }
        public required int VehicleCapacity { get; set; } // Max stops before depot
        public required string LicenseNumber { get; set; }
        public required bool Active { get; set; }
        public required Guid UserId { get; set; }
    }
}
