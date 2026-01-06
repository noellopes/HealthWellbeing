using HealthWellbeing.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Data
{
    public class HealthWellbeingDbContext : DbContext
    {
        public HealthWellbeingDbContext(DbContextOptions<HealthWellbeingDbContext> options)
            : base(options)
        {
        }

        // ==============================================================================
        // DB SETS
        // ==============================================================================
        public DbSet<Alergia> Alergia { get; set; } = default!;
        public DbSet<RestricaoAlimentar> RestricaoAlimentar { get; set; } = default!;
        public DbSet<Receita> Receita { get; set; } = default!;
        public DbSet<Exercicio> Exercicio { get; set; } = default!;
        public DbSet<TipoExercicio> TipoExercicio { get; set; } = default!;
        public DbSet<Beneficio> Beneficio { get; set; } = default!;
        public DbSet<TipoExercicioBeneficio> TipoExercicioBeneficio { get; set; } = default!;
        public DbSet<ProblemaSaude> ProblemaSaude { get; set; } = default!;
        public DbSet<TipoExercicioProblemaSaude> TipoExercicioProblemaSaude { get; set; } = default!;
        public DbSet<Musculo> Musculo { get; set; } = default!;
        public DbSet<GrupoMuscular> GrupoMuscular { get; set; } = default!;
        public DbSet<Genero> Genero { get; set; } = default!;
        public DbSet<HistoricoAtividade> HistoricoAtividades { get; set; } = default!;
        public DbSet<PlanoExercicios> PlanoExercicios { get; set; } = default!;
        public DbSet<Equipamento> Equipamento { get; set; } = default!;
        public DbSet<ProfissionalExecutante> ProfissionalExecutante { get; set; } = default!;
        public DbSet<UtenteGrupo7> UtenteGrupo7 { get; set; } = default!;
        public DbSet<Sono> Sono { get; set; } = default!;
        public DbSet<ExercicioProblemaSaude> ExercicioProblemaSaude { get; set; } = default!;
        public DbSet<AvaliacaoFisica> AvaliacaoFisica { get; set; } = default!;
        public DbSet<Profissional> Profissional { get; set; } = default!;
        public DbSet<ObjetivoFisico> ObjetivoFisico { get; set; } = default!;
        public DbSet<UtenteGrupo7ProblemaSaude> UtenteProblemaSaude { get; set; } = default!;
        public DbSet<LimitacaoMedica> LimitacaoMedica { get; set; } = default!;

        // ==============================================================================
        // CONFIGURAÇÕES DE RELAÇÕES (FLUENT API)
        // ==============================================================================
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Exercicio <-> Genero
            modelBuilder.Entity<ExercicioGenero>()
                .HasKey(eg => new { eg.ExercicioId, eg.GeneroId });

            modelBuilder.Entity<ExercicioGenero>()
                .HasOne(eg => eg.Exercicio)
                .WithMany(e => e.ExercicioGeneros)
                .HasForeignKey(eg => eg.ExercicioId)
                .OnDelete(DeleteBehavior.Cascade); // Apagar Exercicio = Limpa link

            modelBuilder.Entity<ExercicioGenero>()
                .HasOne(eg => eg.Genero)
                .WithMany(g => g.ExercicioGeneros)
                .HasForeignKey(eg => eg.GeneroId)
                .OnDelete(DeleteBehavior.Restrict); // Apagar Genero = Bloqueia se usado

            // Exercicio <-> Grupo Muscular
            modelBuilder.Entity<ExercicioGrupoMuscular>()
                .HasKey(egm => new { egm.ExercicioId, egm.GrupoMuscularId });

            modelBuilder.Entity<ExercicioGrupoMuscular>()
                .HasOne(egm => egm.Exercicio)
                .WithMany(e => e.ExercicioGrupoMusculares)
                .HasForeignKey(egm => egm.ExercicioId)
                .OnDelete(DeleteBehavior.Cascade); // Apagar Exercicio = Limpa link

            modelBuilder.Entity<ExercicioGrupoMuscular>()
                .HasOne(egm => egm.GrupoMuscular)
                .WithMany(gm => gm.ExercicioGrupoMusculares)
                .HasForeignKey(egm => egm.GrupoMuscularId)
                .OnDelete(DeleteBehavior.Restrict); // Apagar Grupo = Bloqueia se usado

            // Exercicio <-> Equipamento
            modelBuilder.Entity<ExercicioEquipamento>()
                .HasKey(eg => new { eg.ExercicioId, eg.EquipamentoId });

            modelBuilder.Entity<ExercicioEquipamento>()
                .HasOne(eg => eg.Exercicio)
                .WithMany(e => e.ExercicioEquipamentos)
                .HasForeignKey(eg => eg.ExercicioId)
                .OnDelete(DeleteBehavior.Cascade); // Apagar Exercicio = Limpa link

            modelBuilder.Entity<ExercicioEquipamento>()
                .HasOne(eg => eg.Equipamento)
                .WithMany(g => g.ExercicioEquipamentos)
                .HasForeignKey(eg => eg.EquipamentoId)
                .OnDelete(DeleteBehavior.Restrict); // Apagar Equipamento = Bloqueia se usado

            // Exercicio <-> Problema Saude (Contraindicações)
            modelBuilder.Entity<ExercicioProblemaSaude>()
               .HasKey(ep => new { ep.ExercicioId, ep.ProblemaSaudeId });

            modelBuilder.Entity<ExercicioProblemaSaude>()
                .HasOne(ep => ep.Exercicio)
                .WithMany(e => e.Contraindicacoes)
                .HasForeignKey(ep => ep.ExercicioId)
                .OnDelete(DeleteBehavior.Cascade); // Apagar Exercicio = Limpa link

            modelBuilder.Entity<ExercicioProblemaSaude>()
                .HasOne(ep => ep.ProblemaSaude)
                .WithMany(p => p.ExercicioAfetado)
                .HasForeignKey(ep => ep.ProblemaSaudeId)
                .OnDelete(DeleteBehavior.Restrict); // Apagar Problema = Bloqueia se usado

            // Exercicio <-> Tipo Exercicio
            modelBuilder.Entity<ExercicioTipoExercicio>()
                .HasKey(et => new { et.ExercicioId, et.TipoExercicioId });

            modelBuilder.Entity<ExercicioTipoExercicio>()
                .HasOne(et => et.Exercicio)
                .WithMany(e => e.ExercicioTipoExercicios)
                .HasForeignKey(et => et.ExercicioId)
                .OnDelete(DeleteBehavior.Cascade); // Apagar Exercicio = Limpa link

            modelBuilder.Entity<ExercicioTipoExercicio>()
                .HasOne(et => et.TipoExercicio)
                .WithMany(t => t.ExercicioTipoExercicios)
                .HasForeignKey(et => et.TipoExercicioId)
                .OnDelete(DeleteBehavior.Restrict); // Apagar Tipo = Bloqueia se usado

            // Exercicio <-> Objetivo Fisico
            modelBuilder.Entity<ExercicioObjetivoFisico>()
                .HasKey(eo => new { eo.ExercicioId, eo.ObjetivoFisicoId });

            modelBuilder.Entity<ExercicioObjetivoFisico>()
                .HasOne(eo => eo.Exercicio)
                .WithMany(e => e.ExercicioObjetivos)
                .HasForeignKey(eo => eo.ExercicioId)
                .OnDelete(DeleteBehavior.Cascade); // Apagar Exercicio = Limpa link

            modelBuilder.Entity<ExercicioObjetivoFisico>()
                .HasOne(eo => eo.ObjetivoFisico)
                .WithMany(o => o.ExercicioObjetivos)
                .HasForeignKey(eo => eo.ObjetivoFisicoId)
                .OnDelete(DeleteBehavior.Restrict); // Apagar Objetivo = Bloqueia se usado

            modelBuilder.Entity<PlanoExercicioExercicio>()
                .HasKey(pe => new { pe.PlanoExerciciosId, pe.ExercicioId });

            modelBuilder.Entity<PlanoExercicioExercicio>()
                .HasOne(pe => pe.PlanoExercicios)
                .WithMany(p => p.PlanoExercicioExercicios)
                .HasForeignKey(pe => pe.PlanoExerciciosId)
                .OnDelete(DeleteBehavior.Cascade); // Apagar o Plano apaga as linhas da tabela

            modelBuilder.Entity<PlanoExercicioExercicio>()
                .HasOne(pe => pe.Exercicio)
                .WithMany(e => e.PlanoExercicioExercicios)
                .HasForeignKey(pe => pe.ExercicioId)
                .OnDelete(DeleteBehavior.Restrict); // Não deixa apagar Exercicio se estiver num Plano

            // Grupo Muscular PK
            modelBuilder.Entity<GrupoMuscular>()
                .HasKey(g => g.GrupoMuscularId);

            // Musculo -> GrupoMuscular
            modelBuilder.Entity<Musculo>()
                .HasOne(m => m.GrupoMuscular)
                .WithMany(g => g.Musculos)
                .HasForeignKey(m => m.GrupoMuscularId)
                .OnDelete(DeleteBehavior.NoAction);

            // TipoExercicioBeneficio
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

            // TipoExercicioProblemaSaude
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

            // ObjetivoTipoExercicio
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

            // UtenteGrupo7ProblemaSaude
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
        }
    }
}