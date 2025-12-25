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
        public HealthWellbeingDbContext(DbContextOptions<HealthWellbeingDbContext> options)
            : base(options)
        {
        }
        public DbSet<HealthWellbeing.Models.EventType> EventType { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Level> Level { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.LevelCategory> LevelCategory { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Event> Event { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Activity> Activity { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.ActivityType> ActivityType { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.EventActivity> EventActivity { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Alergia> Alergia { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.RestricaoAlimentar> RestricaoAlimentar { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Receita> Receita { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Member> Member { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Client> Client { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.TrainingType> TrainingType { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Plan> Plan { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Trainer> Trainer { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Training> Training { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Exercicio> Exercicio { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.TipoExercicio> TipoExercicio { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Beneficio> Beneficio { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.ProblemaSaude> ProblemaSaude { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Musculo> Musculo { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.GrupoMuscular> GrupoMuscular { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Genero> Genero { get; set; } = default!;
        public DbSet<ProfissionalExecutante> ProfissionalExecutante { get; set; }
        public DbSet<HealthWellbeing.Models.BadgeType> BadgeType { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Badge> Badge { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.BadgeRequirement> BadgeRequirement { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.CustomerBadge> CustomerBadge { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Employee> Employee { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Customer> Customer { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Event>()
                .HasOne(e => e.EventType)
                .WithMany(et => et.Events)
                .HasForeignKey(e => e.EventTypeId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Customer>()
                .HasOne(c => c.Level)
                .WithMany(l => l.Customer)
                .HasForeignKey(c => c.LevelId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Badge>()
                .HasOne(b => b.BadgeType)
                .WithMany(bt => bt.Badges)
                .HasForeignKey(b => b.BadgeTypeId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BadgeRequirement>()
                .HasOne(br => br.Badge)
                .WithMany(b => b.BadgeRequirements)
                .HasForeignKey(br => br.BadgeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CustomerBadge>(entity =>
            {
                entity.HasKey(cb => new { cb.CustomerId, cb.BadgeId });

                entity.HasOne(cb => cb.Customer)
                      .WithMany(c => c.CustomerBadges)
                      .HasForeignKey(cb => cb.CustomerId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(cb => cb.Badge)
                      .WithMany(b => b.CustomerBadges)
                      .HasForeignKey(cb => cb.BadgeId)
                      .OnDelete(DeleteBehavior.Cascade);
            });




            modelBuilder.Entity<Activity>()
                .HasOne(a => a.ActivityType)
                .WithMany(at => at.Activities)
                .HasForeignKey(a => a.ActivityTypeId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<EventActivity>()
               .HasKey(ea => new { ea.EventId, ea.ActivityId });

            modelBuilder.Entity<EventActivity>()
                .HasOne(ea => ea.Event)
                .WithMany(e => e.EventActivities)
                .HasForeignKey(ea => ea.EventId);

            modelBuilder.Entity<EventActivity>()
                .HasOne(ea => ea.Activity)
                .WithMany() 
                .HasForeignKey(ea => ea.ActivityId);
        }
    }
}