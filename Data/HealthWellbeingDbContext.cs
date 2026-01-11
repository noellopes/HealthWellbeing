using HealthWellbeing.Utils.Group1.Interfaces;
using HealthWellbeing.Utils.Group1.Services;
using HealthWellbeing.Models;
using HealthWellbeingRoom.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace HealthWellbeing.Data
{
    public class HealthWellbeingDbContext : DbContext
    {
        public HealthWellbeingDbContext(DbContextOptions<HealthWellbeingDbContext> options)
            : base(options)
        {
        }

        // ========================
        // CONFIGURAÇÃO GLOBAL
        // ========================
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(new SoftDeleteInterceptor());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ========================
            // SOFT DELETE (GLOBAL FILTER)
            // ========================
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "x");
                    var property = Expression.Property(parameter, "IsDeleted");
                    var condition = Expression.Equal(property, Expression.Constant(false));
                    var lambda = Expression.Lambda(condition, parameter);

                    modelBuilder
                        .Entity(entityType.ClrType)
                        .HasQueryFilter(lambda);
                }
            }

            // ========================
            // RELAÇÕES DO STOCK (⚠️ MUITO IMPORTANTE)
            // ========================
            modelBuilder.Entity<Stock>()
                .HasOne(s => s.Consumivel)
                .WithMany()
                .HasForeignKey(s => s.ConsumivelID)
                .OnDelete(DeleteBehavior.Cascade); // ✔️ permitido

            modelBuilder.Entity<Stock>()
                .HasOne(s => s.Zona)
                .WithMany()
                .HasForeignKey(s => s.ZonaID)
                .OnDelete(DeleteBehavior.Restrict); // ❗ evita cascade paths

            modelBuilder.Entity<ZonaArmazenamento>()
            .HasOne(z => z.Consumivel)
            .WithMany()
            .HasForeignKey(z => z.ConsumivelId)
            .OnDelete(DeleteBehavior.Restrict); // impede apagar consumível se tiver zonas


            modelBuilder.Entity<Stock>()
            .HasOne(s => s.Zona)
            .WithMany()
            .HasForeignKey(s => s.ZonaID)
            .OnDelete(DeleteBehavior.Cascade);

            // ========================
            // CONFIGURAÇÃO HISTÓRICO TRANSFERÊNCIAS (Evitar Ciclos)
            // ========================
            modelBuilder.Entity<HistoricoTransferencia>()
                .HasOne(h => h.ZonaOrigem)
                .WithMany()
                .HasForeignKey(h => h.ZonaOrigemId)
                .OnDelete(DeleteBehavior.Restrict); // ⚠️ Importante: Restrict em vez de Cascade

            modelBuilder.Entity<HistoricoTransferencia>()
                .HasOne(h => h.ZonaDestino)
                .WithMany()
                .HasForeignKey(h => h.ZonaDestinoId)
                .OnDelete(DeleteBehavior.Restrict); // ⚠️ Importante: Restrict em vez de Cascade

        }

        // ========================
        // DBSets – Grupo Alimentar / Saúde
        // ========================
        public DbSet<Alergia> Alergia { get; set; } = default!;
        public DbSet<RestricaoAlimentar> RestricaoAlimentar { get; set; } = default!;
        public DbSet<Receita> Receita { get; set; } = default!;
        public DbSet<Pathology> Pathology { get; set; } = default!;
        public DbSet<TreatmentType> TreatmentType { get; set; } = default!;
        public DbSet<Nurse> Nurse { get; set; } = default!;
        public DbSet<TreatmentRecord> TreatmentRecord { get; set; } = default!;

        // ========================
        // CONSUMÍVEIS / STOCK
        // ========================
        public DbSet<ZonaArmazenamento> ZonaArmazenamento { get; set; } = default!;

        public DbSet<CategoriaConsumivel> CategoriaConsumivel { get; set; } = default!;
        public DbSet<Fornecedor> Fornecedor { get; set; } = default!;
        public DbSet<Consumivel> Consumivel { get; set; } = default!;
        public DbSet<Stock> Stock { get; set; } = default!;
        public DbSet<HistoricoCompras> HistoricoCompras { get; set; } = default!;
        public DbSet<Compra> Compra { get; set; } = default!;
        public DbSet<UsoConsumivel> UsoConsumivel { get; set; } = default!;
        public DbSet<LocalizacaoZonaArmazenamento> LocalizacaoZonaArmazenamento { get; set; } = default!;
        public DbSet<Fornecedor_Consumivel> Fornecedor_Consumivel { get; set; } = default!;
        public DbSet<HistoricoTransferencia> HistoricoTransferencias { get; set; } = default!;

        // ========================
        // EQUIPAMENTOS / SALAS
        // ========================
        public DbSet<TypeMaterial> TypeMaterial { get; set; } = default!;
        public DbSet<LocationMedDevice> LocationMedDevice { get; set; } = default!;
        public DbSet<EquipmentType> EquipmentType { get; set; } = default!;
        public DbSet<EquipmentStatus> EquipmentStatus { get; set; } = default!;
        public DbSet<Manufacturer> Manufacturer { get; set; } = default!;
        public DbSet<Equipment> Equipment { get; set; } = default!;
        public DbSet<MedicalDevice> MedicalDevices { get; set; } = default!;
        public DbSet<Room> Room { get; set; } = default!;
        public DbSet<RoomHistory> RoomHistories { get; set; } = default!;
        public DbSet<Specialty> Specialty { get; set; } = default!;
        public DbSet<RoomStatus> RoomStatus { get; set; } = default!;
        public DbSet<RoomType> RoomType { get; set; } = default!;
        public DbSet<RoomLocation> RoomLocation { get; set; } = default!;
    }
}
