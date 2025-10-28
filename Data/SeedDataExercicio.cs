using HealthWellbeing.Models;
using Microsoft.EntityFrameworkCore;

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
            // Se já houver exercícios, não faz nada
            if (dbContext.Exercicio.Any()) return;

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
                    Genero = "Unisexo"
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
                    Genero = "Unisexo"
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
                    Genero = "Unisexo"
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
                    Genero = "Unisexo"
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
                    Genero = "Unisexo"
                },
                new Exercicio
                {
                    ExercicioNome = "Elevação de Pernas",
                    Descricao = "Exercício para abdómen inferior e core.",
                    Duracao = 8,
                    Intencidade = 5,
                    CaloriasGastas = 40,
                    Instrucoes = "Deitar de costas, levantar pernas mantendo-as esticadas e descer lentamente.",
                    EquipamentoNecessario = "Nenhum",
                    Repeticoes = 15,
                    Series = 3,
                    Genero = "Unisexo"
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
                    Genero = "Unisexo"
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
                    Genero = "Unisexo"
                },
                new Exercicio
                {
                    ExercicioNome = "Alongamento de Pernas",
                    Descricao = "Exercício de flexibilidade para pernas e glúteos.",
                    Duracao = 5,
                    Intencidade = 3,
                    CaloriasGastas = 10,
                    Instrucoes = "Sentar, esticar pernas e alcançar os pés lentamente.",
                    EquipamentoNecessario = "Nenhum",
                    Repeticoes = 1,
                    Series = 2,
                    Genero = "Unisexo"
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
                    Genero = "Unisexo"
                }
            };

            // Adiciona todos os exercícios ao DbContext
            dbContext.Exercicio.AddRange(exercicios);
            dbContext.SaveChanges();
        }
    }
}
