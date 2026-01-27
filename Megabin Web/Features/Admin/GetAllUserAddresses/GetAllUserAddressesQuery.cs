using MediatR;
using Megabin_Web.Shared.DTOs.Addresses;

namespace Megabin_Web.Features.Admin.GetAllUserAddresses;

public record GetAllUserAddressesQuery(Guid UserId) : IRequest<List<GetAddress>>;
