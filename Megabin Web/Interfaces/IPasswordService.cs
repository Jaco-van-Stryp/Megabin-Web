namespace Megabin_Web.Interfaces
{
    /// <summary>
    /// Provides password hashing and verification using BCrypt.
    /// </summary>
    public interface IPasswordService
    {
        /// <summary>
        /// Hashes a plain text password using BCrypt with work factor 12.
        /// </summary>
        /// <param name="password">The plain text password to hash.</param>
        /// <returns>The BCrypt hashed password string.</returns>
        string HashPassword(string password);

        /// <summary>
        /// Verifies a plain text password against a BCrypt hash.
        /// </summary>
        /// <param name="password">The plain text password to verify.</param>
        /// <param name="passwordHash">The BCrypt hash to verify against.</param>
        /// <returns>True if the password matches the hash; otherwise, false.</returns>
        bool VerifyPassword(string password, string passwordHash);
    }
}
