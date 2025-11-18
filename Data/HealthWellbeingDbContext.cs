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

        }
    }
}
