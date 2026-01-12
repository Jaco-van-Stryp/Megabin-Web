using System.ComponentModel.DataAnnotations;

namespace Megabin_Web.DTOs.Auth
{
    /// <summary>
    /// Request payload for user login authentication.
    /// </summary>
    /// <param name="Email">The user's email address used as login identifier.</param>
    /// <param name="Password">The user's password in plain text (transmitted over HTTPS only).</param>
    public record LoginRequest(
        [Required, EmailAddress] string Email,
        [Required, MinLength(8)] string Password
    );
}
