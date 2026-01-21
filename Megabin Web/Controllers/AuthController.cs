using System.Security.Claims;
using Megabin_Web.Shared.DTOs.Auth;
using Megabin_Web.Shared.Infrastructure.AuthService;
using Megabin_Web.Shared.Infrastructure.JWTTokenService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Megabin_Web.Controllers
{
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly JwtOptions _jwtOptions;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService,
            IJwtTokenService jwtTokenService,
            IOptions<JwtOptions> jwtOptions,
            ILogger<AuthController> logger
        )
        {
            _authService = authService;
            _jwtTokenService = jwtTokenService;
            _jwtOptions = jwtOptions.Value;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(
                    new ErrorResponse("Invalid input data", StatusCodes.Status400BadRequest)
                );
            }

            var user = await _authService.AuthenticateAsync(request.Email, request.Password);

            if (user == null)
            {
                return Unauthorized(
                    new ErrorResponse(
                        "Invalid email or password",
                        StatusCodes.Status401Unauthorized
                    )
                );
            }

            var token = _jwtTokenService.GenerateToken(user.Id, user.Email, user.Role);
            var expiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes);

            var response = new LoginResponse(
                token,
                user.Id,
                user.Name,
                user.Email,
                user.Role,
                expiresAt
            );

            _logger.LogInformation("User {Email} logged in successfully", user.Email);
            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<ActionResult<LoginResponse>> Register(RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(
                    new ErrorResponse("Invalid input data", StatusCodes.Status400BadRequest)
                );
            }

            try
            {
                var user = await _authService.RegisterUserAsync(
                    request.Name,
                    request.Email,
                    request.Password,
                    request.PhoneNumber
                );

                var token = _jwtTokenService.GenerateToken(user.Id, user.Email, user.Role);
                var expiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes);

                var response = new LoginResponse(
                    token,
                    user.Id,
                    user.Name,
                    user.Email,
                    user.Role,
                    expiresAt
                );

                _logger.LogInformation(
                    "New user {Email} registered with role {Role}",
                    user.Email,
                    user.Role
                );
                return CreatedAtAction(nameof(GetProfile), new { }, response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErrorResponse(ex.Message, StatusCodes.Status400BadRequest));
            }
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userId == null || email == null || role == null)
            {
                return Unauthorized(
                    new ErrorResponse("Invalid token claims", StatusCodes.Status401Unauthorized)
                );
            }

            var profile = new
            {
                UserId = Guid.Parse(userId),
                Email = email,
                Role = role,
            };

            return Ok(profile);
        }
    }
}
