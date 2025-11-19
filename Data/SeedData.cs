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

            // 3. Equipamentos (NOVO - Dependência para Exercícios)
            PopulateEquipamentos(dbContext);

            // 4. Exercícios (Depende de 1, 2 e 3)
            PopulateExercicios(dbContext);

            // 5. Problemas de Saúde (Independente)
            PopulateProblemasSaude(dbContext);

            // 6. Tipos de Exercício e Benefícios (Independente)
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
            dbContext.SaveChanges();

            // --- Criação dos Músculos ---
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

        // --- NOVO MÉTODO: POPULAR EQUIPAMENTOS ---
        private static void PopulateEquipamentos(HealthWellbeingDbContext dbContext)
        {
            if (dbContext.Equipamento.Any()) return;

            var equipamentos = new[]
            {
                new Equipamento { NomeEquipamento = "Halteres" },
                new Equipamento { NomeEquipamento = "Barra Olímpica" },
                new Equipamento { NomeEquipamento = "Banco de Musculação" },
                new Equipamento { NomeEquipamento = "Tapete de Yoga" },
                new Equipamento { NomeEquipamento = "Bola de Pilates" },
                new Equipamento { NomeEquipamento = "Elásticos de Resistência" },
                new Equipamento { NomeEquipamento = "Passadeira" },
                new Equipamento { NomeEquipamento = "Bicicleta Estática" },
                new Equipamento { NomeEquipamento = "Kettlebell" }
            };

            dbContext.Equipamento.AddRange(equipamentos);
            dbContext.SaveChanges();
        }

        private static void PopulateExercicios(HealthWellbeingDbContext dbContext)
        {
            if (dbContext.Exercicio.Any()) return;

            // Recuperar referências para fazer as ligações
            var generos = dbContext.Genero.ToDictionary(g => g.NomeGenero, g => g.GeneroId);
            var grupos = dbContext.GrupoMuscular.ToDictionary(g => g.GrupoMuscularNome, g => g.GrupoMuscularId);
            var equipamentos = dbContext.Equipamento.ToDictionary(e => e.NomeEquipamento, e => e.EquipamentoId);

            // Helpers para obter IDs de forma segura
            int GetGenId(string nome) => generos.ContainsKey(nome) ? generos[nome] : generos.Values.First();
            int GetGrId(string nome) => grupos.ContainsKey(nome) ? grupos[nome] : grupos.Values.First();
            int GetEqId(string nome) => equipamentos.ContainsKey(nome) ? equipamentos[nome] : 0;

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
                    Repeticoes = 15,
                    Series = 3,
                    ExercicioGeneros = new List<ExercicioGenero> { new ExercicioGenero { GeneroId = GetGenId("Unisexo") } },
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>
                    {
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Peito") },
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Tríceps") },
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Ombros") }
                    },
                    // Flexão geralmente não precisa de equipamento, mas podemos por Tapete se quisermos
                    ExercicioEquipamentos = new List<ExercicioEquipamento>
                    {
                        new ExercicioEquipamento { EquipamentoId = GetEqId("Tapete de Yoga") }
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
                    Repeticoes = 20,
                    Series = 4,
                    ExercicioGeneros = new List<ExercicioGenero> { new ExercicioGenero { GeneroId = GetGenId("Unisexo") } },
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>
                    {
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Pernas") },
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Glúteos") }
                    },
                    ExercicioEquipamentos = new List<ExercicioEquipamento>() // Sem equipamento
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
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>
                    {
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Abdômen") }
                    },
                    ExercicioEquipamentos = new List<ExercicioEquipamento>
                    {
                        new ExercicioEquipamento { EquipamentoId = GetEqId("Tapete de Yoga") }
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
                    // Associa Halteres
                    ExercicioEquipamentos = new List<ExercicioEquipamento>
                    {
                        new ExercicioEquipamento { EquipamentoId = GetEqId("Halteres") }
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
                    Repeticoes = 1,
                    Series = 1,
                    ExercicioGeneros = new List<ExercicioGenero> { new ExercicioGenero { GeneroId = GetGenId("Unisexo") } },
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>
                    {
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Pernas") },
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Panturrilhas") }
                    },
                    ExercicioEquipamentos = new List<ExercicioEquipamento>() // Sem equipamento
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
                    ExercicioEquipamentos = new List<ExercicioEquipamento>
                    {
                        new ExercicioEquipamento { EquipamentoId = GetEqId("Tapete de Yoga") }
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
                    Repeticoes = 10,
                    Series = 4,
                    ExercicioGeneros = new List<ExercicioGenero> { new ExercicioGenero { GeneroId = GetGenId("Masculino") } },
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>
                    {
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Peito") },
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Tríceps") }
                    },
                    // Associa Barra e Banco
                    ExercicioEquipamentos = new List<ExercicioEquipamento>
                    {
                        new ExercicioEquipamento { EquipamentoId = GetEqId("Barra Olímpica") },
                        new ExercicioEquipamento { EquipamentoId = GetEqId("Banco de Musculação") }
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
                    Repeticoes = 20,
                    Series = 3,
                    ExercicioGeneros = new List<ExercicioGenero> { new ExercicioGenero { GeneroId = GetGenId("Unisexo") } },
                    ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>
                    {
                        new ExercicioGrupoMuscular { GrupoMuscularId = GetGrId("Abdômen") }
                    },
                    ExercicioEquipamentos = new List<ExercicioEquipamento>
                    {
                        new ExercicioEquipamento { EquipamentoId = GetEqId("Tapete de Yoga") }
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