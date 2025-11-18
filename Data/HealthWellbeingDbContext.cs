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
        public HealthWellbeingDbContext(DbContextOptions<HealthWellbeingDbContext> options): base(options)
        {
        }

        public DbSet<HealthWellbeing.Models.Alergia> Alergia { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.RestricaoAlimentar> RestricaoAlimentar { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Receita> Receita { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Exercicio> Exercicio { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.TipoExercicio> TipoExercicio { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Beneficio> Beneficio { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.TipoExercicioBeneficio> TipoExercicioBeneficio { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.ProblemaSaude> ProblemaSaude { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Musculo> Musculo { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.GrupoMuscular> GrupoMuscular { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Genero> Genero { get; set; } = default!;
        public DbSet<ProfissionalExecutante> ProfissionalExecutante { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TipoExercicioBeneficio>()
                .HasKey(tb => new { tb.TipoExercicioId, tb.BeneficioId });

            modelBuilder.Entity<TipoExercicioBeneficio>()
                .HasOne(tb => tb.TipoExercicio)
                .WithMany(t => t.TipoExercicioBeneficios)
                .HasForeignKey(tb => tb.TipoExercicioId);

            modelBuilder.Entity<TipoExercicioBeneficio>()
                .HasOne(tb => tb.Beneficio)
                .WithMany(b => b.TipoExercicioBeneficios)
                .HasForeignKey(tb => tb.BeneficioId);

            modelBuilder.Entity<ExercicioGenero>()
                .HasKey(eg => new { eg.ExercicioId, eg.GeneroId });

            modelBuilder.Entity<ExercicioGenero>()
                .HasOne(eg => eg.Exercicio)
                .WithMany(e => e.ExercicioGeneros)
                .HasForeignKey(eg => eg.ExercicioId);

            modelBuilder.Entity<ExercicioGenero>()
                .HasOne(eg => eg.Genero)
                .WithMany(g => g.ExercicioGeneros)
                .HasForeignKey(eg => eg.GeneroId);

            modelBuilder.Entity<ExercicioGrupoMuscular>()
                .HasKey(egm => new { egm.ExercicioId, egm.GrupoMuscularId });

            modelBuilder.Entity<ExercicioGrupoMuscular>()
                .HasOne(egm => egm.Exercicio)
                .WithMany(e => e.ExercicioGrupoMusculares)
                .HasForeignKey(egm => egm.ExercicioId);

            modelBuilder.Entity<ExercicioGrupoMuscular>()
                .HasOne(egm => egm.GrupoMuscular)
                .WithMany(gm => gm.ExercicioGrupoMusculares)
                .HasForeignKey(egm => egm.GrupoMuscularId);

            modelBuilder.Entity<ProblemaSaude>()
                    .HasMany(p => p.ProfissionalExecutante)      // Lado A: Problema tem Profissionais
                    .WithMany(p => p.ProblemasSaude)              // Lado B: Profissional tem Problemas <--- O ERRO ESTAVA AQUI (Faltava esta ligação)
                    .UsingEntity(j => j.ToTable("ProblemaSaudeProfissionais"));

        }
    }

}