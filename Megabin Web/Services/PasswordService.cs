using Megabin_Web.Data;
using Megabin_Web.Interfaces;

namespace Megabin_Web.Services
{
    public class PasswordService : IPasswordService
    {
        private readonly ILogger<PasswordService> _logger;
        private readonly AppDbContext _dbContext;

        public PasswordService(ILogger<PasswordService> logger, AppDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public string HashPassword(string password)
        {
            _logger.LogDebug("Hashing password with BCrypt work factor 12");
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        public async Task ResetPassword(Guid UserId, string NewPassword)
        {
            var user = await _dbContext.Users.FindAsync(UserId);
            if (user == null)
                throw new ArgumentException("User not found");
            user.PasswordHash = HashPassword(NewPassword);
            await _dbContext.SaveChangesAsync();
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
