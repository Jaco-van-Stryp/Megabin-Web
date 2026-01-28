using Megabin_Web.Shared.Domain.Enums;

namespace Megabin_Web.Shared.DTOs.Users
{
    public class GetUser
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required UserRoles Role { get; set; }
    }
}
