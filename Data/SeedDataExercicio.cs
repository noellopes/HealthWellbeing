using HealthWellbeing.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthWellbeing.Data
{
    public class SeedDataExercicio
    {
        internal static void Populate(HealthWellbeingDbContext? dbContext)
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));

            // Cria a base de dados se não existir
            dbContext.Database.EnsureCreated();

            PopulateExercicios(dbContext);
        }

        private static void PopulateExercicios(HealthWellbeingDbContext dbContext)
        {
            // 1. Verifica se já existem exercícios. Se sim, não faz nada.
            if (dbContext.Exercicio.Any()) return;

            // 2. Cria ou recupera os Gêneros (Masculino, Feminino, Unisexo)
            var generoMasculino = dbContext.Genero.FirstOrDefault(g => g.NomeGenero == "Masculino")
                                  ?? new Genero { NomeGenero = "Masculino" };

            var generoFeminino = dbContext.Genero.FirstOrDefault(g => g.NomeGenero == "Feminino")
                                 ?? new Genero { NomeGenero = "Feminino" };

            var generoUnisexo = dbContext.Genero.FirstOrDefault(g => g.NomeGenero == "Unisexo")
                                ?? new Genero { NomeGenero = "Unisexo" };

            // Adiciona ao contexto se forem novos (ID == 0)
            if (generoMasculino.GeneroId == 0) dbContext.Genero.Add(generoMasculino);
            if (generoFeminino.GeneroId == 0) dbContext.Genero.Add(generoFeminino);
            if (generoUnisexo.GeneroId == 0) dbContext.Genero.Add(generoUnisexo);

            dbContext.SaveChanges(); // Guarda os géneros para gerar os IDs reais

            // 3. Criação dos Exercícios com a associação ao Gênero correto
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
                    // A maioria dos exercícios básicos são para ambos os sexos
                    ExercicioGeneros = new List<ExercicioGenero>
                    {
                        new ExercicioGenero { GeneroId = generoUnisexo.GeneroId }
                    }
                },
                new Exercicio
                {
                    ExercicioNome = "Agachamento",
                    Descricao = "Exercício para pernas e glúteos.",
                    Duracao = 15,
                    Intencidade = 7,
                    CaloriasGastas = 80,
                    Instrucoes = "Ficar em pé, afastar pernas, flexionar joelhos mantendo costas retas.",
                    EquipamentoNecessario = "Nenhum",
                    Repeticoes = 20,
                    Series = 4,
                    ExercicioGeneros = new List<ExercicioGenero>
                    {
                        new ExercicioGenero { GeneroId = generoUnisexo.GeneroId }
                    }
                },
                new Exercicio
                {
                    ExercicioNome = "Prancha",
                    Descricao = "Exercício isométrico para abdómen e core.",
                    Duracao = 5,
                    Intencidade = 5,
                    CaloriasGastas = 30,
                    Instrucoes = "Ficar de bruços, apoiar antebraços e ponta dos pés, mantendo corpo reto.",
                    EquipamentoNecessario = "Nenhum",
                    Repeticoes = 1,
                    Series = 3,
                    ExercicioGeneros = new List<ExercicioGenero>
                    {
                        new ExercicioGenero { GeneroId = generoUnisexo.GeneroId }
                    }
                },
                new Exercicio
                {
                    ExercicioNome = "Remada com Halteres",
                    Descricao = "Exercício para costas e braços com halteres.",
                    Duracao = 12,
                    Intencidade = 7,
                    CaloriasGastas = 70,
                    Instrucoes = "Inclinar tronco à frente, segurar halteres, puxar em direção ao abdómen.",
                    EquipamentoNecessario = "Halteres",
                    Repeticoes = 12,
                    Series = 3,
                    ExercicioGeneros = new List<ExercicioGenero>
                    {
                        new ExercicioGenero { GeneroId = generoMasculino.GeneroId }, // Exemplo: se quiseres associar a Masculino E Feminino explicitamente em vez de Unisexo
                        new ExercicioGenero { GeneroId = generoFeminino.GeneroId }
                    }
                },
                new Exercicio
                {
                    ExercicioNome = "Corrida no Lugar",
                    Descricao = "Exercício cardiovascular para aumentar resistência e queimar calorias.",
                    Duracao = 20,
                    Intencidade = 6,
                    CaloriasGastas = 150,
                    Instrucoes = "Correr no mesmo lugar, levantando joelhos alternadamente.",
                    EquipamentoNecessario = "Nenhum",
                    Repeticoes = 1,
                    Series = 1,
                    ExercicioGeneros = new List<ExercicioGenero>
                    {
                        new ExercicioGenero { GeneroId = generoUnisexo.GeneroId }
                    }
                },
                new Exercicio
                {
                    ExercicioNome = "Elevação Pélvica", // Exemplo comum em treinos focados em glúteos
                    Descricao = "Exercício focado na região glútea.",
                    Duracao = 10,
                    Intencidade = 5,
                    CaloriasGastas = 40,
                    Instrucoes = "Deitar de costas, joelhos dobrados, elevar a bacia contraindo os glúteos.",
                    EquipamentoNecessario = "Nenhum",
                    Repeticoes = 15,
                    Series = 3,
                    ExercicioGeneros = new List<ExercicioGenero>
                    {
                        new ExercicioGenero { GeneroId = generoFeminino.GeneroId }, // Exemplo: focado em público feminino (embora homens façam)
                        new ExercicioGenero { GeneroId = generoUnisexo.GeneroId }
                    }
                },
                new Exercicio
                {
                    ExercicioNome = "Elevação Lateral de Ombros",
                    Descricao = "Exercício para fortalecer ombros com halteres.",
                    Duracao = 10,
                    Intencidade = 6,
                    CaloriasGastas = 45,
                    Instrucoes = "Em pé, segurar halteres ao lado do corpo e levantar lateralmente até à altura dos ombros.",
                    EquipamentoNecessario = "Halteres",
                    Repeticoes = 12,
                    Series = 3,
                    ExercicioGeneros = new List<ExercicioGenero>
                    {
                        new ExercicioGenero { GeneroId = generoUnisexo.GeneroId }
                    }
                },
                new Exercicio
                {
                    ExercicioNome = "Burpees",
                    Descricao = "Exercício completo que trabalha corpo inteiro e aumenta ritmo cardíaco.",
                    Duracao = 12,
                    Intencidade = 8,
                    CaloriasGastas = 100,
                    Instrucoes = "Agachar, colocar as mãos no chão, saltar para posição de prancha e voltar ao agachamento.",
                    EquipamentoNecessario = "Nenhum",
                    Repeticoes = 10,
                    Series = 3,
                    ExercicioGeneros = new List<ExercicioGenero>
                    {
                        new ExercicioGenero { GeneroId = generoUnisexo.GeneroId }
                    }
                },
                new Exercicio
                {
                    ExercicioNome = "Supino Reto", // Exemplo clássico associado a treino de peito masculino
                    Descricao = "Exercício de força para peitoral maior.",
                    Duracao = 15,
                    Intencidade = 8,
                    CaloriasGastas = 90,
                    Instrucoes = "Deitar no banco, descer a barra até ao peito e empurrar para cima.",
                    EquipamentoNecessario = "Barra e Banco",
                    Repeticoes = 10,
                    Series = 4,
                    ExercicioGeneros = new List<ExercicioGenero>
                    {
                        new ExercicioGenero { GeneroId = generoMasculino.GeneroId }, // Exemplo de target
                        new ExercicioGenero { GeneroId = generoUnisexo.GeneroId }
                    }
                },
                new Exercicio
                {
                    ExercicioNome = "Abdominal Crunch",
                    Descricao = "Exercício clássico para abdómen superior.",
                    Duracao = 7,
                    Intencidade = 5,
                    CaloriasGastas = 35,
                    Instrucoes = "Deitar de costas, dobrar joelhos, levantar ombros do chão e voltar lentamente.",
                    EquipamentoNecessario = "Nenhum",
                    Repeticoes = 20,
                    Series = 3,
                    ExercicioGeneros = new List<ExercicioGenero>
                    {
                        new ExercicioGenero { GeneroId = generoUnisexo.GeneroId }
                    }
                }
            };

            // Adiciona todos os exercícios ao DbContext
            dbContext.Exercicio.AddRange(exercicios);
            dbContext.SaveChanges();
        }
    }
}