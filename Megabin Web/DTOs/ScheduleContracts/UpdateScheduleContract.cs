using Megabin_Web.Enums;

namespace Megabin_Web.DTOs.ScheduleContracts
{
    public class UpdateScheduleContract
    {
        public Guid ContractId { get; set; }
        public Frequency Frequency { get; set; }
        public Enums.DayOfWeek DayOfWeek { get; set; }
        public bool Active { get; set; }
        public bool ApprovedExternally { get; set; }
    }
}
