using MediatR;

namespace Megabin_Web.Features.Auth.GetProfile
{
    public class GetProfileHandler : IRequestHandler<GetProfileQuery, ProfileResponse>
    {
        public Task<ProfileResponse> Handle(
            GetProfileQuery request,
            CancellationToken cancellationToken
        )
        {
            // Simply return the profile information from the JWT claims
            var profile = new ProfileResponse(request.UserId, request.Email, request.Role);
            return Task.FromResult(profile);
        }
    }
}
