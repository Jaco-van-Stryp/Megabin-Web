using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Megabin_Web.Configuration;
using Megabin_Web.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Megabin_Web.Services
{
    /// <summary>
    /// Implementation of JWT token generation service.
    /// </summary>
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtOptions _jwtOptions;
        private readonly ILogger<JwtTokenService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtTokenService"/> class.
        /// </summary>
        /// <param name="jwtOptions">JWT configuration options.</param>
        /// <param name="logger">Logger for JWT token service operations.</param>
        public JwtTokenService(IOptions<JwtOptions> jwtOptions, ILogger<JwtTokenService> logger)
        {
            _jwtOptions = jwtOptions.Value;
            _logger = logger;
        }

        /// <inheritdoc/>
        public string GenerateToken(Guid userId, string email, string role)
        {
            _logger.LogDebug("Generating JWT token for user {UserId} with role {Role}", userId, role);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            _logger.LogInformation("JWT token generated successfully for user {UserId}", userId);

            return tokenString;
        }
    }
}
