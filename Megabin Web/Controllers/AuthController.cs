using System.Security.Claims;
using Megabin_Web.Configuration;
using Megabin_Web.DTOs.Auth;
using Megabin_Web.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Megabin_Web.Controllers
{
    /// <summary>
    /// Handles user authentication and registration operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly JwtOptions _jwtOptions;
        private readonly ILogger<AuthController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="authService">Authentication service.</param>
        /// <param name="jwtTokenService">JWT token generation service.</param>
        /// <param name="jwtOptions">JWT configuration options.</param>
        /// <param name="logger">Logger for controller operations.</param>
        public AuthController(
            IAuthService authService,
            IJwtTokenService jwtTokenService,
            IOptions<JwtOptions> jwtOptions,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _jwtTokenService = jwtTokenService;
            _jwtOptions = jwtOptions.Value;
            _logger = logger;
        }

        /// <summary>
        /// Authenticates a user and returns a JWT access token.
        /// </summary>
        /// <param name="request">Login credentials.</param>
        /// <returns>JWT token and user information.</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse(
                    "Invalid input data",
                    StatusCodes.Status400BadRequest
                ));
            }

            var user = await _authService.AuthenticateAsync(request.Email, request.Password);

            if (user == null)
            {
                return Unauthorized(new ErrorResponse(
                    "Invalid email or password",
                    StatusCodes.Status401Unauthorized
                ));
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

        /// <summary>
        /// Registers a new user account. Restricted to Admin role.
        /// </summary>
        /// <param name="request">User registration details.</param>
        /// <returns>JWT token and user information for the newly created user.</returns>
        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse(
                    "Invalid input data",
                    StatusCodes.Status400BadRequest
                ));
            }

            try
            {
                var user = await _authService.RegisterUserAsync(
                    request.Name,
                    request.Email,
                    request.Password,
                    request.Role
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

                _logger.LogInformation("New user {Email} registered with role {Role}", user.Email, user.Role);
                return CreatedAtAction(nameof(GetProfile), new { }, response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErrorResponse(ex.Message, StatusCodes.Status400BadRequest));
            }
        }

        /// <summary>
        /// Returns the authenticated user's profile information.
        /// </summary>
        /// <returns>User profile information from JWT claims.</returns>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userId == null || email == null || role == null)
            {
                return Unauthorized(new ErrorResponse(
                    "Invalid token claims",
                    StatusCodes.Status401Unauthorized
                ));
            }

            var profile = new
            {
                UserId = Guid.Parse(userId),
                Email = email,
                Role = role
            };

            return Ok(profile);
        }
    }
}
