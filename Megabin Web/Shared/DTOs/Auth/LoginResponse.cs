using Megabin_Web.Shared.Domain.Enums;

namespace Megabin_Web.Shared.DTOs.Auth
{
    /// <summary>
    /// Response payload for successful login authentication.
    /// </summary>
    /// <param name="Token">The JWT access token for authenticated requests.</param>
    /// <param name="UserId">The authenticated user's unique identifier.</param>
    /// <param name="Name">The user's full name.</param>
    /// <param name="Email">The user's email address.</param>
    /// <param name="Role">The user's role (Admin, User, or Driver).</param>
    /// <param name="ExpiresAt">The UTC timestamp when the token expires.</param>
    public record LoginResponse(
        string Token,
        Guid UserId,
        string Name,
        string Email,
        UserRoles Role,
        DateTime ExpiresAt
    );
}
