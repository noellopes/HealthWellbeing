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
        public DbSet<RoomConsumable> RoomConsumable { get; set; }

        // Soft Delete interceptor
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.AddInterceptors(new SoftDeleteInterceptor());

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =====================================================
            // Filtro global para ISoftDeletable
            // =====================================================
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

            // =====================================================
            // RELACIONAMENTOS EXPLÍCITOS (evitar múltiplos cascades)
            // =====================================================

            //Se uma Room for apagada, NÃO apagues Consultations automaticamente
            modelBuilder.Entity<Consultation>()
                .HasOne(c => c.Room)
                .WithMany(r => r.Consultations)
                .HasForeignKey(c => c.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            //Se uma Room for apagada, NÃO apagues ConsumablesExpenses automaticamente
            modelBuilder.Entity<ConsumablesExpenses>()
                .HasOne(e => e.Room)
                .WithMany()
                .HasForeignKey(e => e.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            //Se uma Reserva for apagada, NÃO apagues ConsumablesExpenses automaticamente
            modelBuilder.Entity<ConsumablesExpenses>()
                .HasOne(e => e.RoomReservation)
                .WithMany()
                .HasForeignKey(e => e.RoomReservationId)
                .OnDelete(DeleteBehavior.Restrict);

            //Se um Consumível for apagado, NÃO apagues ConsumablesExpenses automaticamente
            modelBuilder.Entity<ConsumablesExpenses>()
                .HasOne(e => e.Consumable)
                .WithMany()
                .HasForeignKey(e => e.ConsumableId)
                .OnDelete(DeleteBehavior.Restrict);
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
        public DbSet<HealthWellbeing.Models.UsoConsumivel> UsoConsumivel { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.LocalizacaoZonaArmazenamento> LocalizacaoZonaArmazenamento { get; set; }

        public DbSet<TypeMaterial> TypeMaterial { get; set; } = default!;
        public DbSet<LocationMedDevice> LocationMedDevice { get; set; } = default!;
        public DbSet<EquipmentType> EquipmentType { get; set; } = default!;
        public DbSet<EquipmentStatus> EquipmentStatus { get; set; } = default!;
        public DbSet<Manufacturer> Manufacturer { get; set; } = default!;
        public DbSet<Equipment> Equipment { get; set; } = default!;
        public DbSet<MedicalDevice> MedicalDevices { get; set; } = default!;
        public DbSet<Room> Room { get; set; } = default!;
        public DbSet<RoomHistory> RoomHistories { get; set; }
        public DbSet<RoomReservationHistory> RoomReservationHistory { get; set; }

        // Novos DbSets
        public DbSet<Specialty> Specialty { get; set; } = default!;
        public DbSet<RoomStatus> RoomStatus { get; set; } = default!;
        public DbSet<RoomType> RoomType { get; set; } = default!;
        public DbSet<RoomLocation> RoomLocation { get; set; }

        public DbSet<TypeMaterialHistory> TypeMaterialHistories { get; set; }
        public DbSet<RoomReservation> RoomReservations { get; set; }
        public DbSet<Consultation> Consultations { get; set; } = default!;
        public DbSet<ConsumablesExpenses> ConsumablesExpenses { get; set; }
    }
}
