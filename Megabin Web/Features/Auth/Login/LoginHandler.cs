using MediatR;
using Megabin_Web.Shared.DTOs.Auth;
using Megabin_Web.Shared.Infrastructure.AuthService;
using Megabin_Web.Shared.Infrastructure.JWTTokenService;
using Microsoft.Extensions.Options;

namespace Megabin_Web.Features.Auth.Login
{
    public class LoginHandler(
        IAuthService authService,
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
            var user = await authService.AuthenticateAsync(request.Email, request.Password);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

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
