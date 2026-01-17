using System.ComponentModel.DataAnnotations;
using Megabin_Web.Enums;

namespace Megabin_Web.DTOs.Auth
{
    public class RegisterRequest
    {
        public required string Name { get; set; }
        public required string Email { get; set; }

        [RegularExpression(
            @"^\+[1-9]\d{1,14}$",
            ErrorMessage = "Phone number must be in E.164 format"
        )]
        public required string PhoneNumber { get; set; }
        public required string Password { get; set; }
    }
}
