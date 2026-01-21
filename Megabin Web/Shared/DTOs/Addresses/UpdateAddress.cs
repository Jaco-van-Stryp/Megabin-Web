using Megabin_Web.Shared.Domain.Enums;
using Megabin_Web.Shared.DTOs.Routing;

namespace Megabin_Web.Shared.DTOs.Addresses
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
