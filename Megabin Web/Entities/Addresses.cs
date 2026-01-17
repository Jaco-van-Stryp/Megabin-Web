using Megabin_Web.Enums;

namespace Megabin_Web.Entities
{
    public class Addresses
    {
        public Guid Id { get; set; }
        public required string Address { get; set; }
        public required int TotalBins { get; set; }
        public required double Long { get; set; }
        public string? AddressNotes { get; set; }
        public required double Lat { get; set; }
        public required Guid UserId { get; set; }
        public required Users User { get; set; }
        public AddressStatus Status { get; set; } = AddressStatus.PendingAddressCompletion;
        public ICollection<ScheduleContract> Schedules { get; set; } = new List<ScheduleContract>();
    }
}
