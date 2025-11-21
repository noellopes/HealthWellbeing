using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using HealthWellbeing.Models;
using HealthWellbeing.Utils.Group1.Services;
using HealthWellbeing.Utils.Group1.Interfaces;

namespace HealthWellbeing.Data
{
    public class HealthWellbeingDbContext : DbContext
    {
        public HealthWellbeingDbContext (DbContextOptions<HealthWellbeingDbContext> options)
            : base(options)
        {
        }
        public DbSet<HealthWellbeing.Models.EventType> EventType { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Level> Level { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Event> Event { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Activity_> Activity { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Alergia> Alergia { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.RestricaoAlimentar> RestricaoAlimentar { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Receita> Receita { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.UtenteSaude> UtenteSaude { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Consulta> Consulta { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Doctor> Doctor{ get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Specialities> Specialities { get; set; } = default!;
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
        public DbSet<ProfissionalExecutante> ProfissionalExecutante { get; set; }

        // GROUP 1
        public DbSet<HealthWellbeing.Models.Nurse> Nurse { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Pathology> Pathology { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.TreatmentType> TreatmentType { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.TreatmentRecord> TreatmentRecord { get; set; } = default!;


        // Adiciona capacidade de "Soft Delete" ao contexto
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.AddInterceptors(new SoftDeleteInterceptor());

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Regista um filtro nas query dos modelos que implementam ISoftDeletable
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDeletableEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var method = typeof(ModelBuilder).GetMethods()
                        .First(m => m.Name == "Entity" && m.IsGenericMethod);
                    var genericMethod = method.MakeGenericMethod(entityType.ClrType);
                    dynamic entityBuilder = genericMethod.Invoke(modelBuilder, null);
                    entityBuilder.HasQueryFilter(CreateIsDeletedFilter(entityType.ClrType));
                }
            }

            modelBuilder.Entity<Event>()
               .HasOne(e => e.EventType)
               .WithMany()
               .HasForeignKey(e => e.EventTypeId)
               .OnDelete(DeleteBehavior.NoAction);
        }
        private static LambdaExpression CreateIsDeletedFilter(Type entityType)
        {
            var param = Expression.Parameter(entityType, "x");
            var prop = Expression.Property(param, "IsDeleted");
            var condition = Expression.Equal(prop, Expression.Constant(false));
            return Expression.Lambda(condition, param);
        }
    }
}
