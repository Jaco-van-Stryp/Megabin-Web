using Megabin_Web.Shared.Domain.Enums;

namespace Megabin_Web.Shared.DTOs.ScheduleContracts
{
    public class CreateScheduleContract
    {
        public required Guid AddressId { get; set; }
        public required Domain.Enums.DayOfWeek DayOfWeek { get; set; }
        public required Frequency Frequency { get; set; }
    }
}
