using Megabin_Web.Shared.Domain.Enums;

namespace Megabin_Web.Shared.DTOs.ScheduleContracts
{
    public class UpdateScheduleContract
    {
        public Guid ContractId { get; set; }
        public Frequency Frequency { get; set; }
        public Domain.Enums.DayOfWeek DayOfWeek { get; set; }
        public bool Active { get; set; }
        public bool ApprovedExternally { get; set; }
    }
}
