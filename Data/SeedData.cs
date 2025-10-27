using HealthWellbeing.Data;
using HealthWellbeing.Models;

internal class SeedData
{
    public static void Populate(HealthWellbeingDbContext? context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        context.Database.EnsureCreated();

        PopulateAlergias(context);
    }

    private static void PopulateAlergias(HealthWellbeingDbContext context)
    {
        // Só adiciona dados se ainda não existir nenhuma alergia cadastrada
        if (context.Alergia.Any())
        {
            return;
        }

        context.Alergia.AddRange(
            new Alergia
            {
                Nome = "Alergia ao Amendoim",
                Descricao = "Reação alérgica grave a proteínas do amendoim.",
                Gravidade = "Alta",
                Sintomas = "Urticária, dificuldade para respirar, inchaço na garganta.",
                AlimentoId = 2 
            },
            new Alergia
            {
                Nome = "Alergia ao Leite",
                Descricao = "Sensibilidade às proteínas do leite de vaca.",
                Gravidade = "Média",
                Sintomas = "Cólicas, diarreia, erupções cutâneas.",
                AlimentoId = 2
            },
            new Alergia
            {
                Nome = "Alergia ao Ovo",
                Descricao = "Reação imunológica às proteínas da clara ou gema do ovo.",
                Gravidade = "Baixa",
                Sintomas = "Coceira, vermelhidão, desconforto gastrointestinal.",
                AlimentoId = 2
            },
            new Alergia
            {
                Nome = "Alergia ao Trigo",
                Descricao = "Reação às proteínas do trigo, incluindo o glúten.",
                Gravidade = "Média",
                Sintomas = "Inchaço abdominal, erupções cutâneas, fadiga.",
                AlimentoId = 2
            },
            new Alergia
            {
                Nome = "Alergia a Frutos do Mar",
                Descricao = "Reação a crustáceos e moluscos.",
                Gravidade = "Alta",
                Sintomas = "Inchaço facial, vômitos, anafilaxia.",
                AlimentoId = 2
            }
        );

        context.SaveChanges();
    }
}
