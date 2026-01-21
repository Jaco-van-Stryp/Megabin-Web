using System.Security.Claims;

namespace Megabin_Web.Shared.Infrastructure.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="ClaimsPrincipal"/> to extract user information from claims.
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Gets the current authenticated user's ID from claims.
        /// </summary>
        /// <param name="principal">The claims principal (typically from HttpContext.User).</param>
        /// <returns>The user ID as a Guid, or null if not found or invalid.</returns>
        public static Guid? GetUserId(this ClaimsPrincipal principal)
        {
            if (principal == null)
                return null;

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return null;

            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        /// <summary>
        /// Gets the current authenticated user's ID from claims, throwing an exception if not found.
        /// </summary>
        /// <param name="principal">The claims principal (typically from HttpContext.User).</param>
        /// <returns>The user ID as a Guid.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when user ID claim is not found or invalid.</exception>
        public static Guid GetUserIdOrThrow(this ClaimsPrincipal principal)
        {
            var userId = principal.GetUserId();

            if (!userId.HasValue)
                throw new UnauthorizedAccessException("User ID not found in claims.");

            return userId.Value;
        }

        /// <summary>
        /// Gets the current authenticated user's role from claims.
        /// </summary>
        /// <param name="principal">The claims principal (typically from HttpContext.User).</param>
        /// <returns>The user role, or null if not found.</returns>
        public static string? GetUserRole(this ClaimsPrincipal principal)
        {
            return principal?.FindFirst(ClaimTypes.Role)?.Value;
        }

        /// <summary>
        /// Gets the current authenticated user's email from claims.
        /// </summary>
        /// <param name="principal">The claims principal (typically from HttpContext.User).</param>
        /// <returns>The user email, or null if not found.</returns>
        public static string? GetUserEmail(this ClaimsPrincipal principal)
        {
            return principal?.FindFirst(ClaimTypes.Email)?.Value;
        }

        /// <summary>
        /// Checks if the current user is authenticated.
        /// </summary>
        /// <param name="principal">The claims principal (typically from HttpContext.User).</param>
        /// <returns>True if authenticated, false otherwise.</returns>
        public static bool IsAuthenticated(this ClaimsPrincipal principal)
        {
            return principal?.Identity?.IsAuthenticated ?? false;
        }

        /// <summary>
        /// Checks if the current user has a specific role.
        /// </summary>
        /// <param name="principal">The claims principal (typically from HttpContext.User).</param>
        /// <param name="role">The role to check for.</param>
        /// <returns>True if the user has the role, false otherwise.</returns>
        public static bool HasRole(this ClaimsPrincipal principal, string role)
        {
            return principal?.IsInRole(role) ?? false;
        }
    }
}
