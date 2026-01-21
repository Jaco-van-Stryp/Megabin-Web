using Megabin_Web.Shared.Infrastructure.Extensions;

namespace Megabin_Web.Shared.Infrastructure.CurrentUserService
{
    public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
    {
        public Guid GetUserId()
        {
            return httpContextAccessor.HttpContext!.User.GetUserIdOrThrow();
        }
    }
}
