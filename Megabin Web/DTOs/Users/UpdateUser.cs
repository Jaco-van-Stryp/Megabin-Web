using Megabin_Web.Enums;

namespace Megabin_Web.DTOs.Users
{
    public class UpdateUser
    {
        public required Guid UserId { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required UserRoles Role { get; set; }
        public required int TotalBins { get; set; }
    }
}
