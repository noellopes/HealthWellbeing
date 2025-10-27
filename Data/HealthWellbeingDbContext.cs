using HealthWellbeing.Models;
using HealthWellBeing.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellbeing.Data
{
    public class HealthWellbeingDbContext : DbContext
    {
        public HealthWellbeingDbContext(DbContextOptions<HealthWellbeingDbContext> options)
            : base(options)
        {
        }

        // === Tabelas principais ===
        public DbSet<Exame> Exames { get; set; } = default!;
        public DbSet<Utente> Utentes { get; set; } = default!;
        public DbSet<ExameTipo> ExameTipos { get; set; } = default!;
        public DbSet<Medicos> Medicos { get; set; } = default!;
        public DbSet<SalaDeExames> SalaDeExame { get; set; } = default!;
        public DbSet<ProfissionalExecutante> ProfissionaisExecutantes { get; set; } = default!;
        public DbSet<MaterialEquipamentoAssociado> MaterialEquipamentoAssociado { get; set; } = default!;
    }
}
