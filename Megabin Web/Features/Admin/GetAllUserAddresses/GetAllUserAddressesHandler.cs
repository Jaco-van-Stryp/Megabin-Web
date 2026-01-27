using MediatR;
using Megabin_Web.Shared.Domain.Data;
using Megabin_Web.Shared.DTOs.Addresses;
using Megabin_Web.Shared.DTOs.Routing;
using Microsoft.EntityFrameworkCore;

namespace Megabin_Web.Features.Admin.GetAllUserAddresses
{
    public class GetAllUserAddressesHandler(AppDbContext dbContext)
        : IRequestHandler<GetAllUserAddressesQuery, List<GetAddress>>
    {
        public async Task<List<GetAddress>> Handle(
            GetAllUserAddressesQuery request,
            CancellationToken cancellationToken
        )
        {
            var user = await dbContext
                .Users.Include(a => a.Addresss)
                .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {request.UserId} not found");
            }

            var addressList = user.Addresss
                .Select(address => new GetAddress
                {
                    Id = address.Id,
                    Address = address.Address,
                    AddressNotes = address.AddressNotes ?? string.Empty,
                    TotalBins = address.TotalBins,
                    AddressStatus = address.Status,
                    Location = new Location(address.Long, address.Lat),
                })
                .ToList();

            return addressList;
        }
    }
}
