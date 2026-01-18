using Megabin_Web.DTOs.Routing;
using Megabin_Web.Enums;

namespace Megabin_Web.DTOs.Addresses
{
    public class UpdateAddress
    {
        public required Guid AddressId { get; set; }
        public required int TotalBins { get; set; }
        public string AddressNotes { get; set; } = string.Empty;
        public required AddressStatus Status { get; set; }
        public required Location Location { get; set; }
    }
}
