namespace Megabin_Web.Shared.Domain.Entities
{
    public class ScheduledCollections
    {
        public Guid Id { get; set; }
        public required DateTime ScheduledFor { get; set; }
        public required Guid UserId { get; set; } // Driver Id
        public required Users User { get; set; } // Driver

        // Address being collected from
        public required Guid AddressId { get; set; }
        public required Addresses Address { get; set; }

        // Route order (1-based sequence in the driver's daily route)
        public int RouteSequence { get; set; }

        public required bool Collected { get; set; }
        public required string Notes { get; set; } = string.Empty;
    }
}
