using Megabin_Web.Interfaces;

namespace Megabin_Web.Services
{
    /// <summary>
    /// Implementation of password hashing and verification using BCrypt.
    /// </summary>
    public class PasswordService : IPasswordService
    {
        private readonly ILogger<PasswordService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordService"/> class.
        /// </summary>
        /// <param name="logger">Logger for password service operations.</param>
        public PasswordService(ILogger<PasswordService> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public string HashPassword(string password)
        {
            _logger.LogDebug("Hashing password with BCrypt work factor 12");
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        /// <inheritdoc/>
        public bool VerifyPassword(string password, string passwordHash)
        {
            try
            {
                var isValid = BCrypt.Net.BCrypt.Verify(password, passwordHash);
                _logger.LogDebug("Password verification result: {IsValid}", isValid);
                return isValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying password hash");
                return false;
            }
        }
    }
}
