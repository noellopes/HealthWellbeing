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
        public DbSet<ExameTipo> ExameTipo { get; set; } = default!;
        public DbSet<Medicos> Medicos { get; set; } = default!;
        public DbSet<SalaDeExames> SalaDeExame { get; set; } = default!;
        public DbSet<ProfissionalExecutante> ProfissionalExecutante { get; set; } = default!;
        public DbSet<MaterialEquipamentoAssociado> MaterialEquipamentoAssociado { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seeding para a classe ExameTipo (sua responsabilidade)
            modelBuilder.Entity<ExameTipo>().HasData(
                new ExameTipo
                {
                    ExameTipoId = 1,
                    Nome = "Análise de Sangue Completa",
                    Descricao = "Exame laboratorial de rotina para avaliação hematológica.",
                    Especialidade = "Hematologia",
                    
                },
                new ExameTipo
                {
                    ExameTipoId = 2,
                    Nome = "Ressonância Magnética",
                    Descricao = "Exame de imagem detalhado para estruturas internas.",
                    Especialidade = "Radiologia",
                    
                },
                new ExameTipo
                {
                    ExameTipoId = 3,
                    Nome = "Eletrocardiograma (ECG)",
                    Descricao = "Avaliação da atividade elétrica do coração.",
                    Especialidade = "Cardiologia",
                    
                },
                new ExameTipo
                {
                    ExameTipoId = 4,
                    Nome = "Tomografia Computorizada (TAC)",
                    Descricao = "Processamento de imagens por computador para criar visões transversais do corpo.",
                    Especialidade = "Radiologia",
                    
                },
                new ExameTipo
                {
                    ExameTipoId = 5,
                    Nome = "Ecografia Abdominal",
                    Descricao = "Utiliza ondas sonoras de alta frequência para criar imagens dos órgãos internos.",
                    Especialidade = "Imagiologia",
                    
                },
                new ExameTipo
                {
                    ExameTipoId = 6,
                    Nome = "Teste de Esforço Cardíaco",
                    Descricao = "Monitorização cardíaca durante exercício físico controlado.",
                    Especialidade = "Cardiologia",
                    
                },
                new ExameTipo
                {
                    ExameTipoId = 7,
                    Nome = "Densitometria Óssea",
                    Descricao = "Mede a densidade mineral óssea para diagnosticar osteoporose.",
                    Especialidade = "Reumatologia",
                    
                },
                new ExameTipo
                {
                    ExameTipoId = 8,
                    Nome = "Exame de Urina Tipo II",
                    Descricao = "Análise laboratorial de amostra de urina.",
                    Especialidade = "Urologia",
                    
                }
            );

            
        }
    }
}
