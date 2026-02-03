using MediatR;

namespace Megabin_Web.Features.Customer.UpdateProfile;

public record UpdateProfileCommand(string Name, string PhoneNumber) : IRequest;
