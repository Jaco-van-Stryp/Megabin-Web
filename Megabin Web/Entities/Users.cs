using Megabin_Web.Enums;

namespace Megabin_Web.Entities
{
    public class Users
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string PasswordHash { get; set; }
        public required UserRoles Role { get; set; } = UserRoles.Customer;
        public ICollection<Addresses> Addresss { get; set; } = new List<Addresses>();
        public ICollection<APIUsageTracker> ApiUsageTracker { get; set; } =
            new List<APIUsageTracker>();

        // Users can also have a driver profile if they are drivers
        public Driver? DriverProfile { get; set; }
    }
}
