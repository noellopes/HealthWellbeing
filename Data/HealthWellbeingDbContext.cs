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
        public DbSet<HealthWellbeing.Models.ScoringStrategy> ScoringStrategy { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Level> Level { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Event> Event { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Activity_> Activity { get; set; } = default!;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder) {

            modelBuilder.Entity<Event>()
                .HasOne(e => e.EventType)
                .WithMany(et => et.Events)
                .HasForeignKey(e => e.EventTypeId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<EventType>()
                .HasOne(et => et.ScoringStrategy)   
                .WithMany(ss => ss.EventTypes)          
                .HasForeignKey(et => et.ScoringStrategyId) 
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
