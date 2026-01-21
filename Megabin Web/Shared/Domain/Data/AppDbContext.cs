using Megabin_Web.Shared.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Megabin_Web.Shared.Domain.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options)
            : base(options) { }

        public DbSet<Addresses> Addresses { get; set; }
        public DbSet<ScheduledCollections> ScheduledCollections { get; set; }
        public DbSet<ScheduleContract> ScheduledContract { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<APIUsageTracker> APIUsageTrackers { get; set; }
    }
}
