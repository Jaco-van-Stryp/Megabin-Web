using MediatR;
using Megabin_Web.Shared.Domain.Data;
using Megabin_Web.Shared.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Megabin_Web.Features.Admin.AddScheduleContract
{
    public class AddScheduleContractHandler(AppDbContext dbContext)
        : IRequestHandler<AddScheduleContractCommand>
    {
        public async Task Handle(
            AddScheduleContractCommand request,
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

            dbContext.ScheduledContract.Add(
                new ScheduleContract
                {
                    AddressesId = request.AddressId,
                    Addresses = address,
                    DayOfWeek = request.DayOfWeek,
                    Frequency = request.Frequency,
                    Active = true,
                    ApprovedExternally = true,
                }
            );

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
