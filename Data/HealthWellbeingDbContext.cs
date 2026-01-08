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

        // Tabelas Intermédias e Auxiliares
        public DbSet<MemberPlan> MemberPlan { get; set; } = default!;
        public DbSet<TrainingPlan> TrainingPlan { get; set; } = default!;
    }
}