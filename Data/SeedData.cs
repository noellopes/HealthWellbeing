using HealthWellbeing.Controllers;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthWellbeing.Data
{
    public static class SeedData
    {
        public static void Populate(HealthWellbeingDbContext? dbContext)
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));

            // Garante que a BD existe
            dbContext.Database.EnsureCreated();

            PopulateGeneros(dbContext);
            PopulateGruposMusculares(dbContext);
            PopulateEquipamentos(dbContext);
            PopulateProblemasSaude(dbContext);
            PopulateObjetivosFisicos(dbContext);
            PopulateBeneficios(dbContext);
            PopulateTiposExercicio(dbContext);
            PopulateExercicios(dbContext);

        }

        private static void PopulateGeneros(HealthWellbeingDbContext dbContext)
        {
            if (dbContext.Genero.Any()) return;

            var generos = new[]
            {
                new Genero { NomeGenero = "Masculino" },
                new Genero { NomeGenero = "Feminino" },
                new Genero { NomeGenero = "Unisexo" }
            };

            dbContext.Genero.AddRange(generos);
            dbContext.SaveChanges();
        }

        private static void PopulateGruposMusculares(HealthWellbeingDbContext dbContext)
        {
            if (dbContext.GrupoMuscular.Any()) return;

            // --- Criação dos Grupos ---
            var grupos = new[]
            {
            new GrupoMuscular { GrupoMuscularNome = "Peito", LocalizacaoCorporal = "Frente do tronco" },
            new GrupoMuscular { GrupoMuscularNome = "Costas", LocalizacaoCorporal = "Parte posterior do tronco" },
            new GrupoMuscular { GrupoMuscularNome = "Bíceps", LocalizacaoCorporal = "Parte frontal do braço" },
            new GrupoMuscular { GrupoMuscularNome = "Tríceps", LocalizacaoCorporal = "Parte posterior do braço" },
            new GrupoMuscular { GrupoMuscularNome = "Ombros", LocalizacaoCorporal = "Deltoides" },
            new GrupoMuscular { GrupoMuscularNome = "Deltoide Anterior", LocalizacaoCorporal = "Parte frontal do ombro" },
            new GrupoMuscular { GrupoMuscularNome = "Deltoide Lateral", LocalizacaoCorporal = "Parte lateral do ombro" },
            new GrupoMuscular { GrupoMuscularNome = "Deltoide Posterior", LocalizacaoCorporal = "Parte posterior do ombro" },
            new GrupoMuscular { GrupoMuscularNome = "Trapézio", LocalizacaoCorporal = "Parte superior das costas e pescoço" },
            new GrupoMuscular { GrupoMuscularNome = "Romboides", LocalizacaoCorporal = "Entre as escápulas" },
            new GrupoMuscular { GrupoMuscularNome = "Latíssimo do Dorso", LocalizacaoCorporal = "Laterais das costas" },
            new GrupoMuscular { GrupoMuscularNome = "Abdómen", LocalizacaoCorporal = "Região abdominal" },
            new GrupoMuscular { GrupoMuscularNome = "Abdominais Superiores", LocalizacaoCorporal = "Parte superior do abdómen" },
            new GrupoMuscular { GrupoMuscularNome = "Abdominais Inferiores", LocalizacaoCorporal = "Parte inferior do abdómen" },
            new GrupoMuscular { GrupoMuscularNome = "Oblíquos", LocalizacaoCorporal = "Laterais do abdómen" },
            new GrupoMuscular { GrupoMuscularNome = "Core", LocalizacaoCorporal = "Centro do tronco" },
            new GrupoMuscular { GrupoMuscularNome = "Glúteos", LocalizacaoCorporal = "Região das nádegas" },
            new GrupoMuscular { GrupoMuscularNome = "Quadríceps", LocalizacaoCorporal = "Parte frontal da coxa" },
            new GrupoMuscular { GrupoMuscularNome = "Isquiotibiais", LocalizacaoCorporal = "Parte posterior da coxa" },
            new GrupoMuscular { GrupoMuscularNome = "Adutores", LocalizacaoCorporal = "Parte interna da coxa" },
            new GrupoMuscular { GrupoMuscularNome = "Abdutores", LocalizacaoCorporal = "Parte externa da coxa" },
            new GrupoMuscular { GrupoMuscularNome = "Panturrilhas", LocalizacaoCorporal = "Parte inferior da perna" },
            new GrupoMuscular { GrupoMuscularNome = "Gastrocnémio", LocalizacaoCorporal = "Panturrilha superficial" },
            new GrupoMuscular { GrupoMuscularNome = "Sóleo", LocalizacaoCorporal = "Panturrilha profunda" },
            new GrupoMuscular { GrupoMuscularNome = "Antebraços", LocalizacaoCorporal = "Parte inferior do braço" },
            new GrupoMuscular { GrupoMuscularNome = "Flexores do Antebraço", LocalizacaoCorporal = "Face interna do antebraço" },
            new GrupoMuscular { GrupoMuscularNome = "Extensores do Antebraço", LocalizacaoCorporal = "Face externa do antebraço" },
            new GrupoMuscular { GrupoMuscularNome = "Pescoço", LocalizacaoCorporal = "Região cervical" },
            new GrupoMuscular { GrupoMuscularNome = "Eretores da Coluna", LocalizacaoCorporal = "Ao longo da coluna vertebral" },
            new GrupoMuscular { GrupoMuscularNome = "Lombares", LocalizacaoCorporal = "Parte inferior das costas" },
            new GrupoMuscular { GrupoMuscularNome = "Peitoral Maior", LocalizacaoCorporal = "Centro do peito" },
            new GrupoMuscular { GrupoMuscularNome = "Peitoral Menor", LocalizacaoCorporal = "Parte interna do peito" },
            new GrupoMuscular { GrupoMuscularNome = "Serrátil Anterior", LocalizacaoCorporal = "Lateral do tórax" },
            new GrupoMuscular { GrupoMuscularNome = "Pernas", LocalizacaoCorporal = "Membros inferiores" }

            };

            dbContext.GrupoMuscular.AddRange(grupos);
            dbContext.SaveChanges();

            // --- Criação dos Músculos ---
            var gruposDb = dbContext.GrupoMuscular.ToList();
            Func<string, int> getId = nome => gruposDb.First(g => g.GrupoMuscularNome == nome).GrupoMuscularId;

            var musculos = new[]
            {
                new Musculo { Nome_Musculo = "Peitoral Maior", GrupoMuscularId = getId("Peito") },
            new Musculo { Nome_Musculo = "Peitoral Menor", GrupoMuscularId = getId("Peito") },
            new Musculo { Nome_Musculo = "Subclávio", GrupoMuscularId = getId("Peito") },
            new Musculo { Nome_Musculo = "Serrátil Anterior", GrupoMuscularId = getId("Peito") },
            new Musculo { Nome_Musculo = "Dorsal Largo", GrupoMuscularId = getId("Costas") },
            new Musculo { Nome_Musculo = "Romboide Maior", GrupoMuscularId = getId("Costas") },
            new Musculo { Nome_Musculo = "Romboide Menor", GrupoMuscularId = getId("Costas") },
            new Musculo { Nome_Musculo = "Eretor da Espinha", GrupoMuscularId = getId("Costas") },
            new Musculo { Nome_Musculo = "Multífidos", GrupoMuscularId = getId("Costas") },
            new Musculo { Nome_Musculo = "Quadrado Lombar", GrupoMuscularId = getId("Costas") },
            new Musculo { Nome_Musculo = "Deltoide Anterior", GrupoMuscularId = getId("Ombros") },
            new Musculo { Nome_Musculo = "Deltoide Lateral", GrupoMuscularId = getId("Ombros") },
            new Musculo { Nome_Musculo = "Deltoide Posterior", GrupoMuscularId = getId("Ombros") },
            new Musculo { Nome_Musculo = "Redondo Maior", GrupoMuscularId = getId("Ombros") },
            new Musculo { Nome_Musculo = "Redondo Menor", GrupoMuscularId = getId("Ombros") },
            new Musculo { Nome_Musculo = "Bíceps Braquial", GrupoMuscularId = getId("Bíceps") },
            new Musculo { Nome_Musculo = "Braquial", GrupoMuscularId = getId("Bíceps") },
            new Musculo { Nome_Musculo = "Braquiorradial", GrupoMuscularId = getId("Bíceps") },
            new Musculo { Nome_Musculo = "Tríceps Braquial Cabeça Longa", GrupoMuscularId = getId("Tríceps") },
            new Musculo { Nome_Musculo = "Tríceps Braquial Cabeça Lateral", GrupoMuscularId = getId("Tríceps") },
            new Musculo { Nome_Musculo = "Tríceps Braquial Cabeça Medial", GrupoMuscularId = getId("Tríceps") },
            new Musculo { Nome_Musculo = "Ancóneo", GrupoMuscularId = getId("Tríceps") },
            new Musculo { Nome_Musculo = "Flexor Radial do Carpo", GrupoMuscularId = getId("Antebraços") },
            new Musculo { Nome_Musculo = "Flexor Ulnar do Carpo", GrupoMuscularId = getId("Antebraços") },
            new Musculo { Nome_Musculo = "Extensor Radial do Carpo", GrupoMuscularId = getId("Antebraços") },
            new Musculo { Nome_Musculo = "Extensor Ulnar do Carpo", GrupoMuscularId = getId("Antebraços") },
            new Musculo { Nome_Musculo = "Reto Abdominal", GrupoMuscularId = getId("Abdómen") },
            new Musculo { Nome_Musculo = "Oblíquo Externo", GrupoMuscularId = getId("Abdómen") },
            new Musculo { Nome_Musculo = "Oblíquo Interno", GrupoMuscularId = getId("Abdómen") },
            new Musculo { Nome_Musculo = "Transverso do Abdômen", GrupoMuscularId = getId("Abdómen") },
            new Musculo { Nome_Musculo = "Glúteo Máximo", GrupoMuscularId = getId("Glúteos") },
            new Musculo { Nome_Musculo = "Glúteo Médio", GrupoMuscularId = getId("Glúteos") },
            new Musculo { Nome_Musculo = "Glúteo Mínimo", GrupoMuscularId = getId("Glúteos") },
            new Musculo { Nome_Musculo = "Quadríceps", GrupoMuscularId = getId("Pernas") },
            new Musculo { Nome_Musculo = "Reto Femoral", GrupoMuscularId = getId("Pernas") },
            new Musculo { Nome_Musculo = "Vasto Lateral", GrupoMuscularId = getId("Pernas") },
            new Musculo { Nome_Musculo = "Vasto Medial", GrupoMuscularId = getId("Pernas") },
            new Musculo { Nome_Musculo = "Vasto Intermédio", GrupoMuscularId = getId("Pernas") },
            new Musculo { Nome_Musculo = "Isquiotibiais", GrupoMuscularId = getId("Pernas") },
            new Musculo { Nome_Musculo = "Bíceps Femoral", GrupoMuscularId = getId("Pernas") },
            new Musculo { Nome_Musculo = "Semitendinoso", GrupoMuscularId = getId("Pernas") },
            new Musculo { Nome_Musculo = "Semimembranoso", GrupoMuscularId = getId("Pernas") },
            new Musculo { Nome_Musculo = "Adutor Longo", GrupoMuscularId = getId("Adutores") },
            new Musculo { Nome_Musculo = "Adutor Curto", GrupoMuscularId = getId("Adutores") },
            new Musculo { Nome_Musculo = "Adutor Magno", GrupoMuscularId = getId("Adutores") },
            new Musculo { Nome_Musculo = "Tensor da Fáscia Lata", GrupoMuscularId = getId("Abdutores") },
            new Musculo { Nome_Musculo = "Gastrocnémio", GrupoMuscularId = getId("Panturrilhas") },
            new Musculo { Nome_Musculo = "Sóleo", GrupoMuscularId = getId("Panturrilhas") },
            new Musculo { Nome_Musculo = "Plantar", GrupoMuscularId = getId("Panturrilhas") }

            };

            dbContext.Musculo.AddRange(musculos);
            dbContext.SaveChanges();
        }

        private static void PopulateEquipamentos(HealthWellbeingDbContext dbContext)
        {
            if (dbContext.Equipamento.Any()) return;

            var equipamentos = new[]{
            new Equipamento { NomeEquipamento = "Halteres", RequerPeso = true },
            new Equipamento { NomeEquipamento = "Barra Olímpica", RequerPeso = true },
            new Equipamento { NomeEquipamento = "Discos de Peso", RequerPeso = true },
            new Equipamento { NomeEquipamento = "Caneleiras com Peso", RequerPeso = true },
            new Equipamento { NomeEquipamento = "Coletes com Peso", RequerPeso = true },
            new Equipamento { NomeEquipamento = "Kettlebell", RequerPeso = true },
            new Equipamento { NomeEquipamento = "Medicine Ball", RequerPeso = true },
            new Equipamento { NomeEquipamento = "Elásticos de Resistência", RequerPeso = true },
            new Equipamento { NomeEquipamento = "Banco de Musculação" },
            new Equipamento { NomeEquipamento = "Banco Inclinado" },
            new Equipamento { NomeEquipamento = "Banco Declinado" },
            new Equipamento { NomeEquipamento = "Rack de Agachamento" },
            new Equipamento { NomeEquipamento = "Gaiola de Potência" },
            new Equipamento { NomeEquipamento = "Tapete de Yoga" },
            new Equipamento { NomeEquipamento = "Bola de Pilates" },
            new Equipamento { NomeEquipamento = "Passadeira" },
            new Equipamento { NomeEquipamento = "Bicicleta Estática" },
            new Equipamento { NomeEquipamento = "Bicicleta de Spinning" },
            new Equipamento { NomeEquipamento = "Elíptica" },
            new Equipamento { NomeEquipamento = "Remo Indoor" },
            new Equipamento { NomeEquipamento = "TRX / Suspensão" },
            new Equipamento { NomeEquipamento = "Ab Wheel" },
            new Equipamento { NomeEquipamento = "Step" },
            new Equipamento { NomeEquipamento = "Barra de Elevações" },
            new Equipamento { NomeEquipamento = "Paralelas" },
            new Equipamento { NomeEquipamento = "Bosu" },
            new Equipamento { NomeEquipamento = "Rolo de Espuma" },
            new Equipamento { NomeEquipamento = "Escada de Agilidade" },
            new Equipamento { NomeEquipamento = "Saco de Boxe" }
        };


            dbContext.Equipamento.AddRange(equipamentos);
            dbContext.SaveChanges();
        }

        private static void PopulateProblemasSaude(HealthWellbeingDbContext dbContext)
        {
            if (dbContext.ProblemaSaude.Any()) return;

            var problemas = new[]
            {
            new ProblemaSaude { ProblemaCategoria = "Muscular", ProblemaNome = "Tendinite", ZonaAtingida = "Braço", Gravidade = 6 },
            new ProblemaSaude { ProblemaCategoria = "Muscular", ProblemaNome = "Distensão muscular", ZonaAtingida = "Coxa", Gravidade = 5 },
            new ProblemaSaude { ProblemaCategoria = "Muscular", ProblemaNome = "Rutura muscular", ZonaAtingida = "Gémeos", Gravidade = 8 },
            new ProblemaSaude { ProblemaCategoria = "Muscular", ProblemaNome = "Mialgia", ZonaAtingida = "Corpo", Gravidade = 4 },
            new ProblemaSaude { ProblemaCategoria = "Muscular", ProblemaNome = "Fibromialgia", ZonaAtingida = "Corpo", Gravidade = 7 },
            new ProblemaSaude { ProblemaCategoria = "Articular", ProblemaNome = "Artrite", ZonaAtingida = "Joelhos", Gravidade = 7 },
            new ProblemaSaude { ProblemaCategoria = "Articular", ProblemaNome = "Artrose", ZonaAtingida = "Anca", Gravidade = 6 },
            new ProblemaSaude { ProblemaCategoria = "Articular", ProblemaNome = "Luxação do ombro", ZonaAtingida = "Ombro", Gravidade = 7 },
            new ProblemaSaude { ProblemaCategoria = "Articular", ProblemaNome = "Entorse", ZonaAtingida = "Tornozelo", Gravidade = 5 },
            new ProblemaSaude { ProblemaCategoria = "Articular", ProblemaNome = "Bursite", ZonaAtingida = "Ombro", Gravidade = 6 },
            new ProblemaSaude { ProblemaCategoria = "Cardíaco", ProblemaNome = "Hipertensão Arterial", ZonaAtingida = "Coração", Gravidade = 8 },
            new ProblemaSaude { ProblemaCategoria = "Cardíaco", ProblemaNome = "Arritmia cardíaca", ZonaAtingida = "Coração", Gravidade = 7 },
            new ProblemaSaude { ProblemaCategoria = "Cardíaco", ProblemaNome = "Insuficiência cardíaca", ZonaAtingida = "Coração", Gravidade = 9 },
            new ProblemaSaude { ProblemaCategoria = "Cardíaco", ProblemaNome = "Doença coronária", ZonaAtingida = "Coração", Gravidade = 9 },
            new ProblemaSaude { ProblemaCategoria = "Cardíaco", ProblemaNome = "Taquicardia", ZonaAtingida = "Coração", Gravidade = 6 },
            new ProblemaSaude { ProblemaCategoria = "Respiratório", ProblemaNome = "Asma", ZonaAtingida = "Pulmões", Gravidade = 5 },
            new ProblemaSaude { ProblemaCategoria = "Respiratório", ProblemaNome = "Bronquite crónica", ZonaAtingida = "Pulmões", Gravidade = 6 },
            new ProblemaSaude { ProblemaCategoria = "Respiratório", ProblemaNome = "DPOC", ZonaAtingida = "Pulmões", Gravidade = 8 },
            new ProblemaSaude { ProblemaCategoria = "Respiratório", ProblemaNome = "Pneumonia", ZonaAtingida = "Pulmões", Gravidade = 8 },
            new ProblemaSaude { ProblemaCategoria = "Respiratório", ProblemaNome = "Apneia do sono", ZonaAtingida = "Sistema respiratório", Gravidade = 7 },
            new ProblemaSaude { ProblemaCategoria = "Neurológico", ProblemaNome = "Enxaqueca crónica", ZonaAtingida = "Cabeça", Gravidade = 6 },
            new ProblemaSaude { ProblemaCategoria = "Neurológico", ProblemaNome = "Epilepsia", ZonaAtingida = "Sistema nervoso", Gravidade = 8 },
            new ProblemaSaude { ProblemaCategoria = "Neurológico", ProblemaNome = "AVC", ZonaAtingida = "Cérebro", Gravidade = 10 },
            new ProblemaSaude { ProblemaCategoria = "Neurológico", ProblemaNome = "Parkinson", ZonaAtingida = "Sistema nervoso", Gravidade = 9 },
            new ProblemaSaude { ProblemaCategoria = "Neurológico", ProblemaNome = "Esclerose múltipla", ZonaAtingida = "Sistema nervoso", Gravidade = 9 },
            new ProblemaSaude { ProblemaCategoria = "Postural", ProblemaNome = "Escoliose", ZonaAtingida = "Coluna", Gravidade = 5 },
            new ProblemaSaude { ProblemaCategoria = "Postural", ProblemaNome = "Cifose", ZonaAtingida = "Coluna", Gravidade = 5 },
            new ProblemaSaude { ProblemaCategoria = "Postural", ProblemaNome = "Lordose", ZonaAtingida = "Coluna", Gravidade = 4 },
            new ProblemaSaude { ProblemaCategoria = "Postural", ProblemaNome = "Hérnia discal", ZonaAtingida = "Coluna lombar", Gravidade = 8 },
            new ProblemaSaude { ProblemaCategoria = "Postural", ProblemaNome = "Desvio postural", ZonaAtingida = "Coluna", Gravidade = 4 },
            new ProblemaSaude { ProblemaCategoria = "Endócrino", ProblemaNome = "Diabetes tipo 1", ZonaAtingida = "Sistema endócrino", Gravidade = 9 },
            new ProblemaSaude { ProblemaCategoria = "Endócrino", ProblemaNome = "Diabetes tipo 2", ZonaAtingida = "Sistema endócrino", Gravidade = 8 },
            new ProblemaSaude { ProblemaCategoria = "Endócrino", ProblemaNome = "Hipotiroidismo", ZonaAtingida = "Tiroide", Gravidade = 6 },
            new ProblemaSaude { ProblemaCategoria = "Endócrino", ProblemaNome = "Hipertiroidismo", ZonaAtingida = "Tiroide", Gravidade = 6 },
            new ProblemaSaude { ProblemaCategoria = "Endócrino", ProblemaNome = "Síndrome de Cushing", ZonaAtingida = "Sistema endócrino", Gravidade = 8 },
            new ProblemaSaude { ProblemaCategoria = "Ocular", ProblemaNome = "Miopia", ZonaAtingida = "Olhos", Gravidade = 3 },
            new ProblemaSaude { ProblemaCategoria = "Ocular", ProblemaNome = "Hipermetropia", ZonaAtingida = "Olhos", Gravidade = 3 },
            new ProblemaSaude { ProblemaCategoria = "Ocular", ProblemaNome = "Astigmatismo", ZonaAtingida = "Olhos", Gravidade = 3 },
            new ProblemaSaude { ProblemaCategoria = "Ocular", ProblemaNome = "Glaucoma", ZonaAtingida = "Olhos", Gravidade = 7 },
            new ProblemaSaude { ProblemaCategoria = "Ocular", ProblemaNome = "Cataratas", ZonaAtingida = "Olhos", Gravidade = 6 },
            new ProblemaSaude { ProblemaCategoria = "Metabólico", ProblemaNome = "Obesidade", ZonaAtingida = "Corpo", Gravidade = 7 },
            new ProblemaSaude { ProblemaCategoria = "Metabólico", ProblemaNome = "Síndrome metabólica", ZonaAtingida = "Corpo", Gravidade = 8 },
            new ProblemaSaude { ProblemaCategoria = "Metabólico", ProblemaNome = "Dislipidemia", ZonaAtingida = "Sistema metabólico", Gravidade = 6 },
            new ProblemaSaude { ProblemaCategoria = "Metabólico", ProblemaNome = "Gota", ZonaAtingida = "Articulações", Gravidade = 6 },
            new ProblemaSaude { ProblemaCategoria = "Metabólico", ProblemaNome = "Resistência à insulina", ZonaAtingida = "Sistema metabólico", Gravidade = 6 },
            new ProblemaSaude { ProblemaCategoria = "Ortopédico", ProblemaNome = "Fratura óssea", ZonaAtingida = "Membros", Gravidade = 8 },
            new ProblemaSaude { ProblemaCategoria = "Ortopédico", ProblemaNome = "Osteoporose", ZonaAtingida = "Ossos", Gravidade = 7 },
            new ProblemaSaude { ProblemaCategoria = "Ortopédico", ProblemaNome = "Escoliose degenerativa", ZonaAtingida = "Coluna", Gravidade = 7 },
            new ProblemaSaude { ProblemaCategoria = "Ortopédico", ProblemaNome = "Lesão ligamentar", ZonaAtingida = "Joelho", Gravidade = 7 },
            new ProblemaSaude { ProblemaCategoria = "Ortopédico", ProblemaNome = "Condromalácia patelar", ZonaAtingida = "Joelho", Gravidade = 6 }
            };

            dbContext.ProblemaSaude.AddRange(problemas);
            dbContext.SaveChanges();
        }

        private static void PopulateObjetivosFisicos(HealthWellbeingDbContext dbContext)
        {
            if (dbContext.ObjetivoFisico.Any()) return;

            var objetivos = new[]
            {
            new ObjetivoFisico { NomeObjetivo = "Perda de Peso" },
            new ObjetivoFisico { NomeObjetivo = "Ganho de Massa Muscular" },
            new ObjetivoFisico { NomeObjetivo = "Melhoria da Condição Física" },
            new ObjetivoFisico { NomeObjetivo = "Reabilitação Física" },
            new ObjetivoFisico { NomeObjetivo = "Aumento da Resistência" },
            new ObjetivoFisico { NomeObjetivo = "Definição Muscular" },
            new ObjetivoFisico { NomeObjetivo = "Mobilidade e Flexibilidade" },
            new ObjetivoFisico { NomeObjetivo = "Melhoria Postural" },
            new ObjetivoFisico { NomeObjetivo = "Fortalecimento do Core"},
            new ObjetivoFisico { NomeObjetivo = "Resistência Cardiovascular"},
            new ObjetivoFisico { NomeObjetivo = "Saúde Geral" }

        };

            dbContext.ObjetivoFisico.AddRange(objetivos);
            dbContext.SaveChanges();
        }


        private static void PopulateBeneficios(HealthWellbeingDbContext dbContext)
        {
            // Usa ToHashSet para verificação eficiente
            var beneficiosExistentes = dbContext.Beneficio.Select(b => b.NomeBeneficio).ToHashSet();

            var todosBeneficios = new[]
            {
            new Beneficio { NomeBeneficio = "Melhora da resistência cardiovascular", DescricaoBeneficio = "Aumenta a capacidade do coração e pulmões de fornecer oxigénio aos músculos." },
            new Beneficio { NomeBeneficio = "Fortalecimento muscular", DescricaoBeneficio = "Aumenta a força, resistência e tamanho das fibras musculares." },
            new Beneficio { NomeBeneficio = "Aumento da flexibilidade", DescricaoBeneficio = "Melhora a amplitude de movimento nas articulações, prevenindo lesões." },
            new Beneficio { NomeBeneficio = "Redução do stress", DescricaoBeneficio = "Ajuda a reduzir a ansiedade e melhora o humor através da libertação de endorfinas." },
            new Beneficio { NomeBeneficio = "Perda de peso", DescricaoBeneficio = "Auxilia no controlo do peso, aumentando o gasto calórico e melhorando o metabolismo." },
            new Beneficio { NomeBeneficio = "Melhora da coordenação e equilíbrio", DescricaoBeneficio = "Desenvolve a capacidade do corpo de realizar movimentos complexos e manter a postura." },
            new Beneficio { NomeBeneficio = "Aumento da densidade óssea", DescricaoBeneficio = "Estimula o aumento da massa óssea, prevenindo a osteoporose." },
            new Beneficio { NomeBeneficio = "Melhoria da Qualidade do Sono", DescricaoBeneficio = "Regula os ciclos de sono, promovendo um descanso mais profundo." },
            new Beneficio { NomeBeneficio = "Aumento da Capacidade Pulmonar", DescricaoBeneficio = "Fortalece os músculos respiratórios e melhora a oxigenação." },
            new Beneficio { NomeBeneficio = "Controlo Glicémico", DescricaoBeneficio = "Ajuda a regular os níveis de açúcar no sangue." },
            new Beneficio { NomeBeneficio = "Promoção da Saúde Mental", DescricaoBeneficio = "Reduz sintomas de depressão e melhora a concentração." },
            new Beneficio { NomeBeneficio = "Recuperação Ativa", DescricaoBeneficio = "Reduz dores musculares e acelera a recuperação pós-exercício." },
            new Beneficio { NomeBeneficio = "Melhora da Postura Corporal", DescricaoBeneficio = "Fortalece músculos estabilizadores e reduz desequilíbrios posturais." },
            new Beneficio { NomeBeneficio = "Estímulo do Sistema Imunitário", DescricaoBeneficio = "Melhora a resposta do organismo contra infeções." },
            new Beneficio { NomeBeneficio = "Desenvolvimento da agilidade", DescricaoBeneficio = "Aumenta a rapidez e eficiência dos movimentos." },
            new Beneficio { NomeBeneficio = "Aumento da força funcional", DescricaoBeneficio = "Melhora a capacidade de realizar tarefas do dia a dia." },
            new Beneficio { NomeBeneficio = "Prevenção de lesões", DescricaoBeneficio = "Fortalece músculos e articulações, reduzindo o risco de lesões." },
            new Beneficio { NomeBeneficio = "Melhora da mobilidade articular", DescricaoBeneficio = "Facilita os movimentos naturais das articulações." },
            new Beneficio { NomeBeneficio = "Redução da pressão arterial", DescricaoBeneficio = "Ajuda a manter níveis saudáveis de tensão arterial." },
            new Beneficio { NomeBeneficio = "Aumento da resistência muscular", DescricaoBeneficio = "Permite sustentar esforços por mais tempo sem fadiga." },
            new Beneficio { NomeBeneficio = "Melhoria da composição corporal", DescricaoBeneficio = "Reduz a massa gorda e aumenta a massa magra." },
            new Beneficio { NomeBeneficio = "Aumento da autoestima", DescricaoBeneficio = "Contribui para uma imagem corporal mais positiva." },
            new Beneficio { NomeBeneficio = "Melhora da circulação sanguínea", DescricaoBeneficio = "Facilita o transporte de oxigénio e nutrientes." },
            new Beneficio { NomeBeneficio = "Regulação hormonal", DescricaoBeneficio = "Ajuda a equilibrar hormonas relacionadas com o stress e metabolismo." },
            new Beneficio { NomeBeneficio = "Melhoria da capacidade cognitiva", DescricaoBeneficio = "Aumenta a memória, atenção e velocidade de raciocínio." },
            new Beneficio { NomeBeneficio = "Redução da fadiga", DescricaoBeneficio = "Aumenta os níveis de energia no dia a dia." },
            new Beneficio { NomeBeneficio = "Melhora da saúde articular", DescricaoBeneficio = "Lubrifica as articulações e reduz rigidez." },
            new Beneficio { NomeBeneficio = "Aumento da tolerância ao esforço", DescricaoBeneficio = "Permite realizar atividades físicas com menor desconforto." },
            new Beneficio { NomeBeneficio = "Promoção do envelhecimento saudável", DescricaoBeneficio = "Ajuda a manter autonomia e funcionalidade com o avançar da idade." },
            new Beneficio { NomeBeneficio = "Melhoria do equilíbrio emocional", DescricaoBeneficio = "Contribui para maior estabilidade emocional." },
            new Beneficio { NomeBeneficio = "Redução do risco cardiovascular", DescricaoBeneficio = "Diminui a probabilidade de doenças cardíacas." },
            new Beneficio { NomeBeneficio = "Aumento da consciência corporal", DescricaoBeneficio = "Melhora o controlo e perceção do próprio corpo." },
            new Beneficio { NomeBeneficio = "Melhoria da respiração", DescricaoBeneficio = "Ensina padrões respiratórios mais eficientes." },
            new Beneficio { NomeBeneficio = "Redução da rigidez muscular", DescricaoBeneficio = "Diminui tensões acumuladas nos músculos." },
            new Beneficio { NomeBeneficio = "Promoção de hábitos saudáveis", DescricaoBeneficio = "Incentiva um estilo de vida mais ativo e equilibrado." },
            new Beneficio { NomeBeneficio = "Desenvolvimento de Agilidade", DescricaoBeneficio = "Incentiva um estilo de vida mais ativo e equilibrado." }
            };

            // Filtra e adiciona apenas os benefícios que ainda não existem
            var beneficiosParaAdicionar = todosBeneficios
                .Where(b => !beneficiosExistentes.Contains(b.NomeBeneficio))
                .ToList();

            if (beneficiosParaAdicionar.Any())
            {
                dbContext.Beneficio.AddRange(beneficiosParaAdicionar);
                dbContext.SaveChanges();
            }
        }

        private static void PopulateTiposExercicio(HealthWellbeingDbContext dbContext)
        {
            // 1. Recuperar Benefícios para ligar
            // É essencial recarregar a BD para obter os IDs gerados pelo SaveChanges do PopulateBeneficios.
            var beneficiosDb = dbContext.Beneficio.ToDictionary(b => b.NomeBeneficio, b => b);

            // Função de utilidade para buscar o ID do benefício pela chave (nome).
            // Usaremos o ID na entidade de junção (TipoExercicioBeneficio) para evitar o erro de tracking.
            Beneficio GetBen(string nome)
            {
                if (beneficiosDb.TryGetValue(nome, out var beneficio))
                {
                    return beneficio;
                }
                // Fallback (se por acaso o nome não for encontrado, o que não deve acontecer)
                return beneficiosDb.First().Value;
            }

            // 2. Definir todos os tipos de exercício
            var todosTiposExercicio = new[]
            {
                new TipoExercicio
                {
                    NomeTipoExercicios = "Cardiovascular",
                    DescricaoTipoExercicios = "Atividades contínuas que aumentam a frequência cardíaca por um período prolongado.",
                    CaracteristicasTipoExercicios = "Alta queima calórica, ritmo constante, como corrida ou ciclismo.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        // CORREÇÃO: Usar o ID da entidade já rastreada
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Melhora da resistência cardiovascular").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Perda de peso").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Aumento da Capacidade Pulmonar").BeneficioId }
                    }
                },
                new TipoExercicio
                {
                    NomeTipoExercicios = "Força",
                    DescricaoTipoExercicios = "Exercícios focados no desenvolvimento da força e massa muscular.",
                    CaracteristicasTipoExercicios = "Uso de pesos livres, máquinas ou resistência corporal, com repetições e séries.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Fortalecimento muscular").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Aumento da densidade óssea").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Controlo Glicémico").BeneficioId }
                    }
                },
                new TipoExercicio
                {
                    NomeTipoExercicios = "Flexibilidade",
                    DescricaoTipoExercicios = "Atividades estáticas ou dinâmicas que melhoram o alongamento muscular e a mobilidade articular.",
                    CaracteristicasTipoExercicios = "Movimentos suaves e sustentados, como alongamentos estáticos.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Aumento da flexibilidade").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Redução do stress").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Recuperação Ativa").BeneficioId }
                    }
                },
                new TipoExercicio
                {
                    NomeTipoExercicios = "Funcional",
                    DescricaoTipoExercicios = "Exercícios que preparam o corpo para atividades da vida diária, envolvendo múltiplos grupos musculares.",
                    CaracteristicasTipoExercicios = "Trabalho com peso corporal, bolas medicinais ou faixas de resistência, focando na estabilidade.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Fortalecimento muscular").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Melhora da coordenação e equilíbrio").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Melhora da Postura Corporal").BeneficioId }
                    }
                },
                new TipoExercicio
                {
                    NomeTipoExercicios = "Pilates",
                    DescricaoTipoExercicios = "Método focado no fortalecimento do 'core' (centro do corpo), postura e consciência corporal.",
                    CaracteristicasTipoExercicios = "Controle de respiração e movimentos precisos, uso de máquinas ou tapetes.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Fortalecimento muscular").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Melhora da Postura Corporal").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Aumento da flexibilidade").BeneficioId }
                    }
                },
                new TipoExercicio
                {
                    NomeTipoExercicios = "Yoga",
                    DescricaoTipoExercicios = "Prática que combina posturas físicas (asanas), exercícios de respiração (pranayama) e meditação.",
                    CaracteristicasTipoExercicios = "Série de posturas fluidas ou mantidas, com foco na mente e corpo.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Aumento da flexibilidade").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Redução do stress").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Promoção da Saúde Mental").BeneficioId }
                    }
                },
                new TipoExercicio
                {
                    NomeTipoExercicios = "Resistência",
                    DescricaoTipoExercicios = "Alterna curtos períodos de exercício intenso com períodos de recuperação.",
                    CaracteristicasTipoExercicios = "Esforço máximo seguido de descanso, treino rápido e eficiente.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Melhora da resistência cardiovascular").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Perda de peso").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Aumento da Capacidade Pulmonar").BeneficioId }
                    }
                },
                new TipoExercicio
                {
                    NomeTipoExercicios = "Natação",
                    DescricaoTipoExercicios = "Exercício de baixo impacto que trabalha o corpo inteiro na água.",
                    CaracteristicasTipoExercicios = "Resistência da água, trabalho corporal total, melhora respiratória.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Melhora da resistência cardiovascular").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Fortalecimento muscular").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Aumento da Capacidade Pulmonar").BeneficioId }
                    }
                },
                new TipoExercicio
                {
                    NomeTipoExercicios = "Boxe/Artes Marciais",
                    DescricaoTipoExercicios = "Treino que combina movimentos aeróbicos, de força, agilidade e coordenação.",
                    CaracteristicasTipoExercicios = "Golpes, chutes, uso de sacos de pancada, alta intensidade.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Desenvolvimento de Agilidade").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Redução do stress").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Perda de peso").BeneficioId }
                    }
                },
                new TipoExercicio
                {
                    NomeTipoExercicios = "Calistenia",
                    DescricaoTipoExercicios = "Utiliza o peso corporal como resistência para desenvolver força e coordenação.",
                    CaracteristicasTipoExercicios = "Flexões, barras, agachamentos sem peso adicional, foco em movimentos compostos.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Fortalecimento muscular").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Melhora da coordenação e equilíbrio").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Melhora da Postura Corporal").BeneficioId }
                    }
                },
                new TipoExercicio
                {
                    NomeTipoExercicios = "Treino de Agilidade",
                    DescricaoTipoExercicios = "Focado em melhorar a capacidade de reação, velocidade e coordenação motora.",
                    CaracteristicasTipoExercicios = "Uso de escadas de agilidade, cones, saltos laterais.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Desenvolvimento de Agilidade").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Melhora da coordenação e equilíbrio").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Estímulo do Sistema Imunitário").BeneficioId }
                    }
                },
                new TipoExercicio
                {
                    NomeTipoExercicios = "Caminhada/Passeio",
                    DescricaoTipoExercicios = "Atividade cardiovascular de baixo impacto, ideal para recuperação ou iniciantes.",
                    CaracteristicasTipoExercicios = "Ritmo moderado, pode ser feita em qualquer lugar.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Melhora da resistência cardiovascular").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Redução do stress").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Recuperação Ativa").BeneficioId }
                    }
                },
                new TipoExercicio
                {
                    NomeTipoExercicios = "Recuperação Ativa",
                    DescricaoTipoExercicios = "Movimentos leves destinados a melhorar a circulação e acelerar a recuperação muscular.",
                    CaracteristicasTipoExercicios = "Alongamentos suaves, massagem com rolo de espuma, ioga regenerativa.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Recuperação Ativa").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Aumento da flexibilidade").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Promoção da Saúde Mental").BeneficioId }
                    }
                },
                new TipoExercicio
                {
                    NomeTipoExercicios = "Mindfulness e Respiração",
                    DescricaoTipoExercicios = "Focado na atenção plena e técnicas de respiração para controlar o sistema nervoso.",
                    CaracteristicasTipoExercicios = "Sessões de meditação, exercícios de respiração diafragmática.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Redução do stress").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Promoção da Saúde Mental").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Melhoria da Qualidade do Sono").BeneficioId }
                    }
                },
                new TipoExercicio
                {
                    NomeTipoExercicios = "Alongamentos",
                    DescricaoTipoExercicios = "Exercícios destinados a alongar músculos e melhorar a flexibilidade geral.",
                    CaracteristicasTipoExercicios = "Movimentos lentos e controlados, mantidos por alguns segundos.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Aumento da flexibilidade").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Redução do stress").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Melhora da mobilidade articular").BeneficioId }
                    }
                },

                new TipoExercicio
                {
                    NomeTipoExercicios = "Treino de Resistência",
                    DescricaoTipoExercicios = "Treino focado em aumentar a resistência muscular ao longo do tempo.",
                    CaracteristicasTipoExercicios = "Repetições elevadas com cargas moderadas.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Aumento da resistência muscular").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Fortalecimento muscular").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Melhoria da composição corporal").BeneficioId }
                    }
                },

                new TipoExercicio
                {
                    NomeTipoExercicios = "Treino Postural",
                    DescricaoTipoExercicios = "Exercícios focados na correção postural e alinhamento corporal.",
                    CaracteristicasTipoExercicios = "Trabalho de consciência corporal e fortalecimento estabilizador.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Melhora da Postura Corporal").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Aumento da consciência corporal").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Prevenção de lesões").BeneficioId }
                    }
                },

                new TipoExercicio
                {
                    NomeTipoExercicios = "Treino de Equilíbrio",
                    DescricaoTipoExercicios = "Exercícios destinados a melhorar estabilidade e controlo corporal.",
                    CaracteristicasTipoExercicios = "Exercícios unilaterais e superfícies instáveis.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Melhora da coordenação e equilíbrio").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Prevenção de lesões").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Aumento da consciência corporal").BeneficioId }
                    }
                },

                new TipoExercicio
                {
                    NomeTipoExercicios = "Treino Respiratório",
                    DescricaoTipoExercicios = "Exercícios focados no controlo da respiração e eficiência pulmonar.",
                    CaracteristicasTipoExercicios = "Respiração controlada, diafragmática e consciente.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Melhoria da respiração").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Aumento da Capacidade Pulmonar").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Redução do stress").BeneficioId }
                    }
                },

                new TipoExercicio
                {
                    NomeTipoExercicios = "Treino de Baixo Impacto",
                    DescricaoTipoExercicios = "Atividades físicas suaves que reduzem o impacto nas articulações.",
                    CaracteristicasTipoExercicios = "Movimentos controlados, ideais para iniciantes ou reabilitação.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Melhora da saúde articular").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Recuperação Ativa").BeneficioId },
                        new TipoExercicioBeneficio { BeneficioId = GetBen("Promoção do envelhecimento saudável").BeneficioId }
                    }
                }

            };

            // 3. Verifica e adiciona apenas os Tipos de Exercício que faltam
            var tiposExistentes = dbContext.TipoExercicio.Select(t => t.NomeTipoExercicios).ToHashSet();

            var tiposParaAdicionar = todosTiposExercicio
                .Where(t => !tiposExistentes.Contains(t.NomeTipoExercicios))
                .ToList();

            if (tiposParaAdicionar.Any())
            {
                // NOTA: Ao adicionar o TipoExercicio, as suas entidades aninhadas TipoExercicioBeneficio
                // serão rastreadas, mas como apenas usamos o ID do Beneficio, não haverá conflito.
                dbContext.TipoExercicio.AddRange(tiposParaAdicionar);
                dbContext.SaveChanges();
            }
        }

        private static void PopulateExercicios(HealthWellbeingDbContext dbContext)
        {
            if (dbContext.Exercicio.Any()) return;

            // 1. Carregar referências para chaves estrangeiras
            var generos = dbContext.Genero.ToDictionary(g => g.NomeGenero, g => g.GeneroId);
            var grupos = dbContext.GrupoMuscular.ToDictionary(g => g.GrupoMuscularNome, g => g.GrupoMuscularId);
            var equipamentos = dbContext.Equipamento.ToDictionary(e => e.NomeEquipamento, e => e.EquipamentoId);
            var tipos = dbContext.TipoExercicio.ToDictionary(t => t.NomeTipoExercicios, t => t.TipoExercicioId);
            var problemas = dbContext.ProblemaSaude.ToDictionary(p => p.ProblemaNome, p => p.ProblemaSaudeId);
            var objetivos = dbContext.ObjetivoFisico.ToDictionary(o => o.NomeObjetivo, o => o.ObjetivoFisicoId);



            // Helpers seguros (Fallback para o primeiro item se não encontrar)
            int GetGenId(string nome) => generos.TryGetValue(nome, out int id) ? id : generos.Values.First();
            int GetGrId(string nome) => grupos.TryGetValue(nome, out int id) ? id : grupos.Values.First();
            int GetEqId(string nome) => equipamentos.TryGetValue(nome, out int id) ? id : equipamentos.Values.First();
            int GetObjId(string nome) => objetivos.TryGetValue(nome, out int id) ? id : objetivos.Values.First();
            int GetTipoId(string nome) => tipos.TryGetValue(nome, out int id) ? id : (tipos.TryGetValue("Força", out int fId) ? fId : tipos.Values.First());
            int GetProbId(string nome) => problemas.TryGetValue(nome, out int id) ? id : 0;

            var exercicios = new List<Exercicio>
            {
                new Exercicio
                {
                    ExercicioNome = "Flexão",
                    Descricao = "Exercício para peito, braços e ombros usando apenas o peso do corpo.",
                    Duracao = 10,
                    Intencidade = 6,
                    CaloriasGastas = 50,
                    Instrucoes = "Deitar no chão, mãos alinhadas aos ombros, flexionar braços mantendo corpo reto.",
                    Repeticoes = 15,
                    Series = 3,
                    ExercicioGeneros = new List<ExercicioGenero> { new ExercicioGenero { GeneroId = GetGenId("Unisexo") } },
                    ExercicioObjetivos = new List<ExercicioObjetivoFisico>
                    {
                        new ExercicioObjetivoFisico { ObjetivoFisicoId = GetObjId("Ganho de Massa Muscular") },
                        new ExercicioObjetivoFisico { ObjetivoFisicoId = GetObjId("Saúde Geral") }
                    },
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>
                    {
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Peito") },
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Tríceps") },
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Ombros") }
                    },
                    ExercicioTipoExercicios = new List<ExercicioTipoExercicio>
                    {
                        new ExercicioTipoExercicio
                        {
                            TipoExercicioId = GetTipoId("Força")
                        }
                    },
                    ExercicioEquipamentos = new List<ExercicioEquipamento> { new ExercicioEquipamento { EquipamentoId = GetEqId("Tapete de Yoga") } },
                    // Contraindicação: Quem tem tendinite no braço
                    Contraindicacoes = (GetProbId("Tendinite") > 0)
                        ? new List<ExercicioProblemaSaude> { new ExercicioProblemaSaude { ProblemaSaudeId = GetProbId("Tendinite") } }
                        : new List<ExercicioProblemaSaude>()
                },
                new Exercicio
                {
                    ExercicioNome = "Agachamento",
                    Descricao = "Exercício composto para membros inferiores.",
                    Duracao = 15,
                    Intencidade = 7,
                    CaloriasGastas = 80,
                    Instrucoes = "Ficar em pé, afastar pernas, flexionar joelhos mantendo costas retas.",
                    Repeticoes = 20,
                    Series = 4,
                    ExercicioGeneros = new List<ExercicioGenero> { new ExercicioGenero { GeneroId = GetGenId("Unisexo") } },
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>
                    {
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Pernas") },
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Glúteos") }
                    },
                    ExercicioEquipamentos = new List<ExercicioEquipamento>(), // Sem equipamento
                    // Contraindicação: Artrite nos joelhos
                    Contraindicacoes = (GetProbId("Artrite") > 0)
                        ? new List<ExercicioProblemaSaude> { new ExercicioProblemaSaude { ProblemaSaudeId = GetProbId("Artrite") } }
                        : new List<ExercicioProblemaSaude>()
                },
                new Exercicio
                {
                    ExercicioNome = "Prancha",
                    Descricao = "Exercício isométrico para core.",
                    Duracao = 5,
                    Intencidade = 5,
                    CaloriasGastas = 30,
                    Instrucoes = "Apoiar antebraços e ponta dos pés, mantendo corpo reto.",
                    Repeticoes = 1,
                    Series = 3,
                    ExercicioGeneros = new List<ExercicioGenero> { new ExercicioGenero { GeneroId = GetGenId("Unisexo") } },
                    ExercicioObjetivos = new List<ExercicioObjetivoFisico>
                    {
                        new ExercicioObjetivoFisico { ObjetivoFisicoId = GetObjId("Fortalecimento do Core") },
                        new ExercicioObjetivoFisico { ObjetivoFisicoId = GetObjId("Melhoria Postural") }
                    },
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular> { new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Abdómen") } },
                    ExercicioEquipamentos = new List<ExercicioEquipamento> { new ExercicioEquipamento { EquipamentoId = GetEqId("Tapete de Yoga") } },
                    Contraindicacoes = new List<ExercicioProblemaSaude>()
                },
                new Exercicio
                {
                    ExercicioNome = "Remada com Halteres",
                    Descricao = "Exercício para costas.",
                    Duracao = 12,
                    Intencidade = 7,
                    CaloriasGastas = 70,
                    Instrucoes = "Inclinar tronco, puxar halteres em direção ao abdómen.",
                    Repeticoes = 12,
                    Series = 3,
                    ExercicioGeneros = new List<ExercicioGenero>
                    {
                        new ExercicioGenero { GeneroId = GetGenId("Masculino") },
                        new ExercicioGenero { GeneroId = GetGenId("Feminino") }
                    },
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>
                    {
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Costas") },
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Bíceps") }
                    },
                    ExercicioEquipamentos = new List<ExercicioEquipamento> { new ExercicioEquipamento { EquipamentoId = GetEqId("Halteres") } },
                    // Contraindicação: Escoliose (pode agravar se má postura)
                    Contraindicacoes = (GetProbId("Escoliose") > 0)
                        ? new List<ExercicioProblemaSaude> { new ExercicioProblemaSaude { ProblemaSaudeId = GetProbId("Escoliose") } }
                        : new List<ExercicioProblemaSaude>()
                },
                new Exercicio
                {
                    ExercicioNome = "Corrida no Lugar",
                    Descricao = "Cardio para queimar calorias.",
                    Duracao = 20,
                    Intencidade = 6, // No original era 6, ajustei para 8 em alguns exemplos, mantendo 6
                    CaloriasGastas = 150,
                    Instrucoes = "Correr no mesmo lugar elevando os joelhos.",
                    Repeticoes = 1,
                    Series = 1,
                    ExercicioGeneros = new List<ExercicioGenero> { new ExercicioGenero { GeneroId = GetGenId("Unisexo") } },
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>
                    {
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Pernas") },
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Panturrilhas") }
                    },
                    ExercicioObjetivos = new List<ExercicioObjetivoFisico>
                    {
                        new ExercicioObjetivoFisico { ObjetivoFisicoId = GetObjId("Perda de Peso") },
                        new ExercicioObjetivoFisico { ObjetivoFisicoId = GetObjId("Resistência Cardiovascular") }
                    },
                    ExercicioTipoExercicios = new List<ExercicioTipoExercicio>
                    {
                        new ExercicioTipoExercicio
                        {
                            TipoExercicioId = GetTipoId("Cardiovascular")
                        }
                    },
                    ExercicioEquipamentos = new List<ExercicioEquipamento>(),
                    // Contraindicação: Asma (se alta intensidade)
                    Contraindicacoes = (GetProbId("Asma") > 0)
                        ? new List<ExercicioProblemaSaude> { new ExercicioProblemaSaude { ProblemaSaudeId = GetProbId("Asma") } }
                        : new List<ExercicioProblemaSaude>()
                },
                new Exercicio
                {
                    ExercicioNome = "Elevação Pélvica",
                    Descricao = "Focado em glúteos.",
                    Duracao = 10,
                    Intencidade = 5,
                    CaloriasGastas = 40,
                    Instrucoes = "Deitar, elevar a bacia contraindo glúteos.",
                    Repeticoes = 15,
                    Series = 3,
                    ExercicioGeneros = new List<ExercicioGenero>
                    {
                        new ExercicioGenero { GeneroId = GetGenId("Feminino") },
                        new ExercicioGenero { GeneroId = GetGenId("Unisexo") }
                    },
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>
                    {
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Glúteos") },
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Pernas") }
                    },
                    ExercicioEquipamentos = new List<ExercicioEquipamento> { new ExercicioEquipamento { EquipamentoId = GetEqId("Tapete de Yoga") } },
                    Contraindicacoes = new List<ExercicioProblemaSaude>()
                },
                new Exercicio
                {
                    ExercicioNome = "Elevação Lateral de Ombros",
                    Descricao = "Exercício isolado para os ombros.",
                    Duracao = 8,
                    Intencidade = 5,
                    CaloriasGastas = 30,
                    Instrucoes = "Elevar os halteres lateralmente até à altura dos ombros.",
                    Repeticoes = 12,
                    Series = 3,
                    ExercicioGeneros = new List<ExercicioGenero> { new ExercicioGenero { GeneroId = GetGenId("Unisexo") } },
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>
                    {
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Ombros") }
                    },
                    ExercicioEquipamentos = new List<ExercicioEquipamento>
                    {
                        new ExercicioEquipamento { EquipamentoId = GetEqId("Halteres") }
                    },
                    Contraindicacoes = new List<ExercicioProblemaSaude>()
                },

                new Exercicio
                {
                    ExercicioNome = "Curl de Bíceps",
                    Descricao = "Exercício clássico para fortalecimento do bíceps.",
                    Duracao = 8,
                    Intencidade = 6,
                    CaloriasGastas = 35,
                    Instrucoes = "Flexionar os braços trazendo os halteres até aos ombros.",
                    Repeticoes = 12,
                    Series = 3,
                    ExercicioGeneros = new List<ExercicioGenero> { new ExercicioGenero { GeneroId = GetGenId("Unisexo") } },
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>
                    {
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Bíceps") }
                    },
                    ExercicioEquipamentos = new List<ExercicioEquipamento>
                    {
                        new ExercicioEquipamento { EquipamentoId = GetEqId("Halteres") }
                    },
                    Contraindicacoes = new List<ExercicioProblemaSaude>()
                },

                new Exercicio
                {
                    ExercicioNome = "Extensão de Tríceps",
                    Descricao = "Exercício isolado para os tríceps.",
                    Duracao = 8,
                    Intencidade = 6,
                    CaloriasGastas = 35,
                    Instrucoes = "Estender os braços acima da cabeça segurando o halter.",
                    Repeticoes = 12,
                    Series = 3,
                    ExercicioGeneros = new List<ExercicioGenero> { new ExercicioGenero { GeneroId = GetGenId("Unisexo") } },
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>
                    {
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Tríceps") }
                    },
                    ExercicioEquipamentos = new List<ExercicioEquipamento>
                    {
                        new ExercicioEquipamento { EquipamentoId = GetEqId("Halteres") }
                    },
                    Contraindicacoes = new List<ExercicioProblemaSaude>()
                },

                new Exercicio
                {
                    ExercicioNome = "Leg Press",
                    Descricao = "Exercício de força para membros inferiores.",
                    Duracao = 12,
                    Intencidade = 7,
                    CaloriasGastas = 90,
                    Instrucoes = "Empurrar a plataforma com os pés mantendo costas apoiadas.",
                    Repeticoes = 12,
                    Series = 4,
                    ExercicioGeneros = new List<ExercicioGenero> { new ExercicioGenero { GeneroId = GetGenId("Unisexo") } },
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>
                    {
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Pernas") },
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Glúteos") }
                    },
                    ExercicioEquipamentos = new List<ExercicioEquipamento>
                    {
                        new ExercicioEquipamento { EquipamentoId = GetEqId("Máquina de Leg Press") }
                    },
                    Contraindicacoes = (GetProbId("Artrose") > 0)
                        ? new List<ExercicioProblemaSaude> { new ExercicioProblemaSaude { ProblemaSaudeId = GetProbId("Artrose") } }
                        : new List<ExercicioProblemaSaude>()
                },

                new Exercicio
                {
                    ExercicioNome = "Elevação de Gémeos",
                    Descricao = "Exercício para fortalecimento das panturrilhas.",
                    Duracao = 6,
                    Intencidade = 5,
                    CaloriasGastas = 25,
                    Instrucoes = "Elevar os calcanhares mantendo o corpo alinhado.",
                    Repeticoes = 20,
                    Series = 3,
                    ExercicioGeneros = new List<ExercicioGenero> { new ExercicioGenero { GeneroId = GetGenId("Unisexo") } },
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>
                    {
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Panturrilhas") }
                    },
                    ExercicioEquipamentos = new List<ExercicioEquipamento>(),
                    Contraindicacoes = new List<ExercicioProblemaSaude>()
                },
                new Exercicio
                {
                    ExercicioNome = "Puxada na Máquina",
                    Descricao = "Exercício para fortalecimento das costas.",
                    Duracao = 12,
                    Intencidade = 7,
                    CaloriasGastas = 75,
                    Instrucoes = "Puxar a barra da máquina em direção ao peito mantendo as costas direitas.",
                    Repeticoes = 12,
                    Series = 3,
                    ExercicioGeneros = new List<ExercicioGenero> { new ExercicioGenero { GeneroId = GetGenId("Unisexo") } },
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>
                    {
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Costas") }
                    },
                    ExercicioEquipamentos = new List<ExercicioEquipamento>
                    {
                        new ExercicioEquipamento { EquipamentoId = GetEqId("Máquina de Pulldown") }
                    },
                    Contraindicacoes = new List<ExercicioProblemaSaude>()
                },

                new Exercicio
                {
                    ExercicioNome = "Afundos",
                    Descricao = "Exercício unilateral para pernas e glúteos.",
                    Duracao = 12,
                    Intencidade = 6,
                    CaloriasGastas = 70,
                    Instrucoes = "Dar um passo à frente e flexionar ambos os joelhos.",
                    Repeticoes = 12,
                    Series = 3,
                    ExercicioGeneros = new List<ExercicioGenero> { new ExercicioGenero { GeneroId = GetGenId("Unisexo") } },
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>
                    {
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Pernas") },
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Glúteos") }
                    },
                    ExercicioEquipamentos = new List<ExercicioEquipamento>(),
                    Contraindicacoes = (GetProbId("Artrose") > 0)
                        ? new List<ExercicioProblemaSaude> { new ExercicioProblemaSaude { ProblemaSaudeId = GetProbId("Artrose") } }
                        : new List<ExercicioProblemaSaude>()
                },

                new Exercicio
                {
                    ExercicioNome = "Step Up",
                    Descricao = "Exercício funcional para pernas.",
                    Duracao = 10,
                    Intencidade = 5,
                    CaloriasGastas = 60,
                    Instrucoes = "Subir e descer de um step alternando as pernas.",
                    Repeticoes = 15,
                    Series = 3,
                    ExercicioGeneros = new List<ExercicioGenero> { new ExercicioGenero { GeneroId = GetGenId("Unisexo") } },
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>
                    {
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Pernas") }
                    },
                    ExercicioEquipamentos = new List<ExercicioEquipamento>
                    {
                        new ExercicioEquipamento { EquipamentoId = GetEqId("Step") }
                    },
                    Contraindicacoes = new List<ExercicioProblemaSaude>()
                },

                new Exercicio
                {
                    ExercicioNome = "Russian Twist",
                    Descricao = "Exercício para abdómen e oblíquos.",
                    Duracao = 8,
                    Intencidade = 6,
                    CaloriasGastas = 40,
                    Instrucoes = "Sentado, rodar o tronco alternando os lados.",
                    Repeticoes = 20,
                    Series = 3,
                    ExercicioGeneros = new List<ExercicioGenero> { new ExercicioGenero { GeneroId = GetGenId("Unisexo") } },
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>
                    {
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Abdómen") }
                    },
                    ExercicioEquipamentos = new List<ExercicioEquipamento>
                    {
                        new ExercicioEquipamento { EquipamentoId = GetEqId("Medicine Ball") }
                    },
                    Contraindicacoes = new List<ExercicioProblemaSaude>()
                },

                new Exercicio
                {
                    ExercicioNome = "Burpees",
                    Descricao = "Exercício de corpo inteiro de alta intensidade.",
                    Duracao = 10,
                    Intencidade = 8,
                    CaloriasGastas = 120,
                    Instrucoes = "Agachar, apoiar mãos no chão, saltar para prancha e voltar.",
                    Repeticoes = 10,
                    Series = 3,
                    ExercicioGeneros = new List<ExercicioGenero> { new ExercicioGenero { GeneroId = GetGenId("Unisexo") } },
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>
                    {
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Pernas") },
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Peito") },
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Abdómen") }
                    },
                    ExercicioTipoExercicios = new List<ExercicioTipoExercicio>
                    {
                        new ExercicioTipoExercicio
                        {
                            TipoExercicioId = GetTipoId("Resistência")
                        }
                    },
                    ExercicioEquipamentos = new List<ExercicioEquipamento>(),
                    Contraindicacoes = (GetProbId("Hipertensão Arterial") > 0)
                        ? new List<ExercicioProblemaSaude> { new ExercicioProblemaSaude { ProblemaSaudeId = GetProbId("Hipertensão Arterial") } }
                        : new List<ExercicioProblemaSaude>()
                },

                new Exercicio
                {
                    ExercicioNome = "Salto à Corda",
                    Descricao = "Exercício cardiovascular intenso.",
                    Duracao = 15,
                    Intencidade = 7,
                    CaloriasGastas = 130,
                    Instrucoes = "Saltar continuamente mantendo ritmo constante.",
                    Repeticoes = 1,
                    Series = 1,
                    ExercicioGeneros = new List<ExercicioGenero> { new ExercicioGenero { GeneroId = GetGenId("Unisexo") } },
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>
                    {
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Panturrilhas") },
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Pernas") }
                    },
                    ExercicioEquipamentos = new List<ExercicioEquipamento>
                    {
                        new ExercicioEquipamento { EquipamentoId = GetEqId("Corda de Saltar") }
                    },
                    Contraindicacoes = new List<ExercicioProblemaSaude>()
                },

                new Exercicio
                {
                    ExercicioNome = "Alongamento Geral",
                    Descricao = "Exercício leve para mobilidade e relaxamento.",
                    Duracao = 10,
                    Intencidade = 2,
                    CaloriasGastas = 15,
                    Instrucoes = "Alongar suavemente os principais grupos musculares.",
                    Repeticoes = 1,
                    Series = 1,
                    ExercicioGeneros = new List<ExercicioGenero> { new ExercicioGenero { GeneroId = GetGenId("Unisexo") } },
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>
                    {
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Core") }
                    },
                    ExercicioEquipamentos = new List<ExercicioEquipamento>
                    {
                        new ExercicioEquipamento { EquipamentoId = GetEqId("Tapete de Yoga") }
                    },
                    ExercicioObjetivos = new List<ExercicioObjetivoFisico>
                    {
                        new ExercicioObjetivoFisico { ObjetivoFisicoId = GetObjId("Mobilidade e Flexibilidade") }
                    },
                    ExercicioTipoExercicios = new List<ExercicioTipoExercicio>
                    {
                        new ExercicioTipoExercicio
                        {
                            TipoExercicioId = GetTipoId("Flexibilidade")
                        }
                    },
                    Contraindicacoes = new List<ExercicioProblemaSaude>()
                },
                new Exercicio
                {
                    ExercicioNome = "Mountain Climbers",
                    Descricao = "Exercício cardiovascular e de core.",
                    Duracao = 10,
                    Intencidade = 7,
                    CaloriasGastas = 100,
                    Instrucoes = "Em posição de prancha, alternar joelhos em direção ao peito.",
                    Repeticoes = 1,
                    Series = 3,
                    ExercicioGeneros = new List<ExercicioGenero> { new ExercicioGenero { GeneroId = GetGenId("Unisexo") } },
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>
                    {
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Abdómen") },
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Pernas") }
                    },
                    ExercicioEquipamentos = new List<ExercicioEquipamento>(),
                    Contraindicacoes = (GetProbId("Asma") > 0)
                        ? new List<ExercicioProblemaSaude> { new ExercicioProblemaSaude { ProblemaSaudeId = GetProbId("Asma") } }
                        : new List<ExercicioProblemaSaude>()
                },
                new Exercicio
                {
                    ExercicioNome = "Supino Reto",
                    Descricao = "Força para peitoral.",
                    Duracao = 15,
                    Intencidade = 8,
                    CaloriasGastas = 90,
                    Instrucoes = "Empurrar barra para cima deitado no banco.",
                    Repeticoes = 10,
                    Series = 4,
                    ExercicioGeneros = new List<ExercicioGenero> { new ExercicioGenero { GeneroId = GetGenId("Masculino") } },
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>
                    {
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Peito") },
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Tríceps") }
                    },
                    ExercicioEquipamentos = new List<ExercicioEquipamento>
                    {
                        new ExercicioEquipamento { EquipamentoId = GetEqId("Barra Olímpica") },
                        new ExercicioEquipamento { EquipamentoId = GetEqId("Banco de Musculação") }
                    },
                    // Contraindicação: Tendinite e Hipertensão
                    Contraindicacoes = new List<ExercicioProblemaSaude>()
                },
                new Exercicio
                {
                    ExercicioNome = "Abdominal Crunch",
                    Descricao = "Abdómen superior.",
                    Duracao = 7,
                    Intencidade = 5,
                    CaloriasGastas = 35,
                    Instrucoes = "Deitar, levantar ombros do chão.",
                    Repeticoes = 20,
                    Series = 3,
                    ExercicioGeneros = new List<ExercicioGenero> { new ExercicioGenero { GeneroId = GetGenId("Unisexo") } },
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular> { new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Abdómen") } },
                    ExercicioEquipamentos = new List<ExercicioEquipamento> { new ExercicioEquipamento { EquipamentoId = GetEqId("Tapete de Yoga") } },
                    Contraindicacoes = new List<ExercicioProblemaSaude>()
                }
            };

            // Lógica extra para adicionar múltiplas contraindicações ao Supino se existirem
            var supino = exercicios.FirstOrDefault(e => e.ExercicioNome == "Supino Reto");
            if (supino != null)
            {
                if (GetProbId("Tendinite") > 0) supino.Contraindicacoes.Add(new ExercicioProblemaSaude { ProblemaSaudeId = GetProbId("Tendinite") });
                if (GetProbId("Hipertensão Arterial") > 0) supino.Contraindicacoes.Add(new ExercicioProblemaSaude { ProblemaSaudeId = GetProbId("Hipertensão Arterial") });
            }

            dbContext.Exercicio.AddRange(exercicios);
            dbContext.SaveChanges();
        }

        public static class Roles
        {
            public const string Administrador = "Administrador";
            public const string Profissional = "ProfissionalSaude";
            public const string Utente = "Utente";
        }

        // Método para criar as Roles (Já tinhas, está correto)
        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { Roles.Administrador, Roles.Profissional, Roles.Utente };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        // A. ADMIN
        public static async Task SeedDefaultAdmin(UserManager<IdentityUser> userManager)
        {
            var adminEmail = "admin@ginasio.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                // A password deve ser forte
                var result = await userManager.CreateAsync(adminUser, "Admin@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, Roles.Administrador);
                }
            }
        }

        public static async Task SeedProfissional(
        UserManager<IdentityUser> userManager, // Liga à BD de Users
        HealthWellbeingDbContext dbContext)    // Liga à BD de Negócio
        {
            // 1. Criar o Login na BD "HealthWellbeingUsers"
            var profEmail = "medico@ginasio.com";
            var profUser = await userManager.FindByEmailAsync(profEmail);

            if (profUser == null)
            {
                profUser = new IdentityUser
                {
                    UserName = profEmail,
                    Email = profEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(profUser, "Medico@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(profUser, Roles.Profissional);
                }
            }

            // Importante: Recarregar o user para garantir que temos o ID correto
            profUser = await userManager.FindByEmailAsync(profEmail);

            // 2. Criar o Perfil na BD "HealthWellbeing"
            // Usamos o UserId (string) para fazer a ponte
            var perfilExiste = await dbContext.Profissional.AnyAsync(p => p.UserId == profUser.Id);

            if (!perfilExiste)
            {
                var novoProfissional = new Profissional
                {
                    UserId = profUser.Id, //ID da outra BD
                    Nome = "Dr. Exemplo",
                    Especialidade = "Fisioterapeuta",
                    NumeroCedula = "12345XYZ"
                };

                dbContext.Profissional.Add(novoProfissional);
                await dbContext.SaveChangesAsync();
            }
        }

        public static async Task SeedUtente(UserManager<IdentityUser> userManager, HealthWellbeingDbContext dbContext)
        {
            var utenteEmail = "utente@ginasio.com";
            var utenteUser = await userManager.FindByEmailAsync(utenteEmail);

            // 1. Criar Login
            if (utenteUser == null)
            {
                utenteUser = new IdentityUser
                {
                    UserName = utenteEmail,
                    Email = utenteEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(utenteUser, "Utente@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(utenteUser, Roles.Utente);
                }
            }
        }
    }
}