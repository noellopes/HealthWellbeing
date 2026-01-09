using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Models;

namespace HealthWellbeing.Data
{
    public class HealthWellbeingDbContext : DbContext
    {
        public HealthWellbeingDbContext(DbContextOptions<HealthWellbeingDbContext> options)
            : base(options)
        {
        }

        // Tabelas
        public DbSet<HealthWellbeing.Models.Alergia> Alergia { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Alimento> Alimentos { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.AlergiaAlimento> AlergiaAlimento { get; set; }
        public DbSet<HealthWellbeing.Models.AlimentoSubstituto> AlimentoSubstitutos { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.RestricaoAlimentar> RestricaoAlimentar { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.RestricaoAlimentarAlimento> RestricaoAlimentarAlimento { get; set; }
        public DbSet<HealthWellbeing.Models.ComponenteReceita> ComponenteReceita { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.CategoriaAlimento> CategoriaAlimento { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Receita> Receita { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.EventType> EventType { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Level> Level { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Event> Event { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Activity_> Activity { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.UtenteSaude> UtenteSaude { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Consulta> Consulta { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Doctor> Doctor { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Specialities> Specialities { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.RestricaoAlimentar> RestricaoAlimentarId { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Member> Member { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Client> Client { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.TrainingType> TrainingType { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Plan> Plan { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Trainer> Trainer { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Training> Training { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Exercicio> Exercicio { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.TipoExercicio> TipoExercicio { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Beneficio> Beneficio { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.ProblemaSaude> ProblemaSaude { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Musculo> Musculo { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.GrupoMuscular> GrupoMuscular { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Genero> Genero { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.ProfissionalExecutante> ProfissionalExecutante { get; set; }
        public DbSet<HealthWellbeing.Models.Food> Food { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.FoodCategory> FoodCategory { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Goal> Goal { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Portion> Portion { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.NutritionalComponent> NutritionalComponent { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Nutritionist> Nutritionist { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.FoodHabitsPlan> FoodHabitsPlan { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.FoodNutritionalComponent> FoodNutritionalComponent { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.PlanFood> FoodPlan { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Alergy> Alergy { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.ClientAlergy> ClientAlergy { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.NutritionistClientPlan> NutritionistClientPlan { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.FoodIntake> FoodIntake { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.FoodPlanDay> FoodPlanDay { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Event>()
                .HasOne(e => e.EventType)
                .WithMany()
                .HasForeignKey(e => e.EventTypeId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configuração da relação auto-referenciada Alimento ↔ AlimentoSubstituto

            modelBuilder.Entity<AlimentoSubstituto>()
                .HasOne(a => a.AlimentoOriginal)
                .WithMany(a => a.Substitutos)
                .HasForeignKey(a => a.AlimentoOriginalId)
                .OnDelete(DeleteBehavior.Restrict); // evita exclusão em cascata

            modelBuilder.Entity<AlimentoSubstituto>()
                .HasOne(a => a.AlimentoSubstitutoRef)
                .WithMany(a => a.SubstituidoPor)
                .HasForeignKey(a => a.AlimentoSubstitutoRefId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AlergiaAlimento>()
                .HasKey(aa => new { aa.AlergiaId, aa.AlimentoId });

            modelBuilder.Entity<AlergiaAlimento>()
                .HasOne(aa => aa.Alergia)
                .WithMany(a => a.AlimentosAssociados)
                .HasForeignKey(aa => aa.AlergiaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AlergiaAlimento>()
                .HasOne(aa => aa.Alimento)
                .WithMany(al => al.AlergiaRelacionadas)
                .HasForeignKey(aa => aa.AlimentoId)
                .OnDelete(DeleteBehavior.Cascade);


            // N:N RestricaoAlimentar ↔ Alimento
            modelBuilder.Entity<RestricaoAlimentarAlimento>()
                .HasKey(ra => new { ra.RestricaoAlimentarId, ra.AlimentoId });

            modelBuilder.Entity<RestricaoAlimentarAlimento>()
                .HasOne(ra => ra.RestricaoAlimentar)
                .WithMany(r => r.AlimentosAssociados)
                .HasForeignKey(ra => ra.RestricaoAlimentarId);

            modelBuilder.Entity<RestricaoAlimentarAlimento>()
                .HasOne(ra => ra.Alimento)
                .WithMany(a => a.RestricoesAssociadas)
                .HasForeignKey(ra => ra.AlimentoId);

            modelBuilder.Entity<ComponenteReceita>()
                .HasOne(c => c.Receita)
                .WithMany(r => r.Componentes)
                .HasForeignKey(c => c.ReceitaId)
                .OnDelete(DeleteBehavior.Cascade);

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
