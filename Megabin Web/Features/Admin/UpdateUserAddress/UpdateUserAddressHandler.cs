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

            // Only update fields that are provided (partial update support)
            if (request.TotalBins.HasValue)
            {
                address.TotalBins = request.TotalBins.Value;
            }

            if (request.AddressNotes is not null)
            {
                address.AddressNotes = request.AddressNotes;
            }

            if (request.Status.HasValue)
            {
                address.Status = request.Status.Value;
            }

            if (request.Location is not null)
            {
                address.Long = request.Location.Longitude;
                address.Lat = request.Location.Latitude;
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
