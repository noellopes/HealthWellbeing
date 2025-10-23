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

        //(CRUD de alimentos)
        public DbSet<Alimento> Alimentos { get; set; } = default!;
        public DbSet<CategoryFood> Categorias { get; set; } = default!;

        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //Nomes de tabelas personalizados
            builder.Entity<Alimento>().ToTable("Alimento");
            builder.Entity<CategoryFood>().ToTable("Categoria");
        }
    }
}
