using MediatR;
using Megabin_Web.Shared.Domain.Data;
using Megabin_Web.Shared.DTOs.Addresses;
using Megabin_Web.Shared.DTOs.Routing;
using Megabin_Web.Shared.Infrastructure.CurrentUserService;
using Microsoft.EntityFrameworkCore;

namespace Megabin_Web.Features.Address.GetAllAddresses
{
    public class GetAllAddressesHandler(
        AppDbContext _dbContext,
        ICurrentUserService _currentUserService
    ) : IRequestHandler<GetAllAddressesQuery, List<GetAddress>>
    {
        public async Task<List<GetAddress>> Handle(
            GetAllAddressesQuery request,
            CancellationToken cancellationToken
        )
        {
            var addresses = await _dbContext
                .Users.Include(x => x.Addresss)
                .FirstOrDefaultAsync(x => x.Id == _currentUserService.GetUserId());

            if (addresses == null)
            {
                return new List<GetAddress>();
            }
            List<GetAddress> result = addresses
                .Addresss.Select(address => new GetAddress
                {
                    Id = address.Id,
                    Address = address.Address,
                    AddressNotes = address.AddressNotes ?? "",
                    TotalBins = address.TotalBins,
                    AddressStatus = address.Status,
                    Location = new Location(address.Long, address.Lat),
                })
                .ToList();
            return result;
        }
    }
}
