using HealthWellbeing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Models {
    public class EventTypeDbContext: DbContext {
        public EventTypeDbContext(DbContextOptions<EventTypeDbContext> options) : base(options) {}

        public DbSet<EventType> EventTypes { get; set; }
    }
}
