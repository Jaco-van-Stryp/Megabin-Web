using MediatR;
using Megabin_Web.Features.Address.CreateAddress;
using Megabin_Web.Shared.Domain.Data;
using Megabin_Web.Shared.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Megabin_Web.Features.Admin.AddUserAddress
{
    public class AddUserAddressHandler(AppDbContext dbContext)
        : IRequestHandler<AddUserAddressCommand, CreateAddressResponseDto>
    {
        public async Task<CreateAddressResponseDto> Handle(
            AddUserAddressCommand request,
            CancellationToken cancellationToken
        )
        {
            var user = await dbContext
                .Users.Include(x => x.Addresss)
                .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {request.UserId} not found");
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
                Status = request.Status,
            };

            user.Addresss.Add(newAddress);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new CreateAddressResponseDto { AddressId = newAddress.Id };
        }
    }
}
