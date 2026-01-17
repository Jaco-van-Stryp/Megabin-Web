using Megabin_Web.Enums;

namespace Megabin_Web.Entities
{
    public class ScheduleContract
    {
        public Guid Id { get; set; }
        public required Frequency Frequency { get; set; }
        public required Enums.DayOfWeek DayOfWeek { get; set; }
        public DateTime StartingDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastCollected { get; set; }
        public required bool Active { get; set; } = true;
        public required bool ApprovedExternally { get; set; } = false;
        public required Guid AddressesId { get; set; }
        public required Addresses Addresses { get; set; }
    }
}
