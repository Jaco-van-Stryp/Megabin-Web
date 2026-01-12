using Megabin_Web.DTOs.Routing;
using Megabin_Web.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Megabin_Web.Controllers
{
    public class RoutesController(IOpenRouteService openRouteService) : BaseController
    {
        [HttpGet("AutoComplete/{address}")]
        public async Task<ActionResult<List<AddressSuggestion>>> AutoCompleteAddress(string address)
        {
            string decodedAddress = Uri.UnescapeDataString(address);
            var results = await openRouteService.AutocompleteAddressAsync(address);
            return Ok(results);
        }
    }
}
