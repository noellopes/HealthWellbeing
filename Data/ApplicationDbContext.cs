using HealthWellbeing.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //DBSets
        public DbSet<UtenteBalneario> Utentes { get; set; }
        public DbSet<DadosMedicos> DadosMedicos { get; set; }
        public DbSet<SeguroSaude> SegurosSaude { get; set; }

        public DbSet<TerapeutaModel> Terapeutas { get; set; }
        public DbSet<Servico> Servicos { get; set; }
        public DbSet<AgendamentoModel> Agendamentos { get; set; }
        public DbSet<TipoServico> TipoServicos{ get; set; }


        //Fluent API
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // RELAÇÃO 1–1 Utente → DadosMedicos
            builder.Entity<UtenteBalneario>()
                .HasOne(u => u.DadosMedicos)
                .WithOne()
                .HasForeignKey<UtenteBalneario>(u => u.DadosMedicos)
                .OnDelete(DeleteBehavior.Cascade);

            // RELAÇÃO 1–1 Utente → SeguroSaude
            builder.Entity<UtenteBalneario>()
                .HasOne(u => u.SeguroSaude)
                .WithOne()
                .HasForeignKey<UtenteBalneario>(u => u.SeguroSaude)
                .OnDelete(DeleteBehavior.Cascade);

            // Regras adicionais (Fluent API)
            builder.Entity<UtenteBalneario>()
                .Property(u => u.NomeCompleto)
                .IsRequired()
                .HasMaxLength(100);

            builder.Entity<UtenteBalneario>()
                .Property(u => u.Morada)
                .IsRequired()
                .HasMaxLength(200);

            builder.Entity<UtenteBalneario>()
                .Property(u => u.NIF)
                .IsRequired()
                .HasMaxLength(9);
        }
    }
}
