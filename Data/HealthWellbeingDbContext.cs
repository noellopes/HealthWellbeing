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

        public DbSet<HealthWellbeing.Models.Pathology> Pathology { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.TreatmentType> TreatmentType { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Nurse> Nurse { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.TreatmentRecord> TreatmentRecord { get; set; } = default!;
    }
}
