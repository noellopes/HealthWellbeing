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
            : base(options) { }

        public DbSet<Food> Food { get; set; } = default!;
        public DbSet<FoodCategory> FoodCategory { get; set; } = default!;
        public DbSet<Objective> Objective { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique constraint: Name + Category
            modelBuilder.Entity<Objective>()
                .HasIndex(o => new { o.Name, o.Category })
                .IsUnique();

            // Optional explicit column sizes (mirror DataAnnotations)
            modelBuilder.Entity<Objective>()
                .Property(o => o.Name).HasMaxLength(120).IsRequired();

            modelBuilder.Entity<Objective>()
                .Property(o => o.Category).HasMaxLength(50).IsRequired();

            modelBuilder.Entity<Objective>()
                .Property(o => o.Details).HasMaxLength(500);
        }
    }
}

