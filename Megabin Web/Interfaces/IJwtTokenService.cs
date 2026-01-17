using Megabin_Web.Enums;

namespace Megabin_Web.Interfaces
{
    /// <summary>
    /// Provides JWT token generation for authenticated users.
    /// </summary>
    public interface IJwtTokenService
    {
        /// <summary>
        /// Generates a JWT access token for the specified user.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <param name="email">The user's email address.</param>
        /// <param name="role">The user's role (Admin, User, or Driver).</param>
        /// <returns>A signed JWT token string.</returns>
        string GenerateToken(Guid userId, string email, UserRoles role);
    }
}
