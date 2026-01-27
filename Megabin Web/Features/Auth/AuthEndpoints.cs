using Megabin_Web.Features.Auth.GetProfile;
using Megabin_Web.Features.Auth.Login;
using Megabin_Web.Features.Auth.Register;

namespace Megabin_Web.Features.Auth
{
    public static class AuthEndpoints
    {
        public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/Auth").WithTags("Auth");

            group.MapLoginEndpoint();
            group.MapRegisterEndpoint();
            group.MapGetProfileEndpoint();

            return app;
        }
    }
}
