using Megabin_Web.Enums;

namespace Megabin_Web.DTOs.Auth
{
    public class RegisterRequest
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Password { get; set; }
    }
}
