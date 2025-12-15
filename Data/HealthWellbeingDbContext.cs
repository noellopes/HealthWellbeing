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

        public DbSet<AlergiaAlimento> AlergiaAlimento { get; set; }
        public DbSet<AlimentoSubstituto> AlimentoSubstitutos { get; set; } = default!;
        public DbSet<RestricaoAlimentar> RestricaoAlimentar { get; set; } = default!;
        public DbSet<RestricaoAlimentarAlimento> RestricaoAlimentarAlimento { get; set; }

        public DbSet<ComponenteReceita> ComponenteReceita { get; set; } = default!;

        public DbSet<CategoriaAlimento> CategoriaAlimento { get; set; } = default!;

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

            modelBuilder.Entity<AlergiaAlimento>()
                .HasKey(aa => new { aa.AlergiaId, aa.AlimentoId });

            modelBuilder.Entity<AlergiaAlimento>()
                .HasOne(aa => aa.Alergia)
                .WithMany(a => a.AlimentosAssociados)
                .HasForeignKey(aa => aa.AlergiaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AlergiaAlimento>()
                .HasOne(aa => aa.Alimento)
                .WithMany(al => al.AlergiaRelacionadas)
                .HasForeignKey(aa => aa.AlimentoId)
                .OnDelete(DeleteBehavior.Cascade);


            // N:N RestricaoAlimentar ↔ Alimento
            modelBuilder.Entity<RestricaoAlimentarAlimento>()
                .HasKey(ra => new { ra.RestricaoAlimentarId, ra.AlimentoId });

            modelBuilder.Entity<RestricaoAlimentarAlimento>()
                .HasOne(ra => ra.RestricaoAlimentar)
                .WithMany(r => r.AlimentosAssociados)
                .HasForeignKey(ra => ra.RestricaoAlimentarId);

            modelBuilder.Entity<RestricaoAlimentarAlimento>()
                .HasOne(ra => ra.Alimento)
                .WithMany(a => a.RestricoesAssociadas)
                .HasForeignKey(ra => ra.AlimentoId);

            modelBuilder.Entity<ComponenteReceita>()
                .HasOne(c => c.Receita)
                .WithMany(r => r.Componentes)
                .HasForeignKey(c => c.ReceitaId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
