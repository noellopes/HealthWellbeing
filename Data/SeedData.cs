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
                Gravidade = GravidadeAlergia.Grave,
                Sintomas = "Urticária, dificuldade para respirar, inchaço na garganta.",
                AlimentoId = 1 
            },
            new Alergia
            {
                Nome = "Alergia ao Leite",
                Descricao = "Sensibilidade às proteínas do leite de vaca.",
                Gravidade = GravidadeAlergia.Moderada,
                Sintomas = "Cólicas, diarreia, erupções cutâneas.",
                AlimentoId = 1
            },
            new Alergia
            {
                Nome = "Alergia ao Ovo",
                Descricao = "Reação imunológica às proteínas da clara ou gema do ovo.",
                Gravidade = GravidadeAlergia.Leve,
                Sintomas = "Coceira, vermelhidão, desconforto gastrointestinal.",
                AlimentoId = 1
            },
            new Alergia
            {
                Nome = "Alergia ao Trigo",
                Descricao = "Reação às proteínas do trigo, incluindo o glúten.",
                Gravidade = GravidadeAlergia.Moderada,
                Sintomas = "Inchaço abdominal, erupções cutâneas, fadiga.",
                AlimentoId = 1
            },
            new Alergia
            {
                Nome = "Alergia a Frutos do Mar",
                Descricao = "Reação a crustáceos e moluscos.",
                Gravidade = GravidadeAlergia.Moderada,
                Sintomas = "Inchaço facial, vômitos, anafilaxia.",
                AlimentoId = 1
            }
        );

        context.SaveChanges();
    }
}
