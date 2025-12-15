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

        public DbSet<EventType> EventType { get; set; } = default!;
        public DbSet<Level> Level { get; set; } = default!;
        public DbSet<Event> Event { get; set; } = default!;
        public DbSet<Activity_> Activity { get; set; } = default!;
        public DbSet<Alergia> Alergia { get; set; } = default!;
        public DbSet<RestricaoAlimentar> RestricaoAlimentar { get; set; } = default!;
        public DbSet<Receita> Receita { get; set; } = default!;
        public DbSet<UtenteSaude> UtenteSaude { get; set; } = default!;
        public DbSet<Consulta> Consulta { get; set; } = default!;
        public DbSet<Doctor> Doctor { get; set; } = default!;
        public DbSet<Specialities> Specialities { get; set; } = default!;
        public DbSet<Member> Member { get; set; } = default!;
        public DbSet<Client> Client { get; set; } = default!;
        public DbSet<TrainingType> TrainingType { get; set; } = default!;
        public DbSet<Plan> Plan { get; set; } = default!;
        public DbSet<Trainer> Trainer { get; set; } = default!;
        public DbSet<Training> Training { get; set; } = default!;
        public DbSet<Exercicio> Exercicio { get; set; } = default!;
        public DbSet<TipoExercicio> TipoExercicio { get; set; } = default!;
        public DbSet<Beneficio> Beneficio { get; set; } = default!;
        public DbSet<ProblemaSaude> ProblemaSaude { get; set; } = default!;
        public DbSet<Musculo> Musculo { get; set; } = default!;
        public DbSet<GrupoMuscular> GrupoMuscular { get; set; } = default!;
        public DbSet<Genero> Genero { get; set; } = default!;
        public DbSet<ProfissionalExecutante> ProfissionalExecutante { get; set; } = default!;
        public DbSet<AgendaMedica> AgendaMedica { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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

            modelBuilder.Entity<AgendaMedica>()
                .HasOne(a => a.Medico)
                .WithMany(d => d.AgendaMedica)
                .HasForeignKey(a => a.IdMedico)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
