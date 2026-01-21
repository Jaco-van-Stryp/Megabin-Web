using MediatR;
using Megabin_Web.Shared.Domain.Data;
using Megabin_Web.Shared.Domain.Entities;
using Megabin_Web.Shared.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Megabin_Web.Features.Address.CreateAddress
{
    public class CreateAddressHandler(
        AppDbContext _dbContext,
        IHttpContextAccessor _httpContextAccessor
    ) : IRequestHandler<CreateAddressCommand, CreateAddressResponseDto>
    {
        public async Task<CreateAddressResponseDto> Handle(
            CreateAddressCommand request,
            CancellationToken cancellationToken
        )
        {
            var user = await _dbContext
                .Users.Include(a => a.Addresss)
                .FirstOrDefaultAsync(x =>
                    x.Id == _httpContextAccessor.HttpContext!.User.GetUserId()
                );

            // Check if the user exists
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }
            if (user.Addresss.Any(a => a.Address == request.Address.Label))
            {
                throw new ArgumentException("Address already exists for this user");
            }

            var newAddress = new Addresses
            {
                Address = request.Address.Label,
                TotalBins = request.TotalBins,
                AddressNotes = request.AddressNotes,
                Lat = request.Address.Location.Latitude,
                Long = request.Address.Location.Longitude,
                UserId = user.Id,
                User = user,
            };

            _dbContext.Addresses.Add(newAddress);
            await _dbContext.SaveChangesAsync();
            return new CreateAddressResponseDto { AddressId = newAddress.Id };
        }
    }
}
