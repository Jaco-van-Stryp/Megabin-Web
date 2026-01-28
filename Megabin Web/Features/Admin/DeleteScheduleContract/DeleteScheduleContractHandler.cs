using MediatR;
using Megabin_Web.Shared.Domain.Data;

namespace Megabin_Web.Features.Admin.DeleteScheduleContract
{
    public class DeleteScheduleContractHandler(AppDbContext dbContext)
        : IRequestHandler<DeleteScheduleContractCommand>
    {
        public async Task Handle(
            DeleteScheduleContractCommand request,
            CancellationToken cancellationToken
        )
        {
            var contract = await dbContext.ScheduledContract.FindAsync(
                [request.ContractId],
                cancellationToken
            );

            if (contract == null)
            {
                throw new KeyNotFoundException($"Contract with ID {request.ContractId} not found");
            }

            dbContext.ScheduledContract.Remove(contract);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
