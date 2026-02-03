using MediatR;
using Megabin_Web.Shared.Domain.Data;
using Megabin_Web.Shared.Domain.Entities;
using Megabin_Web.Shared.Infrastructure.CurrentUserService;
using Microsoft.EntityFrameworkCore;

namespace Megabin_Web.Features.Customer.RequestScheduleContract
{
    public class RequestScheduleContractHandler(
        AppDbContext dbContext,
        ICurrentUserService currentUserService
    ) : IRequestHandler<RequestScheduleContractCommand, RequestScheduleContractResponseDto>
    {
        public async Task<RequestScheduleContractResponseDto> Handle(
            RequestScheduleContractCommand request,
            CancellationToken cancellationToken
        )
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

            var newContract = new ScheduleContract
            {
                AddressesId = request.AddressId,
                Addresses = address,
                DayOfWeek = request.DayOfWeek,
                Frequency = request.Frequency,
                Active = true,
                ApprovedExternally = false, // Customer requests, admin approves
            };

            dbContext.ScheduledContract.Add(newContract);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new RequestScheduleContractResponseDto { ContractId = newContract.Id };
        }
    }
}
