using Megabin_Web.Enums;

namespace Megabin_Web.DTOs.Addresses
{
    public class GetAddress
    {
        public required Guid Id { get; set; }
        public required string Address { get; set; }
        public required string AddressNotes { get; set; }
        public required int TotalBins { get; set; }
        public required AddressStatus AddressStatus { get; set; }
    }
}
