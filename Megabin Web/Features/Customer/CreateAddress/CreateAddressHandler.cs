using MediatR;
using Megabin_Web.Shared.Domain.Data;
using Megabin_Web.Shared.Domain.Entities;
using Megabin_Web.Shared.Domain.Enums;
using Megabin_Web.Shared.Infrastructure.CurrentUserService;
using Microsoft.EntityFrameworkCore;

namespace Megabin_Web.Features.Customer.CreateAddress
{
    public class CustomerCreateAddressHandler(
        AppDbContext dbContext,
        ICurrentUserService currentUserService
    ) : IRequestHandler<CustomerCreateAddressCommand, CustomerCreateAddressResponseDto>
    {
        public async Task<CustomerCreateAddressResponseDto> Handle(
            CustomerCreateAddressCommand request,
            CancellationToken cancellationToken
        )
        {
            var userId = currentUserService.GetUserId();
            var user = await dbContext
                .Users.Include(x => x.Addresss)
                .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

            if (user == null)
            {
                throw new KeyNotFoundException($"User not found");
            }

            var newAddress = new Addresses
            {
                Address = request.Address.Label,
                Lat = request.Address.Location.Latitude,
                Long = request.Address.Location.Longitude,
                TotalBins = request.TotalBins,
                AddressNotes = request.AddressNotes,
                UserId = user.Id,
                User = user,
                Status = AddressStatus.BinRequested,
            };

            user.Addresss.Add(newAddress);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new CustomerCreateAddressResponseDto { AddressId = newAddress.Id };
        }
    }
}
