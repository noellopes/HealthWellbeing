using HealthWellbeing.Utils.Group1.Interfaces;
using HealthWellbeing.Utils.Group1.Services;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Models;
using HealthWellbeingRoom.Models;
using System.Linq.Expressions;
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

        public DbSet<HealthWellbeing.Models.Pathology> Pathology { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.TreatmentType> TreatmentType { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Nurse> Nurse { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.TreatmentRecord> TreatmentRecord { get; set; } = default!;

        // Adiciona capacidade de "Soft Delete" ao contexto
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.AddInterceptors(new SoftDeleteInterceptor());

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Regista um filtro nas query dos modelos que implementam ISoftDeletable
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
                {
                    var method = typeof(ModelBuilder).GetMethods()
                        .First(m => m.Name == "Entity" && m.IsGenericMethod);
                    var genericMethod = method.MakeGenericMethod(entityType.ClrType);
                    dynamic entityBuilder = genericMethod.Invoke(modelBuilder, null);
                    entityBuilder.HasQueryFilter(CreateIsDeletedFilter(entityType.ClrType));
                }
            }
        }
        private static LambdaExpression CreateIsDeletedFilter(Type entityType)
        {
            var param = Expression.Parameter(entityType, "x");
            var prop = Expression.Property(param, "IsDeleted");
            var condition = Expression.Equal(prop, Expression.Constant(false));
            return Expression.Lambda(condition, param);
        }
        public DbSet<HealthWellbeing.Models.ZonaArmazenamento> ZonaArmazenamento { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.CategoriaConsumivel> CategoriaConsumivel { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Fornecedor> Fornecedor { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Consumivel> Consumivel { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Stock> Stock { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.StockMovimento> StockMovimento { get; set; }

        public DbSet<HealthWellbeing.Models.UsoConsumivel> UsoConsumivel { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.LocalizacaoZonaArmazenamento> LocalizacaoZonaArmazenamento { get; set; }
        public DbSet<HealthWellbeing.Models.Fornecedor_Consumivel> Fornecedor_Consumivel { get; set; } = default!;


        public DbSet<TypeMaterial> TypeMaterial { get; set; } = default!;
        public DbSet<LocationMedDevice> LocationMedDevice { get; set; } = default!;
        public DbSet<EquipmentType> EquipmentType { get; set; } = default!;
        public DbSet<EquipmentStatus> EquipmentStatus { get; set; } = default!;
        public DbSet<Manufacturer> Manufacturer { get; set; } = default!;
        public DbSet<Equipment> Equipment { get; set; } = default!;
        public DbSet<MedicalDevice> MedicalDevices { get; set; } = default!;
        public DbSet<Room> Room { get; set; } = default!;
        public DbSet<RoomHistory> RoomHistories { get; set; }

        // Novos DbSets
        public DbSet<Specialty> Specialty { get; set; } = default!;
        public DbSet<RoomStatus> RoomStatus { get; set; } = default!;
        public DbSet<RoomType> RoomType { get; set; } = default!;
        public DbSet<RoomLocation> RoomLocation { get; set; }
    }

}
