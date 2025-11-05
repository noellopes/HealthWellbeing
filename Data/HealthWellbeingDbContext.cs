using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Models;
using HealthWellbeingRoom.Models;

namespace HealthWellbeing.Data
{
    public class HealthWellbeingDbContext : DbContext
    {
        public HealthWellbeingDbContext (DbContextOptions<HealthWellbeingDbContext> options)
            : base(options)
        {
        }
        public DbSet<HealthWellbeing.Models.LocationMaterial> LocationMaterial { get; set; } = default!;

        public DbSet<HealthWellbeingRoom.Models.Equipment> Equipment { get; set; } = default!;

        public DbSet<MedicalDevice> MedicalDevices { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Room> Room { get; set; } = default!;


        public DbSet<HealthWellbeing.Models.Alergia> Alergia { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.RestricaoAlimentar> RestricaoAlimentar { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Receita> Receita { get; set; } = default!;
    }
}
