using MediatR;
using Megabin_Web.Shared.DTOs.Addresses;

namespace Megabin_Web.Features.Address.GetAllAddresses
{
    public record GetAllAddressesQuery() : IRequest<List<GetAddress>> { }
}
