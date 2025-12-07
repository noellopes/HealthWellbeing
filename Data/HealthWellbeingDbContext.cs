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
        public HealthWellbeingDbContext(DbContextOptions<HealthWellbeingDbContext> options)
            : base(options)
        {
        }

        public DbSet<TypeMaterial> TypeMaterial { get; set; } = default!;
        public DbSet<LocationMedDevice> LocationMedDevice { get; set; } = default!;
        public DbSet<EquipmentType> EquipmentType { get; set; } = default!;
        public DbSet<EquipmentStatus> EquipmentStatus { get; set; } = default!;
        public DbSet<Manufacturer> Manufacturer { get; set; } = default!;
        public DbSet<Equipment> Equipment { get; set; } = default!;
        public DbSet<MedicalDevice> MedicalDevices { get; set; } = default!;
        public DbSet<Room> Room { get; set; } = default!;
        public DbSet<Alergia> Alergia { get; set; } = default!;
        public DbSet<RestricaoAlimentar> RestricaoAlimentar { get; set; } = default!;
        public DbSet<Receita> Receita { get; set; } = default!;

        // Novos DbSets
        public DbSet<Specialty> Specialty { get; set; } = default!;
        public DbSet<RoomStatus> RoomStatus { get; set; } = default!;
        public DbSet<RoomType> RoomType { get; set; } = default!;
        public DbSet<RoomLocation> RoomLocation { get; set; }
    }
}