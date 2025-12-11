using HealthWellbeing.Utils.Group1.Interfaces;
using HealthWellbeing.Utils.Group1.Services;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Models;
using HealthWellbeingRoom.Models;
using System.Linq.Expressions;

namespace HealthWellbeing.Data
{
    public class HealthWellbeingDbContext : DbContext
    {
        public HealthWellbeingDbContext (DbContextOptions<HealthWellbeingDbContext> options)
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

        public DbSet<RoomHistory> RoomHistories { get; set; }

        // Novos DbSets
        public DbSet<Specialty> Specialty { get; set; } = default!;
        public DbSet<RoomStatus> RoomStatus { get; set; } = default!;
        public DbSet<RoomType> RoomType { get; set; } = default!;
        public DbSet<RoomLocation> RoomLocation { get; set; }
    }
}