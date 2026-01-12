using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
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

        public DbSet<RegistoMateriais> RegistoMateriais { get; set; }

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

            modelBuilder.Entity<Funcao>().HasData(
                new Funcao { FuncaoId = 1, NomeFuncao = "Técnico de Radiologia" },
                new Funcao { FuncaoId = 2, NomeFuncao = "Fisioterapeuta" },
                new Funcao { FuncaoId = 3, NomeFuncao = "Técnico de Cardiopneumologia" },
                new Funcao { FuncaoId = 4, NomeFuncao = "Técnico de Análises Clínicas" },
                new Funcao { FuncaoId = 5, NomeFuncao = "Terapeuta Ocupacional" },
                new Funcao { FuncaoId = 6, NomeFuncao = "Ortopedista" },
                new Funcao { FuncaoId = 7, NomeFuncao = "Enfermeiro Especialista" },
                new Funcao { FuncaoId = 8, NomeFuncao = "Nutricionista" },
                new Funcao { FuncaoId = 9, NomeFuncao = "Técnico de Medicina Nuclear" },
                new Funcao { FuncaoId = 10, NomeFuncao = "Cardiologista" },
                new Funcao { FuncaoId = 11, NomeFuncao = "Podologista" },
                new Funcao { FuncaoId = 12, NomeFuncao = "Técnico de Neurofisiologia" },
                new Funcao { FuncaoId = 13, NomeFuncao = "Técnico Auxiliar de Saúde" },
                new Funcao { FuncaoId = 14, NomeFuncao = "Optometrista" },
                new Funcao { FuncaoId = 15, NomeFuncao = "Técnico de Medicina Física e Reabilitação" }
            );



            modelBuilder.Entity<EstadoMaterial>().HasData(
                            new EstadoMaterial { MaterialStatusId = 1, Nome = "Disponível", Descricao = "Material pronto para utilização" },
                            new EstadoMaterial { MaterialStatusId = 2, Nome = "Em Uso", Descricao = "Material atualmente utilizado" },
                            new EstadoMaterial { MaterialStatusId = 3, Nome = "Reservado", Descricao = "Material reservado para utilização futura" },
                            new EstadoMaterial { MaterialStatusId = 4, Nome = "Em Manutenção", Descricao = "Material em reparação/manutenção" },
                            new EstadoMaterial { MaterialStatusId = 5, Nome = "Danificado", Descricao = "Material danificado e não utilizável" },
                            new EstadoMaterial { MaterialStatusId = 6, Nome = "Perdido", Descricao = "Material não localizado" }
            );

            modelBuilder.Entity<ProfissionalExecutante>().HasData(
                new ProfissionalExecutante
                {
                    ProfissionalExecutanteId = 1,
                    Nome = "André Kandonga",
                    FuncaoId = 1, // Técnico de Radiologia
                    Telefone = "912912915",
                    Email = "Kandonga123@gmail.com"
                },
                new ProfissionalExecutante
                {
                    ProfissionalExecutanteId = 2,
                    Nome = "Miguel Santos",
                    FuncaoId = 2, // Fisioterapeuta
                    Telefone = "912912914",
                    Email = "MiguelSantos123@gmail.com"
                },
                new ProfissionalExecutante
                {
                    ProfissionalExecutanteId = 3,
                    Nome = "Dostoevsky",
                    FuncaoId = 3, // Técnico de Cardiopneumologia
                    Telefone = "912913914",
                    Email = "DostoevskySuba@gmail.com"
                },
                new ProfissionalExecutante
                {
                    ProfissionalExecutanteId = 4,
                    Nome = "Ricardo Quaresma",
                    FuncaoId = 4, // Técnico de Análises Clínicas
                    Telefone = "910101010",
                    Email = "QuaresmaPorto@gmail.com"
                },
                new ProfissionalExecutante
                {
                    ProfissionalExecutanteId = 5,
                    Nome = "Mai Da Silva",
                    FuncaoId = 5, // Terapeuta Ocupacional
                    Telefone = "912912222",
                    Email = "Mai123222suba@gmail.com",
                },
                new ProfissionalExecutante
                {
                    ProfissionalExecutanteId = 6,
                    Nome = "Diogo Rodrigues",
                    FuncaoId = 6, // Ortopedista
                    Telefone = "912912522",
                    Email = "DiogoRodrigues04@gmail.com",
                }
            );

            modelBuilder.Entity<MaterialEquipamentoAssociado>().HasData(
                // --- LUVAS E PROTEÇÃO ---
                new MaterialEquipamentoAssociado { MaterialEquipamentoAssociadoId = 1, NomeEquipamento = "Luvas de Látex", Quantidade = 2, Tamanho = "S" },
                new MaterialEquipamentoAssociado { MaterialEquipamentoAssociadoId = 2, NomeEquipamento = "Luvas de Látex", Quantidade = 3, Tamanho = "M" },
                new MaterialEquipamentoAssociado { MaterialEquipamentoAssociadoId = 3, NomeEquipamento = "Luvas de Látex", Quantidade = 1, Tamanho = "L" },
                new MaterialEquipamentoAssociado { MaterialEquipamentoAssociadoId = 4, NomeEquipamento = "Máscara Cirúrgica Descartável", Quantidade = 2, Tamanho = "Único" },
                new MaterialEquipamentoAssociado { MaterialEquipamentoAssociadoId = 5, NomeEquipamento = "Bata Cirúrgica Estéril", Quantidade = 1, Tamanho = "L" },

                // --- CONSUMÍVEIS DE INJEÇÃO ---
                new MaterialEquipamentoAssociado { MaterialEquipamentoAssociadoId = 6, NomeEquipamento = "Seringa", Quantidade = 2, Tamanho = "5ml" },
                new MaterialEquipamentoAssociado { MaterialEquipamentoAssociadoId = 7, NomeEquipamento = "Seringa", Quantidade = 1, Tamanho = "10ml" },
                new MaterialEquipamentoAssociado { MaterialEquipamentoAssociadoId = 8, NomeEquipamento = "Agulha Hipodérmica", Quantidade = 2, Tamanho = "21G" },
                new MaterialEquipamentoAssociado { MaterialEquipamentoAssociadoId = 9, NomeEquipamento = "Agulha Hipodérmica", Quantidade = 2, Tamanho = "23G" },
                new MaterialEquipamentoAssociado { MaterialEquipamentoAssociadoId = 10, NomeEquipamento = "Algodão Hidrófilo (Pacote)", Quantidade = 1, Tamanho = "500g" },

                // --- CURATIVOS E LIGADURAS ---
                new MaterialEquipamentoAssociado { MaterialEquipamentoAssociadoId = 11, NomeEquipamento = "Compressas de Gaze Estéril", Quantidade = 2, Tamanho = "10x10cm" },
                new MaterialEquipamentoAssociado { MaterialEquipamentoAssociadoId = 12, NomeEquipamento = "Ligadura Elástica", Quantidade = 1, Tamanho = "5cm" },
                new MaterialEquipamentoAssociado { MaterialEquipamentoAssociadoId = 13, NomeEquipamento = "Ligadura Elástica", Quantidade = 2, Tamanho = "10cm" },
                new MaterialEquipamentoAssociado { MaterialEquipamentoAssociadoId = 14, NomeEquipamento = "Adesivo Micropore", Quantidade = 2, Tamanho = "2.5cm" },
                new MaterialEquipamentoAssociado { MaterialEquipamentoAssociadoId = 15, NomeEquipamento = "Solução Betadine (Frasco)", Quantidade = 1, Tamanho = "100ml" },

                // --- EQUIPAMENTO DE DIAGNÓSTICO/OUTROS ---
                new MaterialEquipamentoAssociado { MaterialEquipamentoAssociadoId = 16, NomeEquipamento = "Espátula de Madeira (Abaixa-língua)", Quantidade = 2, Tamanho = "Único" },
                new MaterialEquipamentoAssociado { MaterialEquipamentoAssociadoId = 17, NomeEquipamento = "Copo de Recolha de Urina", Quantidade = 3, Tamanho = "100ml" },
                new MaterialEquipamentoAssociado { MaterialEquipamentoAssociadoId = 18, NomeEquipamento = "Gel para Ecografia (Frasco)", Quantidade = 2, Tamanho = "250ml" },
                new MaterialEquipamentoAssociado { MaterialEquipamentoAssociadoId = 19, NomeEquipamento = "Papel para marquesa (Rolo)", Quantidade = 2, Tamanho = "Standard" },
                new MaterialEquipamentoAssociado { MaterialEquipamentoAssociadoId = 20, NomeEquipamento = "Cateter Intravenoso", Quantidade = 4, Tamanho = "20G" }
            );
        }
    }
}
