using MediatR;
using Megabin_Web.Shared.DTOs.Auth;
using Megabin_Web.Shared.Infrastructure.AuthService;
using Megabin_Web.Shared.Infrastructure.JWTTokenService;
using Microsoft.Extensions.Options;

namespace Megabin_Web.Features.Auth.Register
{
    public class RegisterHandler(
        IAuthService authService,
        IJwtTokenService jwtTokenService,
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
            var user = await authService.RegisterUserAsync(
                request.Name,
                request.Email,
                request.Password,
                request.PhoneNumber
            );

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
