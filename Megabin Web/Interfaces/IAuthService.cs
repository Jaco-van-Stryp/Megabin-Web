using Megabin_Web.Entities;
using Megabin_Web.Enums;

namespace Megabin_Web.Interfaces
{
    /// <summary>
    /// Provides user authentication operations.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Authenticates a user with email and password.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="password">The user's plain text password.</param>
        /// <returns>The authenticated user entity if successful; null if authentication fails.</returns>
        Task<Users?> AuthenticateAsync(string email, string password);

        /// <summary>
        /// Registers a new user account (admin operation).
        /// </summary>
        /// <param name="name">The user's full name.</param>
        /// <param name="email">The user's email address.</param>
        /// <param name="password">The user's plain text password.</param>
        /// <param name="phoneNumber">The user's phone number.</param>
        /// <returns>The created user entity.</returns>
        Task<Users> RegisterUserAsync(
            string name,
            string email,
            string password,
            string phoneNumber
        );
    }
}
