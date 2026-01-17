using Megabin_Web.Enums;

namespace Megabin_Web.Entities
{
    public class APIUsageTracker
    {
        public Guid Id { get; set; } // Each API Requests counts as one entry
        public required APITypes ExternalApiName { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public Guid? UserId { get; set; }
        public Users? User { get; set; }
    }
}
