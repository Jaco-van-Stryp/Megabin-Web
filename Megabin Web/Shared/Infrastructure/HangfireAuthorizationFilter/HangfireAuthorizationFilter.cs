using Hangfire.Dashboard;

namespace Megabin_Web.Shared.Infrastructure.HangfireAuthorizationFilter
{
    /// <summary>
    /// Authorization filter for Hangfire dashboard.
    /// In development, allows all access. In production, should be secured with proper authorization.
    /// </summary>
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            // TODO: In production, implement proper authorization
            // For now, allow all access in development
            return true;
        }
    }
}
