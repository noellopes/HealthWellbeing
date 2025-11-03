using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Models;

namespace HealthWellbeing.Data
{
    public class HealthWellbeingDbContext : DbContext
    {
        public HealthWellbeingDbContext (DbContextOptions<HealthWellbeingDbContext> options)
            : base(options)
        {
        }
        public DbSet<HealthWellbeing.Models.EventType> EventType { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Levels> Levels { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Event> Events { get; set; } = default!;
    }
}
