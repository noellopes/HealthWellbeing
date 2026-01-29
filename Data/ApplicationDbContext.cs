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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Genero>().HasData(
                new Genero { GeneroId = 1, Nome = "Masculino" },
                new Genero { GeneroId = 2, Nome = "Feminino" },
                new Genero { GeneroId = 3, Nome = "Outro" }
            );
        }
    }

}
