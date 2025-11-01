using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Models;

namespace HealthWellbeing.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public AppDbContext()
        {
        }


        public DbSet<Levels> Levels { get; set; } = default!;
        //public DbSet<HealthWellbeing.Models.RestricaoAlimentar> RestricaoAlimentar { get; set; } = default!;
        //public DbSet<HealthWellbeing.Models.Receita> Receita { get; set; } = default!;



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // ⚠️ Usa a mesma connection string que tens no appsettings.json
                optionsBuilder.UseSqlServer(
                    "Server=(localdb)\\mssqllocaldb;Database=HealthWellbeingAppDB;Trusted_Connection=True;MultipleActiveResultSets=true");
            }
        }
    }
}
