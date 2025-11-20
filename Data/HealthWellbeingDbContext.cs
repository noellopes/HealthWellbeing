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
        public DbSet<HealthWellbeing.Models.Alergia> Alergia { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.RestricaoAlimentar> RestricaoAlimentar { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Receita> Receita { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Exercicio> Exercicio { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.TipoExercicio> TipoExercicio { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Beneficio> Beneficio { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.TipoExercicioBeneficio> TipoExercicioBeneficio { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.ProblemaSaude> ProblemaSaude { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.TipoExercicioProblemaSaude> TipoExercicioProblemaSaude { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Musculo> Musculo { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.GrupoMuscular> GrupoMuscular { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Genero> Genero { get; set; } = default!;

        public DbSet<HealthWellbeing.Models.Equipamento> Equipamento { get; set; }
        public DbSet<ProfissionalExecutante> ProfissionalExecutante { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Chave primária do GrupoMuscular
            modelBuilder.Entity<GrupoMuscular>()
                .HasKey(g => g.GrupoMuscularId);

            // Relação Musculo → GrupoMuscular
            modelBuilder.Entity<Musculo>()
                .HasOne(m => m.GrupoMuscular)
                .WithMany(g => g.Musculos)
                .HasForeignKey(m => m.GrupoMuscularId)
                .OnDelete(DeleteBehavior.NoAction);

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

            modelBuilder.Entity<TipoExercicioProblemaSaude>()
                .HasKey(tb => new { tb.TipoExercicioId, tb.ProblemaSaudeId });

            modelBuilder.Entity<TipoExercicioProblemaSaude>()
                .HasOne(tb => tb.TipoExercicio)
                .WithMany(t => t.Contraindicacao)
                .HasForeignKey(tb => tb.TipoExercicioId);

            modelBuilder.Entity<TipoExercicioProblemaSaude>()
                .HasOne(tb => tb.ProblemaSaude)
                .WithMany(b => b.TipoExercicioAfetado)
                .HasForeignKey(tb => tb.ProblemaSaudeId);

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

            modelBuilder.Entity<ExercicioEquipamento>()
                .HasKey(eg => new { eg.ExercicioId, eg.EquipamentoId });

            modelBuilder.Entity<ExercicioEquipamento>()
                .HasOne(eg => eg.Exercicio)
                .WithMany(e => e.ExercicioEquipamentos)
                .HasForeignKey(eg => eg.ExercicioId);

            modelBuilder.Entity<ExercicioEquipamento>()
                .HasOne(eg => eg.Equipamento)
                .WithMany(g => g.ExercicioEquipamentos)
                .HasForeignKey(eg => eg.EquipamentoId);

        }
    }
}
