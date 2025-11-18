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

            // Seeding para a classe ExameTipo
            modelBuilder.Entity<ExameTipo>().HasData(
                new ExameTipo { ExameTipoId = 1, Nome = "Análise de Sangue Completa", Descricao = "Exame laboratorial de rotina para avaliação hematológica.", Especialidade = "Hematologia" },
                new ExameTipo { ExameTipoId = 2, Nome = "Ressonância Magnética", Descricao = "Exame de imagem detalhado para estruturas internas.", Especialidade = "Radiologia" },
                new ExameTipo { ExameTipoId = 3, Nome = "Eletrocardiograma (ECG)", Descricao = "Avaliação da atividade elétrica do coração.", Especialidade = "Cardiologia" },
                new ExameTipo { ExameTipoId = 4, Nome = "Tomografia Computorizada (TAC)", Descricao = "Processamento de imagens por computador para criar visões transversais do corpo.", Especialidade = "Radiologia" },
                new ExameTipo { ExameTipoId = 5, Nome = "Ecografia Abdominal", Descricao = "Utiliza ondas sonoras de alta frequência para criar imagens dos órgãos internos.", Especialidade = "Imagiologia" },
                new ExameTipo { ExameTipoId = 6, Nome = "Teste de Esforço Cardíaco", Descricao = "Monitorização cardíaca durante exercício físico controlado.", Especialidade = "Cardiologia" },
                new ExameTipo { ExameTipoId = 7, Nome = "Densitometria Óssea", Descricao = "Mede a densidade mineral óssea para diagnosticar osteoporose.", Especialidade = "Reumatologia" },
                new ExameTipo { ExameTipoId = 8, Nome = "Exame de Urina Tipo II", Descricao = "Análise laboratorial de amostra de urina.", Especialidade = "Urologia" },
                new ExameTipo { ExameTipoId = 9, Nome = "Colonoscopia", Descricao = "Exame endoscópico para visualização do intestino grosso.", Especialidade = "Gastroenterologia" },
                new ExameTipo { ExameTipoId = 10, Nome = "Endoscopia Digestiva Alta", Descricao = "Exame do esófago, estômago e duodeno.", Especialidade = "Gastroenterologia" },
                new ExameTipo { ExameTipoId = 11, Nome = "Mamografia Digital", Descricao = "Rastreio e diagnóstico de cancro da mama.", Especialidade = "Radiologia" },
                new ExameTipo { ExameTipoId = 12, Nome = "Ecografia Pélvica", Descricao = "Avaliação dos órgãos pélvicos femininos ou masculinos.", Especialidade = "Ginecologia" },
                new ExameTipo { ExameTipoId = 13, Nome = "Prova de Função Respiratória", Descricao = "Avalia a capacidade pulmonar e o fluxo de ar.", Especialidade = "Pneumologia" },
                new ExameTipo { ExameTipoId = 14, Nome = "Holter 24 Horas", Descricao = "Monitorização contínua da atividade elétrica do coração.", Especialidade = "Cardiologia" },
                new ExameTipo { ExameTipoId = 15, Nome = "Análise Hormonal (Tireoide)", Descricao = "Medição dos níveis de hormonas tiroideias no sangue.", Especialidade = "Endocrinologia" },
                new ExameTipo { ExameTipoId = 16, Nome = "Teste de Alergias", Descricao = "Testes cutâneos para identificação de alergénios específicos.", Especialidade = "Imunoalergologia" },
                new ExameTipo { ExameTipoId = 17, Nome = "Eletroencefalograma (EEG)", Descricao = "Registo da atividade elétrica cerebral.", Especialidade = "Neurologia" },
                new ExameTipo { ExameTipoId = 18, Nome = "Angiografia por TC", Descricao = "Visualização detalhada dos vasos sanguíneos.", Especialidade = "Radiologia" },
                new ExameTipo { ExameTipoId = 19, Nome = "Exame Oftalmológico Completo", Descricao = "Avaliação da acuidade visual e pressão intraocular.", Especialidade = "Oftalmologia" },
                new ExameTipo { ExameTipoId = 20, Nome = "Audiograma", Descricao = "Avaliação da capacidade auditiva.", Especialidade = "Otorrinolaringologia" },
                new ExameTipo { ExameTipoId = 21, Nome = "Biopsia de Pele", Descricao = "Colheita de pequena amostra de tecido cutâneo.", Especialidade = "Dermatologia" },
                new ExameTipo { ExameTipoId = 22, Nome = "Análise Toxicológica", Descricao = "Detecção e quantificação de substâncias químicas no organismo.", Especialidade = "Medicina Legal" },
                new ExameTipo { ExameTipoId = 23, Nome = "Cultura de Urina", Descricao = "Identificação de bactérias que podem causar infeção urinária.", Especialidade = "Urologia" }

                );


        }
    }
}
