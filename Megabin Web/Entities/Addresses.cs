namespace Megabin_Web.Entities
{
    public class Addresses
    {
        public Guid Id { get; set; }
        public required string Address { get; set; }
        public required int TotalBins { get; set; }
        public required double Long { get; set; }
        public required double Lat { get; set; }
        public required Guid UserId { get; set; }
        public required Users User { get; set; }
        public required string Status { get; set; } = "Request_Bin";
        public required ICollection<ScheduleContract> Schedules { get; set; } =
            new List<ScheduleContract>();
    }
}
