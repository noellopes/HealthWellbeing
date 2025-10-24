using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Models;

namespace HealthWellbeing.Data
{
    public class HealthWellbeingDbContext : DbContext
    {
        public HealthWellbeingDbContext(DbContextOptions<HealthWellbeingDbContext> options)
            : base(options) { }

        public DbSet<Food> Food { get; set; } = default!;
        public DbSet<FoodCategory> FoodCategory { get; set; } = default!;
        public DbSet<Objective> Objective { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== FOOD =====
            modelBuilder.Entity<Food>()
                .HasIndex(f => new { f.Name, f.FoodCategoryId })   // Unique Name within Category
                .IsUnique();

            modelBuilder.Entity<Food>()
                .Property(f => f.Name).HasMaxLength(120).IsRequired();

            modelBuilder.Entity<Food>()
                .Property(f => f.Description).HasMaxLength(500);

            // Precisões (SQL Server): decimal(6,2) chega para macros; kcal decimal(6,1) opcional
            modelBuilder.Entity<Food>().Property(f => f.KcalPer100g).HasPrecision(6, 1);
            modelBuilder.Entity<Food>().Property(f => f.ProteinPer100g).HasPrecision(6, 2);
            modelBuilder.Entity<Food>().Property(f => f.CarbsPer100g).HasPrecision(6, 2);
            modelBuilder.Entity<Food>().Property(f => f.FatPer100g).HasPrecision(6, 2);

            // Relação opcional com Category
            modelBuilder.Entity<Food>()
                .HasOne(f => f.Category)
                .WithMany(c => c.Foods)
                .HasForeignKey(f => f.FoodCategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
