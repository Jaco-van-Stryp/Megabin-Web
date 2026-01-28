using MediatR;
using Megabin_Web.Shared.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Megabin_Web.Features.Address.DeleteAddress
{
    public class DeleteAddressHandler(AppDbContext _dbContext)
        : IRequestHandler<DeleteAddressCommand, IResult>
    {
        public async Task<IResult> Handle(
            DeleteAddressCommand request,
            CancellationToken cancellationToken
        )
        {
            var address = await _dbContext.Addresses.FirstOrDefaultAsync(
                x => x.Id == request.id,
                cancellationToken
            );
            if (address is null)
            {
                return Results.NotFound();
            }
            _dbContext.Addresses.Remove(address);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Results.Ok();
        }
    }
}
