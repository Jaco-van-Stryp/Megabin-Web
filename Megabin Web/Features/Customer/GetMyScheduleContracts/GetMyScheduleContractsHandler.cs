using MediatR;
using Megabin_Web.Shared.Domain.Data;
using Megabin_Web.Shared.Infrastructure.CurrentUserService;
using Microsoft.EntityFrameworkCore;

namespace Megabin_Web.Features.Customer.GetMyScheduleContracts
{
    public class GetMyScheduleContractsHandler(
        AppDbContext dbContext,
        ICurrentUserService currentUserService
    ) : IRequestHandler<GetMyScheduleContractsQuery, List<CustomerScheduleContractDto>>
    {
        public async Task<List<CustomerScheduleContractDto>> Handle(
            GetMyScheduleContractsQuery request,
            CancellationToken cancellationToken
        )
        {
            var userId = currentUserService.GetUserId();

            var contracts = await dbContext
                .ScheduledContract.Include(sc => sc.Addresses)
                .Where(sc => sc.Addresses.UserId == userId)
                .Select(sc => new CustomerScheduleContractDto
                {
                    Id = sc.Id,
                    AddressId = sc.AddressesId,
                    Address = sc.Addresses.Address,
                    Frequency = sc.Frequency,
                    DayOfWeek = sc.DayOfWeek,
                    StartingDate = sc.StartingDate,
                    Active = sc.Active,
                    ApprovedExternally = sc.ApprovedExternally,
                })
                .ToListAsync(cancellationToken);

            return contracts;
        }
    }
}
