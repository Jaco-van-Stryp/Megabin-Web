using Megabin_Web.Data;
using Megabin_Web.Entities;
using Megabin_Web.Enums;
using Megabin_Web.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Megabin_Web.Services
{
    /// <summary>
    /// Implementation of user authentication service.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IPasswordService _passwordService;
        private readonly ILogger<AuthService> _logger;
        private readonly IWhatsAppService _whatsAppService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthService"/> class.
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <param name="passwordService">Password hashing and verification service.</param>
        /// <param name="logger">Logger for authentication operations.</param>
        /// <param name="whatsAppService">WhatsApp messaging service.</param>
        public AuthService(
            AppDbContext context,
            IPasswordService passwordService,
            ILogger<AuthService> logger,
            IWhatsAppService whatsAppService
        )
        {
            _context = context;
            _passwordService = passwordService;
            _logger = logger;
            _whatsAppService = whatsAppService;
        }

        /// <inheritdoc/>
        public async Task<Users?> AuthenticateAsync(string email, string password)
        {
            _logger.LogDebug("Attempting to authenticate user with email {Email}", email);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                _logger.LogWarning(
                    "Authentication failed: User with email {Email} not found",
                    email
                );
                return null;
            }

            if (!_passwordService.VerifyPassword(password, user.PasswordHash))
            {
                _logger.LogWarning(
                    "Authentication failed: Invalid password for user {Email}",
                    email
                );
                return null;
            }

            _logger.LogInformation("User {Email} authenticated successfully", email);
            return user;
        }

        /// <inheritdoc/>
        public async Task<Users> RegisterUserAsync(
            string name,
            string email,
            string password,
            string phoneNumber
        )
        {
            _logger.LogDebug(
                "Registering new user with email {Email} and phone number {PhoneNumber}",
                email,
                phoneNumber
            );

            // Check if user already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (existingUser != null)
            {
                _logger.LogError(
                    "Registration failed: User with email {Email} already exists",
                    email
                );
                throw new InvalidOperationException($"User with email {email} already exists");
            }

            // Hash the password
            var passwordHash = _passwordService.HashPassword(password);

            // Create new user
            var user = new Users
            {
                Id = Guid.NewGuid(),
                Name = name,
                Email = email,
                PasswordHash = passwordHash,
                TotalBins = 0,
                PhoneNumber = phoneNumber,
                Role = UserRoles.Customer,
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            await _whatsAppService.SendTextMessageAsync(
                user.Id,
                $"Welcome to Megabin, {name}! Your account has been successfully created."
            );
            _logger.LogInformation(
                "User {Email} registered successfully with role {Role}",
                email,
                UserRoles.Customer
            );
            return user;
        }
    }
}
