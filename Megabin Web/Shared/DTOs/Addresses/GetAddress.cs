using Megabin_Web.Shared.Domain.Enums;
using Megabin_Web.Shared.DTOs.Routing;

namespace Megabin_Web.Shared.DTOs.Addresses
{
    public class GetAddress
    {
        public required Guid Id { get; set; }
        public required string Address { get; set; }
        public required string AddressNotes { get; set; }
        public required int TotalBins { get; set; }
        public required AddressStatus AddressStatus { get; set; }
        public required Location Location { get; set; }
    }
}
