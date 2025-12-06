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
        public HealthWellbeingDbContext (DbContextOptions<HealthWellbeingDbContext> options)
            : base(options)
        {
        }
        public DbSet<HealthWellbeing.Models.Alergia> Alergia { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.RestricaoAlimentar> RestricaoAlimentar { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Receita> Receita { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.UtenteSaude> UtenteSaude { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Consulta> Consulta { get; set; } = default!;

        public DbSet<HealthWellbeing.Models.Doctor> Doctor{ get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Specialities> Specialities { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.AgendaMedica> AgendaMedica { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Doctor → Specialities (1 especialidade, muitos médicos)
            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.Especialidade)
                .WithMany(s => s.Medicos)
                .HasForeignKey(d => d.IdEspecialidade)
                .OnDelete(DeleteBehavior.Restrict); // sem cascade

            // Consulta → Doctor (muitas consultas para 1 médico)
            modelBuilder.Entity<Consulta>()
                .HasOne(c => c.Doctor)
                .WithMany(d => d.Consultas)
                .HasForeignKey(c => c.IdMedico)
                .OnDelete(DeleteBehavior.Restrict);

            // Consulta → Specialities (muitas consultas para 1 especialidade)
            modelBuilder.Entity<Consulta>()
                .HasOne(c => c.Speciality)
                .WithMany(s => s.Consultas)
                .HasForeignKey(c => c.IdEspecialidade)
                .OnDelete(DeleteBehavior.Restrict);

            // AgendaMedica → Doctor
            modelBuilder.Entity<AgendaMedica>()
                .HasOne(a => a.Medico)
                .WithMany(d => d.AgendaMedica)
                .HasForeignKey(a => a.IdMedico)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
