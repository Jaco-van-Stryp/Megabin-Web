using Megabin_Web.Shared.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Megabin_Web.Controllers
{
    /// <summary>
    /// Base controller for all API controllers with authentication helper properties.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BaseController : ControllerBase
    {
        /// <summary>
        /// Gets the current authenticated user's ID from JWT claims.
        /// </summary>
        protected Guid? CurrentUserId => User.GetUserId();

        /// <summary>
        /// Gets the current authenticated user's role from JWT claims.
        /// </summary>
        protected string? CurrentUserRole => User.GetUserRole();

        /// <summary>
        /// Gets a value indicating whether the current request is authenticated.
        /// </summary>
        protected bool IsAuthenticated => User.IsAuthenticated();
    }
}
