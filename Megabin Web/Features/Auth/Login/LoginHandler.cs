using MediatR;
using Megabin_Web.Shared.Domain.Data;
using Megabin_Web.Shared.DTOs.Auth;
using Megabin_Web.Shared.Infrastructure.JWTTokenService;
using Megabin_Web.Shared.Infrastructure.PasswordService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Megabin_Web.Features.Auth.Login
{
    public class LoginHandler(
        AppDbContext dbContext,
        IPasswordService passwordService,
        IJwtTokenService jwtTokenService,
        IOptions<JwtOptions> jwtOptions,
        ILogger<LoginHandler> logger
    ) : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly JwtOptions _jwtOptions = jwtOptions.Value;

        public async Task<LoginResponse> Handle(
            LoginCommand request,
            CancellationToken cancellationToken
        )
        {
            logger.LogDebug("Attempting to authenticate user with email {Email}", request.Email);

            // Find user by email
            var user = await dbContext.Users.FirstOrDefaultAsync(
                u => u.Email == request.Email,
                cancellationToken
            );

            if (user == null)
            {
                logger.LogWarning(
                    "Authentication failed: User with email {Email} not found",
                    request.Email
                );
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            // Verify password
            if (!passwordService.VerifyPassword(request.Password, user.PasswordHash))
            {
                logger.LogWarning(
                    "Authentication failed: Invalid password for user {Email}",
                    request.Email
                );
                throw new UnauthorizedAccessException("Invalid email or password");
            }

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

            logger.LogInformation("User {Email} logged in successfully", user.Email);
            return response;
        }
    }
}
