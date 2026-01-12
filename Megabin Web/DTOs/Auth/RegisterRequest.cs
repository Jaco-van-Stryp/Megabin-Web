using System.ComponentModel.DataAnnotations;

namespace Megabin_Web.DTOs.Auth
{
    /// <summary>
    /// Request payload for user registration (admin operation).
    /// </summary>
    /// <param name="Name">The user's full name.</param>
    /// <param name="Email">The user's email address (must be unique).</param>
    /// <param name="Password">The user's password (minimum 8 characters recommended).</param>
    /// <param name="Role">The user's role: Admin, User, or Driver.</param>
    public record RegisterRequest(
        [Required, MinLength(2), MaxLength(100)] string Name,
        [Required, EmailAddress] string Email,
        [Required, MinLength(8)] string Password,
        [Required, RegularExpression("^(Admin|User|Driver)$", ErrorMessage = "Role must be Admin, User, or Driver")] string Role
    );
}
