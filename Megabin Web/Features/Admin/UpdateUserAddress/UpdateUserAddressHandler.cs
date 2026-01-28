using MediatR;
using Megabin_Web.Shared.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Megabin_Web.Features.Admin.UpdateUserAddress
{
    public class UpdateUserAddressHandler(AppDbContext dbContext)
        : IRequestHandler<UpdateUserAddressCommand>
    {
        public async Task Handle(
            UpdateUserAddressCommand request,
            CancellationToken cancellationToken
        )
        {
            var address = await dbContext
                .Addresses.FirstOrDefaultAsync(
                    x => x.Id == request.AddressId,
                    cancellationToken
                );

            if (address == null)
            {
                throw new KeyNotFoundException($"Address with ID {request.AddressId} not found");
            }

            address.TotalBins = request.TotalBins;
            address.AddressNotes = request.AddressNotes;
            address.Status = request.Status;
            address.Long = request.Location.Longitude;
            address.Lat = request.Location.Latitude;

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
