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

        public DbSet<HealthWellbeing.Models.Alergia> Alergia { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.RestricaoAlimentar> RestricaoAlimentar { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Receita> Receita { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Member> Member { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Client> Client { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Food> Food { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.FoodCategory> FoodCategory { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Goal> Goal { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Portion> Portion { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.NutritionalComponent> NutritionalComponent { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Nutritionist> Nutritionist { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Plan> Plan { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.FoodNutritionalComponent> FoodNutritionalComponent { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.FoodPlan> FoodPlan { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Alergy> Alergy { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.ClientAlergy> ClientAlergy { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.NutritionistClientPlan> NutritionistClientPlan { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.FoodIntake> FoodIntake { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.FoodPlanDay> FoodPlanDay { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.UtenteSaude> UtenteSaude { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Consulta> Consulta { get; set; } = default!;

        public DbSet<HealthWellbeing.Models.Doctor> Doctor { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Specialities> Specialities { get; set; } = default!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<FoodPlanDay>()
                .HasIndex(x => new { x.PlanId, x.Date, x.FoodId })
                .IsUnique();

            modelBuilder.Entity<FoodIntake>()
                .HasIndex(x => new { x.PlanId, x.Date, x.FoodId })
                .IsUnique();

            modelBuilder.Entity<FoodNutritionalComponent>()
                .HasIndex(x => new { x.FoodId, x.NutritionalComponentId })
                .IsUnique();

            foreach (var fk in modelBuilder.Model.GetEntityTypes()
                     .SelectMany(e => e.GetForeignKeys())
                     .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade))
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }


    }
    

}
