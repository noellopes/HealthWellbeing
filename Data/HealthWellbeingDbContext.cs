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
        public HealthWellbeingDbContext(DbContextOptions<HealthWellbeingDbContext> options) : base(options) { }

        public DbSet<Food> Food { get; set; } = default!;
        public DbSet<FoodCategory> FoodCategory { get; set; } = default!;
        public DbSet<Objective> Objective { get; set; } = default!;
        public DbSet<FoodPortion> FoodPortion { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== Objective =====
            modelBuilder.Entity<Objective>()
                .HasIndex(o => new { o.Name, o.Category })
                .IsUnique();
            modelBuilder.Entity<Objective>().Property(o => o.Name).HasMaxLength(120).IsRequired();
            modelBuilder.Entity<Objective>().Property(o => o.Category).HasMaxLength(50).IsRequired();
            modelBuilder.Entity<Objective>().Property(o => o.Details).HasMaxLength(500);

            // ===== Food =====
            modelBuilder.Entity<Food>()
                .HasIndex(f => new { f.Name, f.FoodCategoryId })
                .IsUnique();
            modelBuilder.Entity<Food>().Property(f => f.Name).HasMaxLength(120).IsRequired();
            modelBuilder.Entity<Food>().Property(f => f.Description).HasMaxLength(500);
            modelBuilder.Entity<Food>().Property(f => f.KcalPer100g).HasPrecision(6, 1);
            modelBuilder.Entity<Food>().Property(f => f.ProteinPer100g).HasPrecision(6, 2);
            modelBuilder.Entity<Food>().Property(f => f.CarbsPer100g).HasPrecision(6, 2);
            modelBuilder.Entity<Food>().Property(f => f.FatPer100g).HasPrecision(6, 2);

            modelBuilder.Entity<Food>()
                .HasOne(f => f.Category)
                .WithMany(c => c.Foods)
                .HasForeignKey(f => f.FoodCategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            // ===== FoodCategory =====
            modelBuilder.Entity<FoodCategory>().Property(c => c.Name).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<FoodCategory>().Property(c => c.Description).HasMaxLength(250);

            // ===== FoodPortion =====
            modelBuilder.Entity<FoodPortion>()
                .HasIndex(p => new { p.FoodName, p.Amount })
                .IsUnique();
            modelBuilder.Entity<FoodPortion>().Property(p => p.FoodName).HasMaxLength(60).IsRequired();
            modelBuilder.Entity<FoodPortion>().Property(p => p.Amount).HasMaxLength(30).IsRequired();
        }
    }
}
