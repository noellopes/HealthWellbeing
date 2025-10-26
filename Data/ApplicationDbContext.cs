using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Models;

namespace HealthWellbeing.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets for all models
        public DbSet<Client> Clients { get; set; }
        public DbSet<MentalHealthProfessional> MentalHealthProfessionals { get; set; }
        public DbSet<TherapySession> TherapySessions { get; set; }
        public DbSet<MoodEntry> MoodEntries { get; set; }
        public DbSet<Goal> Goals { get; set; }
        public DbSet<ProgressReport> ProgressReports { get; set; }
        public DbSet<CrisisAlert> CrisisAlerts { get; set; }
        public DbSet<SelfHelpResource> SelfHelpResources { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure relationships
            builder.Entity<TherapySession>()
                .HasOne(t => t.Client)
                .WithMany(c => c.TherapySessions)
                .HasForeignKey(t => t.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TherapySession>()
                .HasOne(t => t.Professional)
                .WithMany(p => p.TherapySessions)
                .HasForeignKey(t => t.ProfessionalId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}