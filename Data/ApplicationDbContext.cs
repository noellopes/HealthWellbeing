using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Models;

namespace HealthWellbeing.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // CLIENTES
        public DbSet<Client> Clientes { get; set; }
        public DbSet<ClienteBalneario> ClientesBalneario { get; set; }
        public DbSet<UtenteBalneario> UtenteBalnearios { get; set; }

        // CLÍNICO
        public DbSet<HistoricoMedico> HistoricosMedicos { get; set; }

        // APOIO
        public DbSet<Genero> Generos { get; set; }
        public DbSet<Exercicio> Exercicio { get; set; }
        public DbSet<GrupoMuscular> GrupoMuscular { get; set; }
        public DbSet<SeguroSaude> SegurosSaude { get; set; }

        // FIDELIZAÇÃO
        public DbSet<SatisfacaoCliente> SatisfacoesClientes { get; set; }
        public DbSet<HistoricoPontos> HistoricoPontos { get; set; }
        public DbSet<VoucherCliente> VouchersCliente { get; set; }
        public DbSet<NivelCliente> NiveisCliente { get; set; }
    }
}