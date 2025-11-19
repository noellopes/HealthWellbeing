using HealthWellbeing.Models;
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

            // 1. Gêneros (Dependência para Exercícios)
            PopulateGeneros(dbContext);

            // 2. Grupos Musculares e Músculos (Dependência para Exercícios)
            PopulateGruposMusculares(dbContext);

            // 3. Exercícios (Depende de 1 e 2)
            PopulateExercicios(dbContext);

            // 4. Problemas de Saúde (Independente)
            PopulateProblemasSaude(dbContext);

            // 5. Tipos de Exercício e Benefícios (Independente)
            PopulateTiposExercicio(dbContext);
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
            dbContext.SaveChanges(); // Salvar para gerar IDs
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
                new GrupoMuscular { GrupoMuscularNome = "Ombros", LocalizacaoCorporal = "Deltoides e trapézio" },
                new GrupoMuscular { GrupoMuscularNome = "Pernas", LocalizacaoCorporal = "Quadríceps, isquiotibiais e glúteos" },
                new GrupoMuscular { GrupoMuscularNome = "Abdômen", LocalizacaoCorporal = "Região abdominal" },
                new GrupoMuscular { GrupoMuscularNome = "Panturrilhas", LocalizacaoCorporal = "Região inferior da perna" },
                new GrupoMuscular { GrupoMuscularNome = "Antebraços", LocalizacaoCorporal = "Parte inferior do braço" },
                new GrupoMuscular { GrupoMuscularNome = "Trapézio", LocalizacaoCorporal = "Parte superior das costas e pescoço" },
                new GrupoMuscular { GrupoMuscularNome = "Glúteos", LocalizacaoCorporal = "Região das nádegas" },
                new GrupoMuscular { GrupoMuscularNome = "Adutores", LocalizacaoCorporal = "Parte interna das coxas" },
                new GrupoMuscular { GrupoMuscularNome = "Abdutores", LocalizacaoCorporal = "Parte externa das coxas" },
                new GrupoMuscular { GrupoMuscularNome = "Serrátil Anterior", LocalizacaoCorporal = "Lateral do tórax" },
                new GrupoMuscular { GrupoMuscularNome = "Reto Femoral", LocalizacaoCorporal = "Parte frontal da coxa" }
            };

            dbContext.GrupoMuscular.AddRange(grupos);
            dbContext.SaveChanges(); // Salvar para gerar IDs necessários para os músculos

            // --- Criação dos Músculos (Associados aos grupos criados acima) ---
            // Recarregar grupos da BD para garantir que temos os IDs corretos
            var gruposDb = dbContext.GrupoMuscular.ToList();
            Func<string, int> getId = nome => gruposDb.First(g => g.GrupoMuscularNome == nome).GrupoMuscularId;

            var musculos = new[]
            {
                new Musculo { Nome_Musculo = "Peitoral Maior", GrupoMuscularId = getId("Peito") },
                new Musculo { Nome_Musculo = "Peitoral Menor", GrupoMuscularId = getId("Peito") },
                new Musculo { Nome_Musculo = "Dorsal Largo", GrupoMuscularId = getId("Costas") },
                new Musculo { Nome_Musculo = "Romboides", GrupoMuscularId = getId("Costas") },
                new Musculo { Nome_Musculo = "Bíceps Braquial", GrupoMuscularId = getId("Bíceps") },
                new Musculo { Nome_Musculo = "Tríceps Braquial", GrupoMuscularId = getId("Tríceps") },
                new Musculo { Nome_Musculo = "Deltoide Anterior", GrupoMuscularId = getId("Ombros") },
                new Musculo { Nome_Musculo = "Deltoide Lateral", GrupoMuscularId = getId("Ombros") },
                new Musculo { Nome_Musculo = "Quadríceps", GrupoMuscularId = getId("Pernas") },
                new Musculo { Nome_Musculo = "Isquiotibiais", GrupoMuscularId = getId("Pernas") },
                new Musculo { Nome_Musculo = "Glúteo Máximo", GrupoMuscularId = getId("Glúteos") },
                new Musculo { Nome_Musculo = "Reto Abdominal", GrupoMuscularId = getId("Abdômen") },
                new Musculo { Nome_Musculo = "Gastrocnêmio", GrupoMuscularId = getId("Panturrilhas") }
            };

            dbContext.Musculo.AddRange(musculos);
            dbContext.SaveChanges();
        }

        private static void PopulateExercicios(HealthWellbeingDbContext dbContext)
        {
            if (dbContext.Exercicio.Any()) return;

            // Recuperar referências para fazer as ligações
            var generos = dbContext.Genero.ToDictionary(g => g.NomeGenero, g => g.GeneroId);
            var grupos = dbContext.GrupoMuscular.ToDictionary(g => g.GrupoMuscularNome, g => g.GrupoMuscularId);

            // Helper para obter ID de forma segura
            int GetGenId(string nome) => generos.ContainsKey(nome) ? generos[nome] : generos.Values.First();
            int GetGrId(string nome) => grupos.ContainsKey(nome) ? grupos[nome] : grupos.Values.First();

            var exercicios = new[]
            {
                new Exercicio
                {
                    ExercicioNome = "Flexão",
                    Descricao = "Exercício para peito, braços e ombros usando apenas o peso do corpo.",
                    Duracao = 10,
                    Intencidade = 6,
                    CaloriasGastas = 50,
                    Instrucoes = "Deitar no chão, mãos alinhadas aos ombros, flexionar braços mantendo corpo reto.",
                    EquipamentoNecessario = "Nenhum",
                    Repeticoes = 15,
                    Series = 3,
                    // Associação Gênero
                    ExercicioGeneros = new List<ExercicioGenero> { new ExercicioGenero { GeneroId = GetGenId("Unisexo") } },
                    // CORREÇÃO: Associação Grupo Muscular (Peito e Tríceps)
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>
                    {
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Peito") },
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Tríceps") },
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Ombros") }
                    }
                },
                new Exercicio
                {
                    ExercicioNome = "Agachamento",
                    Descricao = "Exercício composto para membros inferiores.",
                    Duracao = 15,
                    Intencidade = 7,
                    CaloriasGastas = 80,
                    Instrucoes = "Ficar em pé, afastar pernas, flexionar joelhos mantendo costas retas.",
                    EquipamentoNecessario = "Nenhum",
                    Repeticoes = 20,
                    Series = 4,
                    ExercicioGeneros = new List<ExercicioGenero> { new ExercicioGenero { GeneroId = GetGenId("Unisexo") } },
                    // CORREÇÃO: Associação Grupo Muscular (Pernas e Glúteos)
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>
                    {
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Pernas") },
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Glúteos") }
                    }
                },
                new Exercicio
                {
                    ExercicioNome = "Prancha",
                    Descricao = "Exercício isométrico para core.",
                    Duracao = 5,
                    Intencidade = 5,
                    CaloriasGastas = 30,
                    Instrucoes = "Apoiar antebraços e ponta dos pés, mantendo corpo reto.",
                    EquipamentoNecessario = "Nenhum",
                    Repeticoes = 1,
                    Series = 3,
                    ExercicioGeneros = new List<ExercicioGenero> { new ExercicioGenero { GeneroId = GetGenId("Unisexo") } },
                    // CORREÇÃO: Associação Grupo Muscular (Abdômen)
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>
                    {
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Abdômen") }
                    }
                },
                new Exercicio
                {
                    ExercicioNome = "Remada com Halteres",
                    Descricao = "Exercício para costas.",
                    Duracao = 12,
                    Intencidade = 7,
                    CaloriasGastas = 70,
                    Instrucoes = "Inclinar tronco, puxar halteres em direção ao abdómen.",
                    EquipamentoNecessario = "Halteres",
                    Repeticoes = 12,
                    Series = 3,
                    ExercicioGeneros = new List<ExercicioGenero>
                    {
                        new ExercicioGenero { GeneroId = GetGenId("Masculino") },
                        new ExercicioGenero { GeneroId = GetGenId("Feminino") }
                    },
                    // CORREÇÃO: Associação Grupo Muscular (Costas e Bíceps)
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>
                    {
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Costas") },
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Bíceps") }
                    }
                },
                new Exercicio
                {
                    ExercicioNome = "Corrida no Lugar",
                    Descricao = "Cardio para queimar calorias.",
                    Duracao = 20,
                    Intencidade = 6,
                    CaloriasGastas = 150,
                    Instrucoes = "Correr no mesmo lugar elevando os joelhos.",
                    EquipamentoNecessario = "Nenhum",
                    Repeticoes = 1,
                    Series = 1,
                    ExercicioGeneros = new List<ExercicioGenero> { new ExercicioGenero { GeneroId = GetGenId("Unisexo") } },
                    // CORREÇÃO: Associação Grupo Muscular (Pernas e Panturrilhas)
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>
                    {
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Pernas") },
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Panturrilhas") }
                    }
                },
                new Exercicio
                {
                    ExercicioNome = "Elevação Pélvica",
                    Descricao = "Focado em glúteos.",
                    Duracao = 10,
                    Intencidade = 5,
                    CaloriasGastas = 40,
                    Instrucoes = "Deitar, elevar a bacia contraindo glúteos.",
                    EquipamentoNecessario = "Nenhum",
                    Repeticoes = 15,
                    Series = 3,
                    ExercicioGeneros = new List<ExercicioGenero>
                    {
                        new ExercicioGenero { GeneroId = GetGenId("Feminino") },
                        new ExercicioGenero { GeneroId = GetGenId("Unisexo") }
                    },
                    // CORREÇÃO: Associação Grupo Muscular (Glúteos e Pernas)
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>
                    {
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Glúteos") },
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Pernas") }
                    }
                },
                new Exercicio
                {
                    ExercicioNome = "Supino Reto",
                    Descricao = "Força para peitoral.",
                    Duracao = 15,
                    Intencidade = 8,
                    CaloriasGastas = 90,
                    Instrucoes = "Empurrar barra para cima deitado no banco.",
                    EquipamentoNecessario = "Barra e Banco",
                    Repeticoes = 10,
                    Series = 4,
                    ExercicioGeneros = new List<ExercicioGenero> { new ExercicioGenero { GeneroId = GetGenId("Masculino") } },
                    // CORREÇÃO: Associação Grupo Muscular (Peito e Tríceps)
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>
                    {
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Peito") },
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Tríceps") }
                    }
                },
                new Exercicio
                {
                    ExercicioNome = "Abdominal Crunch",
                    Descricao = "Abdómen superior.",
                    Duracao = 7,
                    Intencidade = 5,
                    CaloriasGastas = 35,
                    Instrucoes = "Deitar, levantar ombros do chão.",
                    EquipamentoNecessario = "Nenhum",
                    Repeticoes = 20,
                    Series = 3,
                    ExercicioGeneros = new List<ExercicioGenero> { new ExercicioGenero { GeneroId = GetGenId("Unisexo") } },
                    // CORREÇÃO: Associação Grupo Muscular (Abdômen)
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>
                    {
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Abdômen") }
                    }
                }
            };

            dbContext.Exercicio.AddRange(exercicios);
            dbContext.SaveChanges();
        }

        private static void PopulateProblemasSaude(HealthWellbeingDbContext dbContext)
        {
            if (dbContext.ProblemaSaude.Any()) return;

            var problemas = new[]
            {
                new ProblemaSaude { ProblemaCategoria = "Muscular", ProblemaNome = "Tendinite", ZonaAtingida = "Braço direito", Gravidade = 6 },
                new ProblemaSaude { ProblemaCategoria = "Cardíaco", ProblemaNome = "Hipertensão Arterial", ZonaAtingida = "Coração", Gravidade = 8 },
                new ProblemaSaude { ProblemaCategoria = "Respiratório", ProblemaNome = "Asma", ZonaAtingida = "Pulmões", Gravidade = 5 },
                new ProblemaSaude { ProblemaCategoria = "Articular", ProblemaNome = "Artrite", ZonaAtingida = "Joelhos", Gravidade = 7 },
                new ProblemaSaude { ProblemaCategoria = "Neurológico", ProblemaNome = "Enxaqueca Crónica", ZonaAtingida = "Cabeça", Gravidade = 6 },
                new ProblemaSaude { ProblemaCategoria = "Postural", ProblemaNome = "Escoliose", ZonaAtingida = "Coluna Vertebral", Gravidade = 5 },
                new ProblemaSaude { ProblemaCategoria = "Endócrino", ProblemaNome = "Diabetes Tipo 2", ZonaAtingida = "Sistema Endócrino", Gravidade = 8 },
                new ProblemaSaude { ProblemaCategoria = "Ocular", ProblemaNome = "Miopia", ZonaAtingida = "Olhos", Gravidade = 3 }
            };

            dbContext.ProblemaSaude.AddRange(problemas);
            dbContext.SaveChanges();
        }

        private static void PopulateTiposExercicio(HealthWellbeingDbContext dbContext)
        {
            // 1. Se não houver benefícios, cria
            if (!dbContext.Beneficio.Any())
            {
                var novosBeneficios = new[]
                {
                    new Beneficio { NomeBeneficio = "Melhora da resistência cardiovascular", DescricaoBeneficio = "Aumenta a capacidade do coração..." },
                    new Beneficio { NomeBeneficio = "Fortalecimento muscular", DescricaoBeneficio = "Aumenta a força..." },
                    new Beneficio { NomeBeneficio = "Aumento da flexibilidade", DescricaoBeneficio = "Melhora a amplitude..." },
                    new Beneficio { NomeBeneficio = "Redução do stress", DescricaoBeneficio = "Ajuda a reduzir a ansiedade..." },
                    new Beneficio { NomeBeneficio = "Perda de peso", DescricaoBeneficio = "Auxilia no controle do peso..." }
                };
                dbContext.Beneficio.AddRange(novosBeneficios);
                dbContext.SaveChanges();
            }

            if (dbContext.TipoExercicio.Any()) return;

            // Recuperar Benefícios para ligar
            var beneficiosDb = dbContext.Beneficio.ToDictionary(b => b.NomeBeneficio, b => b);
            Beneficio GetBen(string nome) => beneficiosDb.ContainsKey(nome) ? beneficiosDb[nome] : beneficiosDb.First().Value;

            var tipoExercicios = new[]
            {
                new TipoExercicio
                {
                    NomeTipoExercicios = "Cardiovascular",
                    DescricaoTipoExercicios = "Atividades que aumentam a frequência cardíaca.",
                    CaracteristicasTipoExercicios = "Alta queima calórica.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { Beneficio = GetBen("Melhora da resistência cardiovascular") },
                        new TipoExercicioBeneficio { Beneficio = GetBen("Perda de peso") }
                    }
                },
                new TipoExercicio
                {
                    NomeTipoExercicios = "Força",
                    DescricaoTipoExercicios = "Exercícios focados no fortalecimento.",
                    CaracteristicasTipoExercicios = "Uso de pesos.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { Beneficio = GetBen("Fortalecimento muscular") },
                        new TipoExercicioBeneficio { Beneficio = GetBen("Perda de peso") }
                    }
                },
                new TipoExercicio
                {
                    NomeTipoExercicios = "Flexibilidade",
                    DescricaoTipoExercicios = "Atividades que melhoram o alongamento.",
                    CaracteristicasTipoExercicios = "Movimentos suaves.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { Beneficio = GetBen("Aumento da flexibilidade") },
                        new TipoExercicioBeneficio { Beneficio = GetBen("Redução do stress") }
                    }
                },
                new TipoExercicio
                {
                    NomeTipoExercicios = "Funcional",
                    DescricaoTipoExercicios = "Exercícios que simulam movimentos do dia a dia.",
                    CaracteristicasTipoExercicios = "Trabalho com peso corporal.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { Beneficio = GetBen("Fortalecimento muscular") },
                        new TipoExercicioBeneficio { Beneficio = GetBen("Redução do stress") }
                    }
                }
            };

            dbContext.TipoExercicio.AddRange(tipoExercicios);
            dbContext.SaveChanges();
        }
    }
}