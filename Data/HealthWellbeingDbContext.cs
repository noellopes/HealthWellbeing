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
        public DbSet<AgendaMedica> AgendaMedica { get; set; } = default!;
        public DbSet<DoctorConsulta> DoctorConsulta { get; set; } = default!;
        public DbSet<ConsultaUtente> ConsultaUtente { get; set; } = default!;

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
        // EVENTOS / GAMIFICAÇÃO
        // -------------------------
        public DbSet<EventType> EventType { get; set; } = default!;
        public DbSet<Level> Level { get; set; } = default!;
        public DbSet<Event> Event { get; set; } = default!;
        public DbSet<Activity_> Activity { get; set; } = default!;

        // -------------------------
        // TREINO
        // -------------------------
        public DbSet<TrainingType> TrainingType { get; set; } = default!;
        public DbSet<Trainer> Trainer { get; set; } = default!;
        public DbSet<Training> Training { get; set; } = default!;

        // -------------------------
        // OUTROS
        // -------------------------
        public DbSet<Receita> Receita { get; set; } = default!;
        public DbSet<RestricaoAlimentar> RestricaoAlimentar { get; set; } = default!;
        public DbSet<Alergia> Alergia { get; set; } = default!;
        public DbSet<Exercicio> Exercicio { get; set; } = default!;
        public DbSet<TipoExercicio> TipoExercicio { get; set; } = default!;
        public DbSet<Beneficio> Beneficio { get; set; } = default!;
        public DbSet<ProblemaSaude> ProblemaSaude { get; set; } = default!;
        public DbSet<Musculo> Musculo { get; set; } = default!;
        public DbSet<GrupoMuscular> GrupoMuscular { get; set; } = default!;
        public DbSet<Genero> Genero { get; set; } = default!;
        public DbSet<ProfissionalExecutante> ProfissionalExecutante { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // -------------------------
            // REGRA GLOBAL: evitar Cascade
            // -------------------------
            foreach (var fk in modelBuilder.Model.GetEntityTypes()
                         .SelectMany(e => e.GetForeignKeys())
                         .Where(fk => !fk.IsOwnership))
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict; // em SQL Server vira NO ACTION
            }

            // -------------------------
            // RELAÇÕES IMPORTANTES
            // -------------------------

            modelBuilder.Entity<Event>()
                .HasOne(e => e.EventType)
                .WithMany()
                .HasForeignKey(e => e.EventTypeId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.Especialidade)
                .WithMany(s => s.Medicos)
                .HasForeignKey(d => d.IdEspecialidade)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Consulta>()
                .HasOne(c => c.Doctor)
                .WithMany(d => d.Consultas)
                .HasForeignKey(c => c.IdMedico)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Consulta>()
                .HasOne(c => c.Speciality)
                .WithMany(s => s.Consultas)
                .HasForeignKey(c => c.IdEspecialidade)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ CASCADE: apagar UtenteSaude apaga Consultas associadas
            modelBuilder.Entity<Consulta>()
                .HasOne(c => c.UtenteSaude)
                .WithMany() 
                .HasForeignKey(c => c.IdUtenteSaude)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AgendaMedica>()
                .HasOne(a => a.Medico)
                .WithMany(d => d.AgendaMedica)
                .HasForeignKey(a => a.IdMedico)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------
            // DoctorConsulta (many-to-many)
            // -------------------------
            modelBuilder.Entity<DoctorConsulta>()
                .HasKey(dc => new { dc.IdMedico, dc.IdConsulta });

            modelBuilder.Entity<DoctorConsulta>()
                .HasOne(dc => dc.Medico)
                .WithMany(d => d.DoctorConsultas)
                .HasForeignKey(dc => dc.IdMedico)
                .OnDelete(DeleteBehavior.NoAction);

            // ✅ CASCADE: apagar Consulta apaga linhas DoctorConsulta
            modelBuilder.Entity<DoctorConsulta>()
                .HasOne(dc => dc.Consulta)
                .WithMany(c => c.ConsultaDoctors)
                .HasForeignKey(dc => dc.IdConsulta)
                .OnDelete(DeleteBehavior.NoAction);

            // -------------------------
            // ConsultaUtente (many-to-many)
            // -------------------------
            modelBuilder.Entity<ConsultaUtente>()
                .HasKey(cu => new { cu.IdConsulta, cu.IdUtente });

            // ✅ CASCADE: apagar Consulta apaga linhas ConsultaUtente
            modelBuilder.Entity<ConsultaUtente>()
                .HasOne(cu => cu.Consulta)
                .WithMany(c => c.ConsultaUtentes)
                .HasForeignKey(cu => cu.IdConsulta)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ConsultaUtente>()
                .HasOne(cu => cu.Utente)
                .WithMany(u => u.UtenteConsultas)
                .HasForeignKey(cu => cu.IdUtente)
                .OnDelete(DeleteBehavior.NoAction);

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
            // 1-para-1: Client <-> UtenteSaude
            // -------------------------
            modelBuilder.Entity<UtenteSaude>()
                .HasOne(u => u.Client)
                .WithOne(c => c.UtenteSaude)
                .HasForeignKey<UtenteSaude>(u => u.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UtenteSaude>()
                .HasIndex(u => u.ClientId)
                .IsUnique();

            // -------------------------
            // DECIMAIS (Receita)
            // -------------------------
            modelBuilder.Entity<Receita>(entity =>
            {
                entity.Property(e => e.CaloriasPorPorcao).HasPrecision(10, 2);
                entity.Property(e => e.Gorduras).HasPrecision(10, 2);
                entity.Property(e => e.HidratosCarbono).HasPrecision(10, 2);
                entity.Property(e => e.Proteinas).HasPrecision(10, 2);
            });
        }
    }
}
