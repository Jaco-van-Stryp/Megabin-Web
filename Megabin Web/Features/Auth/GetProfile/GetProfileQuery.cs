using MediatR;

namespace Megabin_Web.Features.Auth.GetProfile;

public record GetProfileQuery(Guid UserId, string Email, string Role) : IRequest<ProfileResponse>;

public record ProfileResponse(Guid UserId, string Email, string Role);
