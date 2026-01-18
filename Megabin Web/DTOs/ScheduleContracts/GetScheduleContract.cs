using Megabin_Web.Enums;

namespace Megabin_Web.DTOs.ScheduleContracts
{
    public class GetScheduleContract
    {
        public required Guid Id { get; set; }
        public required Frequency Frequency { get; set; }
        public required Enums.DayOfWeek DayOfWeek { get; set; }
        public required DateTime StartingDate { get; set; }
        public DateTime? LastCollected { get; set; }
        public required bool Active { get; set; }
        public required bool ApprovedExternally { get; set; }
        public required Guid AddressesId { get; set; }
    }
}
