using System.Linq;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Models;

namespace HealthWellbeing.Data
{
    public class HealthWellbeingDbContext : DbContext
    {
        public HealthWellbeingDbContext(DbContextOptions<HealthWellbeingDbContext> options)
            : base(options) { }

        // -------------------------
        // PRINCIPAIS
        // -------------------------
        public DbSet<Member> Member { get; set; } = default!;
        public DbSet<Client> Client { get; set; } = default!;
        public DbSet<UtenteSaude> UtenteSaude { get; set; } = default!;
        public DbSet<Consulta> Consulta { get; set; } = default!;
        public DbSet<Doctor> Doctor { get; set; } = default!;
        public DbSet<Specialities> Specialities { get; set; } = default!;

        // -------------------------
        // NUTRIÇÃO / PLANOS
        // -------------------------
        public DbSet<Goal> Goal { get; set; } = default!;
        public DbSet<Plan> Plan { get; set; } = default!;
        public DbSet<Nutritionist> Nutritionist { get; set; } = default!;
        public DbSet<NutritionistClientPlan> NutritionistClientPlan { get; set; } = default!;

        public DbSet<FoodCategory> FoodCategory { get; set; } = default!;
        public DbSet<Food> Food { get; set; } = default!;
        public DbSet<Portion> Portion { get; set; } = default!;
        public DbSet<NutritionalComponent> NutritionalComponent { get; set; } = default!;
        public DbSet<FoodNutritionalComponent> FoodNutritionalComponent { get; set; } = default!;

        public DbSet<FoodPlan> FoodPlan { get; set; } = default!;
        public DbSet<FoodPlanDay> FoodPlanDay { get; set; } = default!;
        public DbSet<FoodIntake> FoodIntake { get; set; } = default!;

        // -------------------------
        // ALERGIAS
        // -------------------------
        public DbSet<Alergy> Alergy { get; set; } = default!;
        public DbSet<ClientAlergy> ClientAlergy { get; set; } = default!;

        // -------------------------
        // RECEITAS (se usares mesmo)
        // -------------------------
        public DbSet<Receita> Receita { get; set; } = default!;
        public DbSet<RestricaoAlimentar> RestricaoAlimentar { get; set; } = default!;
        public DbSet<Alergia> Alergia { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // -------------------------
            // ÍNDICES ÚNICOS
            // -------------------------
            modelBuilder.Entity<FoodPlanDay>()
                .HasIndex(x => new { x.PlanId, x.Date, x.FoodId })
                .IsUnique();

            modelBuilder.Entity<FoodIntake>()
                .HasIndex(x => new { x.PlanId, x.Date, x.FoodId })
                .IsUnique();

            modelBuilder.Entity<FoodNutritionalComponent>()
                .HasIndex(x => new { x.FoodId, x.NutritionalComponentId })
                .IsUnique();

            // -------------------------
            // RELAÇÃO 1-para-1: Client <-> UtenteSaude
            // -------------------------
            modelBuilder.Entity<UtenteSaude>()
                .HasOne(u => u.Client)
                .WithOne(c => c.UtenteSaude)
                .HasForeignKey<UtenteSaude>(u => u.ClientId)
                .OnDelete(DeleteBehavior.Restrict); // evita cascatas em cadeia

            // -------------------------
            // DECIMAIS DA RECEITA (remove warnings)
            // Ajusta as propriedades ao teu model Receita real!
            // -------------------------
            modelBuilder.Entity<Receita>(entity =>
            {
                entity.Property(e => e.CaloriasPorPorcao).HasPrecision(10, 2);
                entity.Property(e => e.Gorduras).HasPrecision(10, 2);
                entity.Property(e => e.HidratosCarbono).HasPrecision(10, 2);
                entity.Property(e => e.Proteinas).HasPrecision(10, 2);
            });

            // -------------------------
            // Regra global: evitar Cascade (MAS sem contrariar configurações explícitas)
            // -------------------------
            foreach (var fk in modelBuilder.Model.GetEntityTypes()
                     .SelectMany(e => e.GetForeignKeys())
                     .Where(fk => !fk.IsOwnership))
            {
                // Se já estiver definido explicitamente, respeita.
                // Só mudamos CASCADE para RESTRICT.
                if (fk.DeleteBehavior == DeleteBehavior.Cascade)
                    fk.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
