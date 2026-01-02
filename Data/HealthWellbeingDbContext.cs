using HealthWellbeing.Controllers;
using HealthWellbeing.Models;
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
        public DbSet<HealthWellbeing.Models.Alergia> Alergia { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.RestricaoAlimentar> RestricaoAlimentar { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Receita> Receita { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Exercicio> Exercicio { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.TipoExercicio> TipoExercicio { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Beneficio> Beneficio { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.TipoExercicioBeneficio> TipoExercicioBeneficio { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.ProblemaSaude> ProblemaSaude { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.TipoExercicioProblemaSaude> TipoExercicioProblemaSaude { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Musculo> Musculo { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.GrupoMuscular> GrupoMuscular { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Genero> Genero { get; set; } = default!;
        public DbSet<HistoricoAtividade> HistoricoAtividades { get; set; }
        public DbSet<HealthWellbeing.Models.PlanoExercicios> PlanoExercicios { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Equipamento> Equipamento { get; set; }
        public DbSet<ProfissionalExecutante> ProfissionalExecutante { get; set; }
        public DbSet<HealthWellbeing.Models.UtenteGrupo7> UtenteGrupo7 { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Sono> Sono { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.ExercicioProblemaSaude> ExercicioProblemaSaude { get; set; } = default!;
        public DbSet<AvaliacaoFisica> AvaliacaoFisica { get; set; } = default!;
        public DbSet<Profissional> Profissional { get; set; }
        public DbSet<HealthWellbeing.Models.ObjetivoFisico> ObjetivoFisico { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.UtenteGrupo7ProblemaSaude> UtenteProblemaSaude { get; set; } = default!;
       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Chave primária do GrupoMuscular
            modelBuilder.Entity<GrupoMuscular>()
                .HasKey(g => g.GrupoMuscularId);

            // Relação Musculo → GrupoMuscular
            modelBuilder.Entity<Musculo>()
                .HasOne(m => m.GrupoMuscular)
                .WithMany(g => g.Musculos)
                .HasForeignKey(m => m.GrupoMuscularId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TipoExercicioBeneficio>()
                .HasKey(tb => new { tb.TipoExercicioId, tb.BeneficioId });

            modelBuilder.Entity<TipoExercicioBeneficio>()
                .HasOne(tb => tb.TipoExercicio)
                .WithMany(t => t.TipoExercicioBeneficios)
                .HasForeignKey(tb => tb.TipoExercicioId);

            modelBuilder.Entity<TipoExercicioBeneficio>()
                .HasOne(tb => tb.Beneficio)
                .WithMany(b => b.TipoExercicioBeneficios)
                .HasForeignKey(tb => tb.BeneficioId);

            modelBuilder.Entity<TipoExercicioProblemaSaude>()
                .HasKey(tb => new { tb.TipoExercicioId, tb.ProblemaSaudeId });

            modelBuilder.Entity<TipoExercicioProblemaSaude>()
                .HasOne(tb => tb.TipoExercicio)
                .WithMany(t => t.Contraindicacao)
                .HasForeignKey(tb => tb.TipoExercicioId);

            modelBuilder.Entity<TipoExercicioProblemaSaude>()
                .HasOne(tb => tb.ProblemaSaude)
                .WithMany(b => b.TipoExercicioAfetado)
                .HasForeignKey(tb => tb.ProblemaSaudeId);

            modelBuilder.Entity<ExercicioGenero>()
                .HasKey(eg => new { eg.ExercicioId, eg.GeneroId });

            modelBuilder.Entity<ExercicioGenero>()
                .HasOne(eg => eg.Exercicio)
                .WithMany(e => e.ExercicioGeneros)
                .HasForeignKey(eg => eg.ExercicioId);

            modelBuilder.Entity<ExercicioGenero>()
                .HasOne(eg => eg.Genero)
                .WithMany(g => g.ExercicioGeneros)
                .HasForeignKey(eg => eg.GeneroId);

            modelBuilder.Entity<ExercicioGrupoMuscular>()
                .HasKey(egm => new { egm.ExercicioId, egm.GrupoMuscularId });

            modelBuilder.Entity<ExercicioGrupoMuscular>()
                .HasOne(egm => egm.Exercicio)
                .WithMany(e => e.ExercicioGrupoMusculares)
                .HasForeignKey(egm => egm.ExercicioId);

            modelBuilder.Entity<ExercicioGrupoMuscular>()
                .HasOne(egm => egm.GrupoMuscular)
                .WithMany(gm => gm.ExercicioGrupoMusculares)
                .HasForeignKey(egm => egm.GrupoMuscularId);

            modelBuilder.Entity<ExercicioEquipamento>()
                .HasKey(eg => new { eg.ExercicioId, eg.EquipamentoId });

            modelBuilder.Entity<ExercicioEquipamento>()
                .HasOne(eg => eg.Exercicio)
                .WithMany(e => e.ExercicioEquipamentos)
                .HasForeignKey(eg => eg.ExercicioId);

            modelBuilder.Entity<ExercicioEquipamento>()
                .HasOne(eg => eg.Equipamento)
                .WithMany(g => g.ExercicioEquipamentos)
                .HasForeignKey(eg => eg.EquipamentoId);

            modelBuilder.Entity<ObjetivoTipoExercicio>()
                .HasKey(ot => new { ot.ObjetivoFisicoId, ot.TipoExercicioId });

            modelBuilder.Entity<ObjetivoTipoExercicio>()
                .HasOne(ot => ot.ObjetivoFisico)
                .WithMany(o => o.ObjetivoTipoExercicio)
                .HasForeignKey(ot => ot.ObjetivoFisicoId);

            modelBuilder.Entity<ObjetivoTipoExercicio>()
                .HasOne(ot => ot.TipoExercicio)
                .WithMany(t => t.ObjetivoTipoExercicio)
                .HasForeignKey(ot => ot.TipoExercicioId);

            modelBuilder.Entity<ExercicioProblemaSaude>()
               .HasKey(ep => new { ep.ExercicioId, ep.ProblemaSaudeId });

            modelBuilder.Entity<ExercicioProblemaSaude>()
                .HasOne(ep => ep.Exercicio)
                .WithMany(e => e.Contraindicacoes)
                .HasForeignKey(ep => ep.ExercicioId);

            modelBuilder.Entity<ExercicioProblemaSaude>()
                .HasOne(ep => ep.ProblemaSaude)
                .WithMany(p => p.ExercicioAfetado)
                .HasForeignKey(ep => ep.ProblemaSaudeId);

            modelBuilder.Entity<UtenteGrupo7ProblemaSaude>()
                .HasKey(up => new { up.UtenteGrupo7Id, up.ProblemaSaudeId });

            modelBuilder.Entity<UtenteGrupo7ProblemaSaude>()
                .HasOne(up => up.Utente)
                .WithMany(u => u.UtenteProblemasSaude)
                .HasForeignKey(up => up.UtenteGrupo7Id);

            modelBuilder.Entity<UtenteGrupo7ProblemaSaude>()
                .HasOne(up => up.ProblemaSaude)
                .WithMany(p => p.UtenteProblemasSaude)
                .HasForeignKey(up => up.ProblemaSaudeId);

            modelBuilder.Entity<ExercicioTipoExercicio>()
        .HasKey(et => new { et.ExercicioId, et.TipoExercicioId });

            modelBuilder.Entity<ExercicioTipoExercicio>()
                .HasOne(et => et.Exercicio)
                .WithMany(e => e.ExercicioTipoExercicios)
                .HasForeignKey(et => et.ExercicioId);

            modelBuilder.Entity<ExercicioTipoExercicio>()
                .HasOne(et => et.TipoExercicio)
                .WithMany(t => t.ExercicioTipoExercicios)
                .HasForeignKey(et => et.TipoExercicioId);

            modelBuilder.Entity<ExercicioObjetivoFisico>()
                .HasKey(eo => new { eo.ExercicioId, eo.ObjetivoFisicoId });

            modelBuilder.Entity<ExercicioObjetivoFisico>()
                .HasOne(eo => eo.Exercicio)
                .WithMany(e => e.ExercicioObjetivos)
                .HasForeignKey(eo => eo.ExercicioId);

            modelBuilder.Entity<ExercicioObjetivoFisico>()
                .HasOne(eo => eo.ObjetivoFisico)
                .WithMany(o => o.ExercicioObjetivos)
                .HasForeignKey(eo => eo.ObjetivoFisicoId);

            modelBuilder.Entity<PlanoExercicioExercicio>()
                .HasKey(pe => new { pe.PlanoExerciciosId, pe.ExercicioId });

            modelBuilder.Entity<PlanoExercicioExercicio>()
                .HasOne(pe => pe.PlanoExercicios)
                .WithMany(p => p.PlanoExercicioExercicios)
                .HasForeignKey(pe => pe.PlanoExerciciosId);

            modelBuilder.Entity<PlanoExercicioExercicio>()
                .HasOne(pe => pe.Exercicio)
                .WithMany(e => e.PlanoExercicioExercicios)
                .HasForeignKey(pe => pe.ExercicioId);

        }
        public DbSet<HealthWellbeing.Models.LimitacaoMedica> LimitacaoMedica { get; set; } = default!;
    }
}
