using Megabin_Web.Enums;

namespace Megabin_Web.DTOs.ScheduleContracts
{
    public class CreateScheduleContract
    {
        public required Guid AddressId { get; set; }
        public required Enums.DayOfWeek DayOfWeek { get; set; }
        public required Frequency Frequency { get; set; }
    }
}
