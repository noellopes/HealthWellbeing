using HealthWellbeing.Models;
using HealthWellBeing.Models;
using Microsoft.EntityFrameworkCore;
using NuGet.ContentModel;
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
        public DbSet<Especialidade> Especialidades { get; set; } = default!;
        public DbSet<ExameTipoRecurso> ExameTipoRecursos { get; set; } = default!;

        public DbSet<Funcao> Funcoes { get; set; }
        public DbSet<EstadoMaterial> EstadosMaterial{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ========================================================
            // CONFIGURAÇÃO M:N (ExameTipo <-> Recurso)
            // ========================================================

            // 1. Definir Chave Primária Composta
            modelBuilder.Entity<ExameTipoRecurso>()
                .HasKey(etr => new { etr.ExameTipoId, etr.MaterialEquipamentoAssociadoId });

            // 2. Configurar Relação com ExameTipo
            modelBuilder.Entity<ExameTipoRecurso>()
                .HasOne(etr => etr.ExameTipo)
                .WithMany(et => et.ExameTipoRecursos)
                .HasForeignKey(etr => etr.ExameTipoId);

            // 3. Configurar Relação com MaterialEquipamentoAssociado
            modelBuilder.Entity<ExameTipoRecurso>()
                .HasOne(etr => etr.Recurso)
                .WithMany(m => m.ExameTipoRecursos)
                .HasForeignKey(etr => etr.MaterialEquipamentoAssociadoId);

            modelBuilder.Entity<ExameTipo>()
                .HasOne(et => et.Especialidade)      // Um TipoExame tem uma Especialidade
                .WithMany(e => e.TiposExame)         // Uma Especialidade tem muitos TiposExame
                .HasForeignKey(et => et.EspecialidadeId); // Usando a FK definida


            modelBuilder.Entity<ProfissionalExecutante>()
                .HasOne(p => p.Funcao)
                .WithMany()
                .HasForeignKey(p => p.FuncaoId)
                .OnDelete(DeleteBehavior.Restrict);



            // SEEDING NOVO: Especialidades 
            modelBuilder.Entity<Especialidade>().HasData(
                new Especialidade { EspecialidadeId = 1, Nome = "Hematologia" },
                new Especialidade { EspecialidadeId = 2, Nome = "Radiologia" },
                new Especialidade { EspecialidadeId = 3, Nome = "Cardiologia" },
                new Especialidade { EspecialidadeId = 4, Nome = "Imagiologia" },
                new Especialidade { EspecialidadeId = 5, Nome = "Reumatologia" },
                new Especialidade { EspecialidadeId = 6, Nome = "Urologia" },
                new Especialidade { EspecialidadeId = 7, Nome = "Gastroenterologia" },
                new Especialidade { EspecialidadeId = 8, Nome = "Ginecologia" },
                new Especialidade { EspecialidadeId = 9, Nome = "Pneumologia" },
                new Especialidade { EspecialidadeId = 10, Nome = "Endocrinologia" },
                new Especialidade { EspecialidadeId = 11, Nome = "Imunoalergologia" },
                new Especialidade { EspecialidadeId = 12, Nome = "Neurologia" },
                new Especialidade { EspecialidadeId = 13, Nome = "Oftalmologia" },
                new Especialidade { EspecialidadeId = 14, Nome = "Otorrinolaringologia" },
                new Especialidade { EspecialidadeId = 15, Nome = "Dermatologia" },
                new Especialidade { EspecialidadeId = 16, Nome = "Medicina Legal" }
            );

            // Seeding para a classe ExameTipo (Atualizado para usar FK)
            modelBuilder.Entity<ExameTipo>().HasData(
                new ExameTipo { ExameTipoId = 1, Nome = "Análise de Sangue Completa", Descricao = "Exame laboratorial de rotina para avaliação hematológica.", EspecialidadeId = 1 },
                new ExameTipo { ExameTipoId = 2, Nome = "Ressonância Magnética", Descricao = "Exame de imagem detalhado para estruturas internas.", EspecialidadeId = 2 },
                new ExameTipo { ExameTipoId = 3, Nome = "Eletrocardiograma (ECG)", Descricao = "Avaliação da atividade elétrica do coração.", EspecialidadeId = 3 },
                new ExameTipo { ExameTipoId = 4, Nome = "Tomografia Computorizada (TAC)", Descricao = "Processamento de imagens por computador para criar visões transversais do corpo.", EspecialidadeId = 2 },
                new ExameTipo { ExameTipoId = 5, Nome = "Ecografia Abdominal", Descricao = "Utiliza ondas sonoras de alta frequência para criar imagens dos órgãos internos.", EspecialidadeId = 4 },
                new ExameTipo { ExameTipoId = 6, Nome = "Teste de Esforço Cardíaco", Descricao = "Monitorização cardíaca durante exercício físico controlado.", EspecialidadeId = 3 },
                new ExameTipo { ExameTipoId = 7, Nome = "Densitometria Óssea", Descricao = "Mede a densidade mineral óssea para diagnosticar osteoporose.", EspecialidadeId = 5 },
                new ExameTipo { ExameTipoId = 8, Nome = "Exame de Urina Tipo II", Descricao = "Análise laboratorial de amostra de urina.", EspecialidadeId = 6 },
                new ExameTipo { ExameTipoId = 9, Nome = "Colonoscopia", Descricao = "Exame endoscópico para visualização do intestino grosso.", EspecialidadeId = 7 },
                new ExameTipo { ExameTipoId = 10, Nome = "Endoscopia Digestiva Alta", Descricao = "Exame do esófago, estômago e duodeno.", EspecialidadeId = 7 },
                new ExameTipo { ExameTipoId = 11, Nome = "Mamografia Digital", Descricao = "Rastreio e diagnóstico de cancro da mama.", EspecialidadeId = 2 },
                new ExameTipo { ExameTipoId = 12, Nome = "Ecografia Pélvica", Descricao = "Avaliação dos órgãos pélvicos femininos ou masculinos.", EspecialidadeId = 8 },
                new ExameTipo { ExameTipoId = 13, Nome = "Prova de Função Respiratória", Descricao = "Avalia a capacidade pulmonar e o fluxo de ar.", EspecialidadeId = 9 },
                new ExameTipo { ExameTipoId = 14, Nome = "Holter 24 Horas", Descricao = "Monitorização contínua da atividade elétrica do coração.", EspecialidadeId = 3 },
                new ExameTipo { ExameTipoId = 15, Nome = "Análise Hormonal (Tireoide)", Descricao = "Medição dos níveis de hormonas tiroideias no sangue.", EspecialidadeId = 10 },
                new ExameTipo { ExameTipoId = 16, Nome = "Teste de Alergias", Descricao = "Testes cutâneos para identificação de alergénios específicos.", EspecialidadeId = 11 },
                new ExameTipo { ExameTipoId = 17, Nome = "Eletroencefalograma (EEG)", Descricao = "Registo da atividade elétrica cerebral.", EspecialidadeId = 12 },
                new ExameTipo { ExameTipoId = 18, Nome = "Angiografia por TC", Descricao = "Visualização detalhada dos vasos sanguíneos.", EspecialidadeId = 2 },
                new ExameTipo { ExameTipoId = 19, Nome = "Exame Oftalmológico Completo", Descricao = "Avaliação da acuidade visual e pressão intraocular.", EspecialidadeId = 13 },
                new ExameTipo { ExameTipoId = 20, Nome = "Audiograma", Descricao = "Avaliação da capacidade auditiva.", EspecialidadeId = 14 },
                new ExameTipo { ExameTipoId = 21, Nome = "Biopsia de Pele", Descricao = "Colheita de pequena amostra de tecido cutâneo.", EspecialidadeId = 15 },
                new ExameTipo { ExameTipoId = 22, Nome = "Análise Toxicológica", Descricao = "Detecção e quantificação de substâncias químicas no organismo.", EspecialidadeId = 16 },
                new ExameTipo { ExameTipoId = 23, Nome = "Cultura de Urina", Descricao = "Identificação de bactérias que podem causar infeção urinária.", EspecialidadeId = 6 }
            );

            modelBuilder.Entity<EstadoMaterial>().HasData(
                            new EstadoMaterial { MaterialStatusId = 1, Nome = "Disponível", Descricao = "Material pronto para utilização" },
                            new EstadoMaterial { MaterialStatusId = 2, Nome = "Em Uso", Descricao = "Material atualmente utilizado" },
                            new EstadoMaterial { MaterialStatusId = 3, Nome = "Reservado", Descricao = "Material reservado para utilização futura" },
                            new EstadoMaterial { MaterialStatusId = 4, Nome = "Em Manutenção", Descricao = "Material em reparação/manutenção" },
                            new EstadoMaterial { MaterialStatusId = 5, Nome = "Danificado", Descricao = "Material danificado e não utilizável" },
                            new EstadoMaterial { MaterialStatusId = 6, Nome = "Perdido", Descricao = "Material não localizado" }
            );

            modelBuilder.Entity<MaterialEquipamentoAssociado>().HasData(
                // 1
                new MaterialEquipamentoAssociado
                {
                    MaterialEquipamentoAssociadoId = 1, // PK
                    NomeEquipamento = "Seringa Descartável 5ml",
                    Quantidade = 500,
                    MaterialStatusId = 1 // Disponível
                },
                // 2
                new MaterialEquipamentoAssociado
                {
                    MaterialEquipamentoAssociadoId = 2, // PK
                    NomeEquipamento = "Compressa de Gaze Esterilizada",
                    Quantidade = 1200,
                    MaterialStatusId = 1 // Disponível
                },
                // 3
                new MaterialEquipamentoAssociado
                {
                    MaterialEquipamentoAssociadoId = 3, // PK
                    NomeEquipamento = "Monitor de Sinais Vitais",
                    Quantidade = 15,
                    MaterialStatusId = 2 // Em Uso
                },
                // 4
                new MaterialEquipamentoAssociado
                {
                    MaterialEquipamentoAssociadoId = 4, // PK
                    NomeEquipamento = "Eletrocardiógrafo Portátil",
                    Quantidade = 5,
                    MaterialStatusId = 4 // Em Manutenção
                },
                // 5
                new MaterialEquipamentoAssociado
                {
                    MaterialEquipamentoAssociadoId = 5, // PK
                    NomeEquipamento = "Luvas de Nitrilo (Caixa)",
                    Quantidade = 80,
                    MaterialStatusId = 1 // Disponível
                },
                // 6
                new MaterialEquipamentoAssociado
                {
                    MaterialEquipamentoAssociadoId = 6, // PK
                    NomeEquipamento = "Cadeira de Rodas Standard",
                    Quantidade = 25,
                    MaterialStatusId = 2 // Em Uso
                },
                // 7
                new MaterialEquipamentoAssociado
                {
                    MaterialEquipamentoAssociadoId = 7, // PK
                    NomeEquipamento = "Bomba de Infusão Volumétrica",
                    Quantidade = 40,
                    MaterialStatusId = 2 // Em Uso
                },
                // 8
                new MaterialEquipamentoAssociado
                {
                    MaterialEquipamentoAssociadoId = 8, // PK
                    NomeEquipamento = "Termómetro Digital de Testa",
                    Quantidade = 95,
                    MaterialStatusId = 1 // Disponível
                },
                // 9
                new MaterialEquipamentoAssociado
                {
                    MaterialEquipamentoAssociadoId = 9, // PK
                    NomeEquipamento = "Aspirador Cirúrgico",
                    Quantidade = 8,
                    MaterialStatusId = 2 // Em Uso
                },
                // 10
                new MaterialEquipamentoAssociado
                {
                    MaterialEquipamentoAssociadoId = 10, // PK
                    NomeEquipamento = "Mesa de Cirurgia Multiusos",
                    Quantidade = 3,
                    MaterialStatusId = 2 // Em Uso
                },
                // 11
                new MaterialEquipamentoAssociado
                {
                    MaterialEquipamentoAssociadoId = 11, // PK
                    NomeEquipamento = "Kit de Sutura Estéril",
                    Quantidade = 300,
                    MaterialStatusId = 1 // Disponível
                },
                // 12
                new MaterialEquipamentoAssociado
                {
                    MaterialEquipamentoAssociadoId = 12, // PK
                    NomeEquipamento = "Bisturi Descartável (Unidade)",
                    Quantidade = 1500,
                    MaterialStatusId = 1 // Disponível
                },
                // 13
                new MaterialEquipamentoAssociado
                {
                    MaterialEquipamentoAssociadoId = 13, // PK
                    NomeEquipamento = "Ventilador Pulmonar",
                    Quantidade = 12,
                    MaterialStatusId = 4 // Em Manutenção
                },
                // 14
                new MaterialEquipamentoAssociado
                {
                    MaterialEquipamentoAssociadoId = 14, // PK
                    NomeEquipamento = "Carro de Emergência (Completo)",
                    Quantidade = 6,
                    MaterialStatusId = 2 // Em Uso
                },
                // 15
                new MaterialEquipamentoAssociado
                {
                    MaterialEquipamentoAssociadoId = 15, // PK
                    NomeEquipamento = "Agulha Hipodérmica 21G",
                    Quantidade = 2000,
                    MaterialStatusId = 1 // Disponível
                },
                // 16
                new MaterialEquipamentoAssociado
                {
                    MaterialEquipamentoAssociadoId = 16, // PK
                    NomeEquipamento = "Otoscópio/Oftalmoscópio",
                    Quantidade = 18,
                    MaterialStatusId = 2 // Em Uso
                },
                // 17
                new MaterialEquipamentoAssociado
                {
                    MaterialEquipamentoAssociadoId = 17, // PK
                    NomeEquipamento = "Tala Imobilizadora (Vários Tamanhos)",
                    Quantidade = 75,
                    MaterialStatusId = 1 // Disponível
                },
                // 18
                new MaterialEquipamentoAssociado
                {
                    MaterialEquipamentoAssociadoId = 18, // PK
                    NomeEquipamento = "Esfigmomanómetro Digital",
                    Quantidade = 35,
                    MaterialStatusId = 1 // Disponível
                },
                // 19
                new MaterialEquipamentoAssociado
                {
                    MaterialEquipamentoAssociadoId = 19, // PK
                    NomeEquipamento = "Mascára Cirúrgica N95",
                    Quantidade = 1000,
                    MaterialStatusId = 1 // Disponível
                },
                // 20
                new MaterialEquipamentoAssociado
                {
                    MaterialEquipamentoAssociadoId = 20, // PK
                    NomeEquipamento = "Laringoscópio Completo",
                    Quantidade = 7,
                    MaterialStatusId = 2 // Em Uso
                },
                // 21
                new MaterialEquipamentoAssociado
                {
                    MaterialEquipamentoAssociadoId = 21, // PK
                    NomeEquipamento = "Fato de Proteção Biológica",
                    Quantidade = 150,
                    MaterialStatusId = 1 // Disponível
                },
                // 22
                new MaterialEquipamentoAssociado
                {
                    MaterialEquipamentoAssociadoId = 22, // PK
                    NomeEquipamento = "Desfibrilhador Externo Automático (DEA)",
                    Quantidade = 10,
                    MaterialStatusId = 2 // Em Uso
                },
                // 23
                new MaterialEquipamentoAssociado
                {
                    MaterialEquipamentoAssociadoId = 23, // PK
                    NomeEquipamento = "Pilha Alcalina AA (Pack de 10)",
                    Quantidade = 20,
                    MaterialStatusId = 1 // Disponível
                },
                // 24
                new MaterialEquipamentoAssociado
                {
                    MaterialEquipamentoAssociadoId = 24, // PK
                    NomeEquipamento = "Estetoscópio Littmann",
                    Quantidade = 55,
                    MaterialStatusId = 2 // Em Uso
                },
                // 25
                new MaterialEquipamentoAssociado
                {
                    MaterialEquipamentoAssociadoId = 25, // PK
                    NomeEquipamento = "Balança Hospitalar Digital",
                    Quantidade = 4,
                    MaterialStatusId = 4 // Em Manutenção
                },
                // 26
                new MaterialEquipamentoAssociado
                {
                    MaterialEquipamentoAssociadoId = 26, // PK
                    NomeEquipamento = "Gesso Ortopédico (Rolo)",
                    Quantidade = 90,
                    MaterialStatusId = 1 // Disponível
                }
            );
        }
    }
}
