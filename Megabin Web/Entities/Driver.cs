namespace Megabin_Web.Entities
{
    public class Driver
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required Guid HomeAddressID { get; set; }
        public required Addresses HomeAddress { get; set; }
        public required int VehicleCapacity { get; set; } // Max stops before depot
        public required string LicenseNumber { get; set; }
        public required bool Active { get; set; }
        public required Guid UserId { get; set; }
        public required Users User { get; set; }
    }
}
