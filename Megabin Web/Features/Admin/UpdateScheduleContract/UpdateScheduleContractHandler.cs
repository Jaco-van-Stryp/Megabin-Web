using MediatR;
using Megabin_Web.Shared.Domain.Data;

namespace Megabin_Web.Features.Admin.UpdateScheduleContract
{
    public class UpdateScheduleContractHandler(AppDbContext dbContext)
        : IRequestHandler<UpdateScheduleContractCommand>
    {
        public async Task Handle(
            UpdateScheduleContractCommand request,
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

            contract.Frequency = request.Frequency;
            contract.DayOfWeek = request.DayOfWeek;
            contract.Active = request.Active;
            contract.ApprovedExternally = request.ApprovedExternally;

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
