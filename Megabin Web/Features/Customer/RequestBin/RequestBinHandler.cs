using MediatR;
using Megabin_Web.Shared.Domain.Data;
using Megabin_Web.Shared.Domain.Enums;
using Megabin_Web.Shared.Infrastructure.CurrentUserService;
using Microsoft.EntityFrameworkCore;

namespace Megabin_Web.Features.Customer.RequestBin
{
    public class RequestBinHandler(
        AppDbContext dbContext,
        ICurrentUserService currentUserService
    ) : IRequestHandler<RequestBinCommand>
    {
        public async Task Handle(RequestBinCommand request, CancellationToken cancellationToken)
        {
            var userId = currentUserService.GetUserId();

            var address = await dbContext
                .Addresses.FirstOrDefaultAsync(
                    x => x.Id == request.AddressId && x.UserId == userId,
                    cancellationToken
                );

            if (address == null)
            {
                throw new KeyNotFoundException(
                    $"Address with ID {request.AddressId} not found or does not belong to you"
                );
            }

            if (address.Status != AddressStatus.PendingAddressCompletion)
            {
                throw new InvalidOperationException(
                    $"Cannot request bin for address with status {address.Status}. Bin can only be requested when status is PendingAddressCompletion."
                );
            }

            address.Status = AddressStatus.BinRequested;
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
