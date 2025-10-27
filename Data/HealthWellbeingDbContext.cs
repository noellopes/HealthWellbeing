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

        // Tabelas
        public DbSet<Alergia> Alergia { get; set; } = default!;
        public DbSet<Alimento> Alimentos { get; set; } = default!;
        public DbSet<AlimentoSubstituto> AlimentoSubstitutos { get; set; } = default!;
        public DbSet<RestricaoAlimentar> RestricaoAlimentar { get; set; } = default!;
        public DbSet<ComponentesDaReceita> ComponentesDaReceita { get; set; } = default!;

        public DbSet<Receita> Receita { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração da relação auto-referenciada Alimento ↔ AlimentoSubstituto

            modelBuilder.Entity<AlimentoSubstituto>()
                .HasOne(a => a.AlimentoOriginal)
                .WithMany(a => a.Substitutos)
                .HasForeignKey(a => a.AlimentoOriginalId)
                .OnDelete(DeleteBehavior.Restrict); // evita exclusão em cascata

            modelBuilder.Entity<AlimentoSubstituto>()
                .HasOne(a => a.AlimentoSubstitutoRef)
                .WithMany(a => a.SubstituidoPor)
                .HasForeignKey(a => a.AlimentoSubstitutoRefId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Receita>()
                .HasMany(r => r.Componentes)
                .WithOne(c => c.Receita)
                .HasForeignKey(c => c.ReceitaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ComponentesDaReceita>()
                .HasOne(c => c.Receita)
                .WithMany(r => r.Componentes)
                .HasForeignKey(c => c.ReceitaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
