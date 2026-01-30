using HealthWellbeing.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<UtenteBalneario> UtenteBalnearios { get; set; }
        public DbSet<Genero> Generos { get; set; }

        public DbSet<SeguroSaude> SegurosSaude { get; set; }

        public DbSet<HistoricoMedico> HistoricosMedicos { get; set; }

        public DbSet<ClienteBalneario> ClientesBalneario { get; set; }

        public DbSet<SatisfacaoCliente> SatisfacoesClientes { get; set; }

        public DbSet<HistoricoPontos> HistoricoPontos { get; set; }

        public DbSet<VoucherCliente> VouchersCliente { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UtenteBalneario>()
                .HasIndex(u => u.NIF)
                .IsUnique();

            modelBuilder.Entity<UtenteBalneario>()
                .HasIndex(u => u.Contacto)
                .IsUnique();

            modelBuilder.Entity<UtenteBalneario>()
                 .HasOne(u => u.ClienteBalneario)
                 .WithMany(c => c.Utentes)
                 .HasForeignKey(u => u.ClienteBalnearioId)
                 .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ClienteBalneario>()
                 .HasIndex(c => c.Email)
                 .IsUnique();

            modelBuilder.Entity<ClienteBalneario>()
                .HasIndex(c => c.Telemovel)
                .IsUnique();

            modelBuilder.Entity<SatisfacaoCliente>()
                .HasOne(s => s.ClienteBalneario)
                .WithMany(c => c.Satisfacoes)
                .HasForeignKey(s => s.ClienteBalnearioId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<HistoricoPontos>()
                .HasOne(h => h.ClienteBalneario)
                .WithMany(c => c.HistoricoPontos)
                .HasForeignKey(h => h.ClienteBalnearioId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<VoucherCliente>()
                .HasOne(v => v.ClienteBalneario)
                .WithMany(c => c.Vouchers)
                .HasForeignKey(v => v.ClienteBalnearioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<VoucherCliente>()
                .Property(v => v.Valor)
                .HasPrecision(10, 2);






            modelBuilder.Entity<Genero>().HasData(
                new Genero { GeneroId = 1, Nome = "Masculino" },
                new Genero { GeneroId = 2, Nome = "Feminino" },
                new Genero { GeneroId = 3, Nome = "Outro" }
            );

            modelBuilder.Entity<SeguroSaude>().HasData(
                new SeguroSaude { SeguroSaudeId = 1, Nome = "ADSE" },
                new SeguroSaude { SeguroSaudeId = 2, Nome = "Multicare" },
                new SeguroSaude { SeguroSaudeId = 3, Nome = "Médis" },
                new SeguroSaude { SeguroSaudeId = 4, Nome = "Particular" }
            );


        }



    }

}
