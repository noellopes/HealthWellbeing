using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace HealthWellbeing.Data
{
    public class HealthWellbeingDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public HealthWellbeingDbContext(DbContextOptions<HealthWellbeingDbContext> options)
            : base(options)
        {
        }

        // Tabelas
        public DbSet<Alergia> Alergia { get; set; } = default!;
        public DbSet<Alimento> Alimentos { get; set; } = default!;
        public DbSet<AlergiaAlimento> AlergiaAlimento { get; set; }
        public DbSet<ClientAlergia> ClientAlergia { get; set; }
        public DbSet<ClientRestricao> ClientRestricao { get; set; }
        public DbSet<AlimentoSubstituto> AlimentoSubstitutos { get; set; } = default!;
        public DbSet<RestricaoAlimentar> RestricaoAlimentar { get; set; } = default!;
        public DbSet<RestricaoAlimentarAlimento> RestricaoAlimentarAlimento { get; set; }
        public DbSet<ComponenteReceita> ComponenteReceita { get; set; } = default!;
        public DbSet<CategoriaAlimento> CategoriaAlimento { get; set; } = default!;
        public DbSet<Receita> Receita { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.EventType> EventType { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Level> Level { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Event> Event { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Activity_> Activity { get; set; } = default!;
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
        public DbSet<HealthWellbeing.Models.PlanoAlimentar> PlanoAlimentar { get; set; }
        public DbSet<HealthWellbeing.Models.Meta> Meta { get; set; }

        public DbSet<HealthWellbeing.Models.ReceitasParaPlanosAlimentares> ReceitasParaPlanosAlimentares { get; set; }



        public DbSet<ProgressRecord> ProgressRecord { get; set; } = default!;
        public DbSet<MetaCorporal> MetaCorporal { get; set; } = default!;

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

            // IdentityUser (0..1) <-> (0..1) Client
            modelBuilder.Entity<Client>()
                .HasIndex(c => c.IdentityUserId)
                .IsUnique()
                .HasFilter("[IdentityUserId] IS NOT NULL");

            modelBuilder.Entity<Client>()
                .HasOne(c => c.IdentityUser)
                .WithOne()
                .HasForeignKey<Client>(c => c.IdentityUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClientAlergia>()
                .HasKey(aa => new { aa.ClientId, aa.AlergiaId });

            modelBuilder.Entity<ClientAlergia>()
                .HasOne(c => c.Client)
                .WithMany(a => a.Alergias)
                .HasForeignKey(a => a.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClientAlergia>()
                .HasOne(a => a.Alergia)
                .WithMany(c => c.ClientesAssociados)
                .HasForeignKey(a => a.AlergiaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClientRestricao>()
                .HasKey(aa => new { aa.ClientId, aa.RestricaoAlimentarId });

            modelBuilder.Entity<ClientRestricao>()
                .HasOne(c => c.Client)
                .WithMany(a => a.RestricoesAlimentares)
                .HasForeignKey(a => a.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClientRestricao>()
                .HasOne(a => a.RestricaoAlimentar)
                .WithMany(c => c.ClientesAssociados)
                .HasForeignKey(a => a.RestricaoAlimentarId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlanoAlimentar>()
                .HasOne(p => p.Client)
                .WithOne(c => c.PlanoAlimentar)
                .HasForeignKey<PlanoAlimentar>(p => p.ClientId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PlanoAlimentar>()
                .HasOne(p => p.Meta)
                .WithMany()
                .HasForeignKey(p => p.MetaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlanoAlimentar>()
                .HasMany(p => p.Receitas)
                .WithMany(r => r.PlanosAlimentares)
                .UsingEntity<ReceitasParaPlanosAlimentares>(
                    j => j
                        .HasOne(pr => pr.Receita)
                        .WithMany()
                        .HasForeignKey(pr => pr.ReceitaId)
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne(pr => pr.PlanoAlimentar)
                        .WithMany()
                        .HasForeignKey(pr => pr.PlanoAlimentarId)
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey(pr => new { pr.PlanoAlimentarId, pr.ReceitaId });
                        j.ToTable("ReceitasParaPlanosAlimentares");
                    });

            // ProgressRecord relationships
            modelBuilder.Entity<ProgressRecord>()
                .HasOne(p => p.Client)
                .WithMany()
                .HasForeignKey(p => p.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProgressRecord>()
                .HasOne(p => p.Nutritionist)
                .WithMany()
                .HasForeignKey(p => p.NutritionistId)
                .OnDelete(DeleteBehavior.Restrict);

            // MetaCorporal relationships
            modelBuilder.Entity<MetaCorporal>()
                .HasOne(m => m.Client)
                .WithMany()
                .HasForeignKey(m => m.ClientId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
