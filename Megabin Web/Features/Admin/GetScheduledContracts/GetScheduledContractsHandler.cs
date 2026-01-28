using MediatR;
using Megabin_Web.Shared.Domain.Data;
using Megabin_Web.Shared.DTOs.ScheduleContracts;
using Microsoft.EntityFrameworkCore;

namespace Megabin_Web.Features.Admin.GetScheduledContracts
{
    public class GetScheduledContractsHandler(AppDbContext dbContext)
        : IRequestHandler<GetScheduledContractsQuery, List<GetScheduleContract>>
    {
        public async Task<List<GetScheduleContract>> Handle(
            GetScheduledContractsQuery request,
            CancellationToken cancellationToken
        )
        {
            var contracts = await dbContext
                .ScheduledContract.Where(x => x.AddressesId == request.AddressId)
                .Select(c => new GetScheduleContract
                {
                    Id = c.Id,
                    Frequency = c.Frequency,
                    DayOfWeek = c.DayOfWeek,
                    StartingDate = c.StartingDate,
                    LastCollected = c.LastCollected,
                    Active = c.Active,
                    ApprovedExternally = c.ApprovedExternally,
                    AddressesId = c.AddressesId,
                })
                .ToListAsync(cancellationToken);

            if (contracts == null || contracts.Count == 0)
            {
                throw new KeyNotFoundException(
                    $"No schedule contracts found for address ID {request.AddressId}"
                );
            }

            return contracts;
        }
    }
}
