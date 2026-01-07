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

        public DbSet<HealthWellbeing.Models.AgendamentoBalneario> Agendamento { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Servico> Servicos { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.TipoServicos> TipoServicos { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Terapeuta> Terapeuta { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.EventType> EventType { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Level> Level { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Event> Event { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Activity_> Activity { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Alergia> Alergia { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.RestricaoAlimentar> RestricaoAlimentar { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Receita> Receita { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.UtenteSaude> UtenteSaude { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Consulta> Consulta { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Doctor> Doctor { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Specialities> Specialities { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Member> Member { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Client> Client { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.TrainingType> TrainingType { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Plan> Plan { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Trainer> Trainer { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Training> Training { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Exercicio> Exercicios { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.TipoExercicio> TipoExercicio { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Beneficio> Beneficio { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.ProblemaSaude> ProblemaSaude { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Musculo> Musculo { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.GrupoMuscular> GrupoMuscular { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Genero> Genero { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.ProfissionalExecutante> ProfissionaisExecutantes { get; set; }
       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Correção para os campos Decimal (evita os avisos/warnings)
            modelBuilder.Entity<Alimento>().Property(a => a.KcalPor100g).HasPrecision(18, 2);
            modelBuilder.Entity<Alimento>().Property(a => a.ProteinaGPor100g).HasPrecision(18, 2);
            modelBuilder.Entity<Alimento>().Property(a => a.HidratosGPor100g).HasPrecision(18, 2);
            modelBuilder.Entity<Alimento>().Property(a => a.GorduraGPor100g).HasPrecision(18, 2);
            modelBuilder.Entity<Plan>().Property(p => p.Price).HasPrecision(18, 2);
            modelBuilder.Entity<Receita>().Property(r => r.CaloriasPorPorcao).HasPrecision(18, 2);
            modelBuilder.Entity<Receita>().Property(r => r.Proteinas).HasPrecision(18, 2);
            modelBuilder.Entity<Receita>().Property(r => r.HidratosCarbono).HasPrecision(18, 2);
            modelBuilder.Entity<Receita>().Property(r => r.Gorduras).HasPrecision(18, 2);
            modelBuilder.Entity<Activity_>().Property(a => a.Weigth).HasPrecision(18, 2);

            modelBuilder.Entity<AgendamentoBalneario>()
         .HasOne(a => a.Servico)
         .WithMany()
         .HasForeignKey(a => a.ServicoId)
         .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AgendamentoBalneario>()
                .HasOne(a => a.TipoServico)
                .WithMany()
                .HasForeignKey(a => a.TipoServicoId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Event>()
                .HasOne(e => e.EventType)
                .WithMany()
                .HasForeignKey(e => e.EventTypeId)
                .OnDelete(DeleteBehavior.NoAction);
        }

    }

    }