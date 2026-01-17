using Megabin_Web.DTOs.Routing;

namespace Megabin_Web.DTOs.Addresses
{
    public class CreateAddress
    {
        public required Guid UserId { get; set; }
        public required AddressSuggestion Address { get; set; }
        public required int TotalBins { get; set; }
        public string AddressNotes { get; set; } = string.Empty;
    }
}
