using HealthWellbeing.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Data
{
    public class SeedDataProblemaSaude
    {
        internal static void Populate(HealthWellbeingDbContext? dbContext)
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));

            // Cria a base de dados se não existir
            dbContext.Database.EnsureCreated();

            PopulateProblemasSaude(dbContext);
        }

        private static void PopulateProblemasSaude(HealthWellbeingDbContext dbContext)
        {
            // Se já houver problemas de saúde, não faz nada
            if (dbContext.ProblemaSaude.Any()) return;

            var problemas = new[]
            {
                new ProblemaSaude
                {
                    ProblemaCategoria = "Muscular",
                    ProblemaNome = "Tendinite",
                    ZonaAtingida = "Braço direito",
                    ProfissionalDeApoio = "Fisioterapeuta",
                    Gravidade = 6
                },
                new ProblemaSaude
                {
                    ProblemaCategoria = "Cardíaco",
                    ProblemaNome = "Hipertensão Arterial",
                    ZonaAtingida = "Coração",
                    ProfissionalDeApoio = "Cardiologista",
                    Gravidade = 8
                },
                new ProblemaSaude
                {
                    ProblemaCategoria = "Respiratório",
                    ProblemaNome = "Asma",
                    ZonaAtingida = "Pulmões",
                    ProfissionalDeApoio = "Pneumologista",
                    Gravidade = 5
                },
                new ProblemaSaude
                {
                    ProblemaCategoria = "Articular",
                    ProblemaNome = "Artrite",
                    ZonaAtingida = "Joelhos",
                    ProfissionalDeApoio = "Reumatologista",
                    Gravidade = 7
                },
                new ProblemaSaude
                {
                    ProblemaCategoria = "Neurológico",
                    ProblemaNome = "Enxaqueca Crónica",
                    ZonaAtingida = "Cabeça",
                    ProfissionalDeApoio = "Neurologista",
                    Gravidade = 6
                },
                new ProblemaSaude
                {
                    ProblemaCategoria = "Postural",
                    ProblemaNome = "Escoliose",
                    ZonaAtingida = "Coluna Vertebral",
                    ProfissionalDeApoio = "Ortopedista",
                    Gravidade = 5
                },
                new ProblemaSaude
                {
                    ProblemaCategoria = "Mental",
                    ProblemaNome = "Ansiedade Generalizada",
                    ZonaAtingida = "Sistema Nervoso",
                    ProfissionalDeApoio = "Psicólogo",
                    Gravidade = 7
                },
                new ProblemaSaude
                {
                    ProblemaCategoria = "Endócrino",
                    ProblemaNome = "Diabetes Tipo 2",
                    ZonaAtingida = "Sistema Endócrino",
                    ProfissionalDeApoio = "Endocrinologista",
                    Gravidade = 8
                },
                new ProblemaSaude
                {
                    ProblemaCategoria = "Dermatológico",
                    ProblemaNome = "Dermatite Atópica",
                    ZonaAtingida = "Pele",
                    ProfissionalDeApoio = "Dermatologista",
                    Gravidade = 4
                },
                new ProblemaSaude
                {
                    ProblemaCategoria = "Ocular",
                    ProblemaNome = "Miopia",
                    ZonaAtingida = "Olhos",
                    ProfissionalDeApoio = "Oftalmologista",
                    Gravidade = 3
                }
            };

            // Adiciona todos os problemas de saúde ao DbContext
            dbContext.ProblemaSaude.AddRange(problemas);
            dbContext.SaveChanges();
        }
    }
}
