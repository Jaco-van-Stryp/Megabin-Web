namespace Megabin_Web.Shared.Domain.Entities
{
    public class ScheduledCollections
    {
        public Guid Id { get; set; }
        public required DateTime ScheduledFor { get; set; }
        public required Guid UserId { get; set; } //Driver Id
        public required Users User { get; set; } // Driver
        public required bool Collected { get; set; }
        public required string Notes { get; set; } = string.Empty;
    }
}
