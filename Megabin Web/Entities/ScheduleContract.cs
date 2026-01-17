namespace Megabin_Web.Entities
{
    public class ScheduleContract
    {
        public Guid Id { get; set; }
        public required string Frequency { get; set; }
        public required string DayOfWeek { get; set; }
        public required DateTime StartingDate { get; set; } = DateTime.Now;
        public required DateTime LastCollected { get; set; }
        public required bool Active { get; set; } = true;
        public required bool ApprovedExternally { get; set; } = false;
        public required Guid AddressesId { get; set; }
        public required Addresses Addresses { get; set; }
    }
}
