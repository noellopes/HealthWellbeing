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

        // Entidades Principais
        public DbSet<Client> Client { get; set; } = default!;
        public DbSet<Member> Member { get; set; } = default!;
        public DbSet<Trainer> Trainer { get; set; } = default!;
        public DbSet<Plan> Plan { get; set; } = default!;

        // Entidades de Treino e Exercício
        public DbSet<Training> Training { get; set; } = default!;
        public DbSet<TrainingType> TrainingType { get; set; } = default!;
        public DbSet<Exercise> Exercise { get; set; } = default!;
        public DbSet<TrainingExercise> TrainingExercise { get; set; } = default!;
        public DbSet<PhysicalAssessment> PhysicalAssessment { get; set; } = default!;

        // Tabelas Intermédias e Auxiliares
        public DbSet<MemberPlan> MemberPlan { get; set; } = default!;
        public DbSet<TrainingPlan> TrainingPlan { get; set; } = default!;
        public DbSet<Session> Session { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração para Peso, Altura, percentagem de gordura e massa muscular
            modelBuilder.Entity<PhysicalAssessment>()
                .Property(p => p.Weight).HasColumnType("decimal(5,2)");
            modelBuilder.Entity<PhysicalAssessment>()
                .Property(p => p.Height).HasColumnType("decimal(3,2)");
            modelBuilder.Entity<PhysicalAssessment>()
                .Property(p => p.BodyFatPercentage).HasColumnType("decimal(5,2)");
            modelBuilder.Entity<PhysicalAssessment>()
                .Property(p => p.MuscleMass).HasColumnType("decimal(5,2)");
        }
    }
}