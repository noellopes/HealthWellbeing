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
        public HealthWellbeingDbContext(DbContextOptions<HealthWellbeingDbContext> options)
            : base(options)
        {
        }
        public DbSet<HealthWellbeing.Models.Alergia> Alergia { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.RestricaoAlimentar> RestricaoAlimentar { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Receita> Receita { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.ZonaArmazenamento> ZonaArmazenamento { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.CategoriaConsumivel> CategoriaConsumivel { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Fornecedor> Fornecedor { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Consumivel> Consumivel { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Stock> Stock { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.AuditoriaConsumivel> AuditoriaConsumivel { get; set; } = default!;
    }
}
