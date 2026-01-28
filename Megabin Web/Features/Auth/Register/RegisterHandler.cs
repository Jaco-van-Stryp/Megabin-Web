using MediatR;
using Megabin_Web.Shared.Domain.Data;
using Megabin_Web.Shared.Domain.Entities;
using Megabin_Web.Shared.Domain.Enums;
using Megabin_Web.Shared.DTOs.Auth;
using Megabin_Web.Shared.Infrastructure.JWTTokenService;
using Megabin_Web.Shared.Infrastructure.PasswordService;
using Megabin_Web.Shared.Infrastructure.WhatsAppService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Megabin_Web.Features.Auth.Register
{
    public class RegisterHandler(
        AppDbContext dbContext,
        IPasswordService passwordService,
        IJwtTokenService jwtTokenService,
        IWhatsAppService whatsAppService,
        IOptions<JwtOptions> jwtOptions,
        ILogger<RegisterHandler> logger
    ) : IRequestHandler<RegisterCommand, LoginResponse>
    {
        private readonly JwtOptions _jwtOptions = jwtOptions.Value;

        public async Task<LoginResponse> Handle(
            RegisterCommand request,
            CancellationToken cancellationToken
        )
        {
            logger.LogDebug("Registering new user with email {Email}", request.Email);

            // Check if user already exists
            var existingUser = await dbContext.Users.FirstOrDefaultAsync(
                u => u.Email == request.Email,
                cancellationToken
            );

            if (existingUser != null)
            {
                logger.LogError(
                    "Registration failed: User with email {Email} already exists",
                    request.Email
                );
                throw new InvalidOperationException(
                    $"User with email {request.Email} already exists"
                );
            }

            // Hash the password
            var passwordHash = passwordService.HashPassword(request.Password);

            // Create new user
            var user = new Users
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email,
                PasswordHash = passwordHash,
                PhoneNumber = request.PhoneNumber,
                Role = UserRoles.Customer,
            };

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync(cancellationToken);

            // Send welcome message
            await whatsAppService.SendTextMessageAsync(
                request.PhoneNumber,
                $"Welcome to Megabin, {request.Name}! Your account has been successfully created."
            );

            // Generate token
            var token = jwtTokenService.GenerateToken(user.Id, user.Email, user.Role);
            var expiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes);

            var response = new LoginResponse(
                token,
                user.Id,
                user.Name,
                user.Email,
                user.Role,
                expiresAt
            );

            logger.LogInformation(
                "New user {Email} registered with role {Role}",
                user.Email,
                user.Role
            );

            return response;
        }
    }
}
