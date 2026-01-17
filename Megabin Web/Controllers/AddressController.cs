using Megabin_Web.Data;
using Megabin_Web.DTOs.Addresses;
using Megabin_Web.DTOs.Routing;
using Megabin_Web.Entities;
using Megabin_Web.Enums;
using Megabin_Web.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Megabin_Web.Controllers
{
    [Authorize]
    public class AddressController : BaseController
    {
        private readonly IMapboxService _mapboxService;
        private readonly IAPILimitationService _limitationService;
        private readonly AppDbContext _dbContext;

        public AddressController(
            IMapboxService mapboxService,
            IAPILimitationService limitationService,
            AppDbContext dbContext
        )
        {
            _mapboxService = mapboxService;
            _limitationService = limitationService;
            _dbContext = dbContext;
        }

        [HttpGet("AutoComplete/{address}")]
        public async Task<ActionResult<List<AddressSuggestion>>> AutoCompleteAddress(string address)
        {
            var requestAllowed = await _limitationService.RecordApiCallAsync(
                CurrentUserId,
                APITypes.Mapbox_Autocomplete
            );
            if (!requestAllowed)
                return BadRequest("API rate limit exceeded.");

            string decodedAddress = Uri.UnescapeDataString(address);
            var results = await _mapboxService.AutocompleteAsync(decodedAddress);
            return Ok(results);
        }

        [HttpPost("CreateAddress")]
        public async Task<ActionResult<Location>> CreateAddress(CreateAddress address)
        {
            var user = await _dbContext
                .Users.Include(a => a.Addresss)
                .FirstOrDefaultAsync(x => x.Id == CurrentUserId);

            // Check if the user exists
            if (user == null)
                return NotFound();

            // Check if the address already exists for the user
            if (user.Addresss.Any(a => a.Address == address.Address.Label))
            {
                return Conflict("Address already exists.");
            }

            var newAddress = new Addresses
            {
                Address = address.Address.Label,
                TotalBins = address.TotalBins,
                AddressNotes = address.AddressNotes,
                Lat = address.Address.Location.Latitude,
                Long = address.Address.Location.Longitude,
                UserId = user.Id,
                User = user,
            };

            _dbContext.Addresses.Add(newAddress);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("GetAllAddresses")]
        public async Task<ActionResult<List<GetAddress>>> GetAllAddresses()
        {
            var user = await _dbContext
                .Users.Include(a => a.Addresss)
                .FirstOrDefaultAsync(x => x.Id == CurrentUserId);
            if (user == null)
                return NotFound();
            var addresses = new List<GetAddress>();
            foreach (var address in user.Addresss)
            {
                addresses.Add(
                    new GetAddress
                    {
                        Id = address.Id,
                        Address = address.Address,
                        AddressNotes = address.AddressNotes ?? string.Empty,
                        TotalBins = address.TotalBins,
                        AddressStatus = address.Status,
                    }
                );
            }
            ;
            return Ok(addresses);
        }

        //TODO - Deleting an address should also delete all contracts associated with that address and thus requires careful handling. The below is just for testing.
        [HttpDelete("DeleteAddress/{id}")]
        public async Task<ActionResult> DeleteAddress(Guid id)
        {
            var user = await _dbContext
                .Users.Include(a => a.Addresss)
                .FirstOrDefaultAsync(x => x.Id == CurrentUserId);
            if (user == null)
                return NotFound();
            var address = user.Addresss.FirstOrDefault(a => a.Id == id);
            if (address == null)
                return NotFound();
            _dbContext.Addresses.Remove(address);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
