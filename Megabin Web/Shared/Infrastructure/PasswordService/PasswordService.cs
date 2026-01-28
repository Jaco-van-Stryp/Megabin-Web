namespace Megabin_Web.Shared.Infrastructure.PasswordService
{
    /// <summary>
    /// Pure utility service for password hashing and verification.
    /// Contains no database access or business logic.
    /// </summary>
    public class PasswordService : IPasswordService
    {
        private readonly ILogger<PasswordService> _logger;

        public PasswordService(ILogger<PasswordService> logger)
        {
            _logger = logger;
        }

        public string HashPassword(string password)
        {
            _logger.LogDebug("Hashing password with BCrypt work factor 12");
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

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
