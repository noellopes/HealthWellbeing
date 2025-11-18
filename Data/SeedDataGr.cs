using HealthWellbeing.Models;

namespace HealthWellbeing.Data
{
    public class SeedDataGr
    {
        internal static void Populate(HealthWellbeingDbContext? dbContext)
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));

            // Cria a base de dados se não existir
            dbContext.Database.EnsureCreated();

            PopulateGrupoMuscular(dbContext);
            PopulateMusculo(dbContext);
        }

        private static void PopulateGrupoMuscular(HealthWellbeingDbContext dbContext)
        {
            if (dbContext.GrupoMuscular.Any()) return;

            var gruposMusculares = new[]
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
                new GrupoMuscular { GrupoMuscularNome = "Dorsal Largo", LocalizacaoCorporal = "Laterais das costas" },
                new GrupoMuscular { GrupoMuscularNome = "Glúteos", LocalizacaoCorporal = "Região das nádegas" },
                new GrupoMuscular { GrupoMuscularNome = "Adutores", LocalizacaoCorporal = "Parte interna das coxas" },
                new GrupoMuscular { GrupoMuscularNome = "Abdutores", LocalizacaoCorporal = "Parte externa das coxas" },
                new GrupoMuscular { GrupoMuscularNome = "Serrátil Anterior", LocalizacaoCorporal = "Lateral do tórax, próximo às costelas" },
                new GrupoMuscular { GrupoMuscularNome = "Reto Femoral", LocalizacaoCorporal = "Parte frontal da coxa" },
                new GrupoMuscular { GrupoMuscularNome = "Isquiotibiais", LocalizacaoCorporal = "Parte posterior da coxa" },
                new GrupoMuscular { GrupoMuscularNome = "Trapézio Inferior", LocalizacaoCorporal = "Parte média e inferior das costas" }
            };


            dbContext.GrupoMuscular.AddRange(gruposMusculares);
            dbContext.SaveChanges();
        }

        private static void PopulateMusculo(HealthWellbeingDbContext dbContext)
        {
            if (dbContext.Musculo.Any()) return;

            // Primeiro, obter os grupos musculares do banco
            var grupos = dbContext.GrupoMuscular.ToList();

            // Criar músculos associados a cada grupo
            var musculos = new[]
            {
                new Musculo { Nome_Musculo = "Peitoral Maior", GrupoMuscularId = grupos.First(g => g.GrupoMuscularNome == "Peito").GrupoMuscularId },
                new Musculo { Nome_Musculo = "Peitoral Menor", GrupoMuscularId = grupos.First(g => g.GrupoMuscularNome == "Peito").GrupoMuscularId },
                new Musculo { Nome_Musculo = "Dorsal Largo", GrupoMuscularId = grupos.First(g => g.GrupoMuscularNome == "Costas").GrupoMuscularId },
                new Musculo { Nome_Musculo = "Romboides", GrupoMuscularId = grupos.First(g => g.GrupoMuscularNome == "Costas").GrupoMuscularId },
                new Musculo { Nome_Musculo = "Eretor da Espinha", GrupoMuscularId = grupos.First(g => g.GrupoMuscularNome == "Costas").GrupoMuscularId },
                new Musculo { Nome_Musculo = "Bíceps Braquial", GrupoMuscularId = grupos.First(g => g.GrupoMuscularNome == "Bíceps").GrupoMuscularId },
                new Musculo { Nome_Musculo = "Braquial", GrupoMuscularId = grupos.First(g => g.GrupoMuscularNome == "Bíceps").GrupoMuscularId },
                new Musculo { Nome_Musculo = "Tríceps Braquial", GrupoMuscularId = grupos.First(g => g.GrupoMuscularNome == "Tríceps").GrupoMuscularId },
                new Musculo { Nome_Musculo = "Ancôneo", GrupoMuscularId = grupos.First(g => g.GrupoMuscularNome == "Tríceps").GrupoMuscularId },
                new Musculo { Nome_Musculo = "Deltoide Anterior", GrupoMuscularId = grupos.First(g => g.GrupoMuscularNome == "Ombros").GrupoMuscularId },
                new Musculo { Nome_Musculo = "Deltoide Lateral", GrupoMuscularId = grupos.First(g => g.GrupoMuscularNome == "Ombros").GrupoMuscularId },
                new Musculo { Nome_Musculo = "Deltoide Posterior", GrupoMuscularId = grupos.First(g => g.GrupoMuscularNome == "Ombros").GrupoMuscularId },
                new Musculo { Nome_Musculo = "Quadríceps", GrupoMuscularId = grupos.First(g => g.GrupoMuscularNome == "Pernas").GrupoMuscularId },
                new Musculo { Nome_Musculo = "Isquiotibiais", GrupoMuscularId = grupos.First(g => g.GrupoMuscularNome == "Pernas").GrupoMuscularId },
                new Musculo { Nome_Musculo = "Glúteo Máximo", GrupoMuscularId = grupos.First(g => g.GrupoMuscularNome == "Pernas").GrupoMuscularId },
                new Musculo { Nome_Musculo = "Glúteo Médio", GrupoMuscularId = grupos.First(g => g.GrupoMuscularNome == "Pernas").GrupoMuscularId },
                new Musculo { Nome_Musculo = "Adutores", GrupoMuscularId = grupos.First(g => g.GrupoMuscularNome == "Adutores").GrupoMuscularId },
                new Musculo { Nome_Musculo = "Abdutores", GrupoMuscularId = grupos.First(g => g.GrupoMuscularNome == "Abdutores").GrupoMuscularId },
                new Musculo { Nome_Musculo = "Reto Abdominal", GrupoMuscularId = grupos.First(g => g.GrupoMuscularNome == "Abdômen").GrupoMuscularId },
                new Musculo { Nome_Musculo = "Oblíquos Externos", GrupoMuscularId = grupos.First(g => g.GrupoMuscularNome == "Abdômen").GrupoMuscularId },
                new Musculo { Nome_Musculo = "Oblíquos Internos", GrupoMuscularId = grupos.First(g => g.GrupoMuscularNome == "Abdômen").GrupoMuscularId },
                new Musculo { Nome_Musculo = "Gastrocnêmio", GrupoMuscularId = grupos.First(g => g.GrupoMuscularNome == "Panturrilhas").GrupoMuscularId },
                new Musculo { Nome_Musculo = "Sóleo", GrupoMuscularId = grupos.First(g => g.GrupoMuscularNome == "Panturrilhas").GrupoMuscularId },
                new Musculo { Nome_Musculo = "Flexores do Antebraço", GrupoMuscularId = grupos.First(g => g.GrupoMuscularNome == "Antebraços").GrupoMuscularId },
                new Musculo { Nome_Musculo = "Extensores do Antebraço", GrupoMuscularId = grupos.First(g => g.GrupoMuscularNome == "Antebraços").GrupoMuscularId },
                new Musculo { Nome_Musculo = "Trapézio Superior", GrupoMuscularId = grupos.First(g => g.GrupoMuscularNome == "Trapézio").GrupoMuscularId },
                new Musculo { Nome_Musculo = "Trapézio Médio", GrupoMuscularId = grupos.First(g => g.GrupoMuscularNome == "Trapézio").GrupoMuscularId },
                new Musculo { Nome_Musculo = "Trapézio Inferior", GrupoMuscularId = grupos.First(g => g.GrupoMuscularNome == "Trapézio").GrupoMuscularId },
                new Musculo { Nome_Musculo = "Serrátil Anterior", GrupoMuscularId = grupos.First(g => g.GrupoMuscularNome == "Serrátil Anterior").GrupoMuscularId },
                new Musculo { Nome_Musculo = "Reto Femoral", GrupoMuscularId = grupos.First(g => g.GrupoMuscularNome == "Reto Femoral").GrupoMuscularId }
            };


            dbContext.Musculo.AddRange(musculos);
            dbContext.SaveChanges();
        }
    }
}
