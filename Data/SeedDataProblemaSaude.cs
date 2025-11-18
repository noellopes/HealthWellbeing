using HealthWellbeing.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Data
{
    public class SeedDataProblemaSaude
    {
        public static void Populate(HealthWellbeingDbContext dbContext)
        {
            // Garante que a BD existe
            dbContext.Database.EnsureCreated();

            // Se já houver problemas de saúde, não faz nada
            if (dbContext.ProblemaSaude.Any()) return;

            var problemas = new[]
            {
                new ProblemaSaude
                {
                    ProblemaCategoria = "Muscular",
                    ProblemaNome = "Tendinite",
                    ZonaAtingida = "Braço direito",
                    Gravidade = 6
                },
                new ProblemaSaude
                {
                    ProblemaCategoria = "Cardíaco",
                    ProblemaNome = "Hipertensão Arterial",
                    ZonaAtingida = "Coração",
                    Gravidade = 8
                },
                new ProblemaSaude
                {
                    ProblemaCategoria = "Respiratório",
                    ProblemaNome = "Asma",
                    ZonaAtingida = "Pulmões",
                    Gravidade = 5
                },
                new ProblemaSaude
                {
                    ProblemaCategoria = "Articular",
                    ProblemaNome = "Artrite",
                    ZonaAtingida = "Joelhos",
                    Gravidade = 7
                },
                new ProblemaSaude
                {
                    ProblemaCategoria = "Neurológico",
                    ProblemaNome = "Enxaqueca Crónica",
                    ZonaAtingida = "Cabeça",
                    Gravidade = 6
                },
                new ProblemaSaude
                {
                    ProblemaCategoria = "Postural",
                    ProblemaNome = "Escoliose",
                    ZonaAtingida = "Coluna Vertebral",
                    Gravidade = 5
                },
                new ProblemaSaude
                {
                    ProblemaCategoria = "Mental",
                    ProblemaNome = "Ansiedade Generalizada",
                    ZonaAtingida = "Sistema Nervoso",
                    Gravidade = 7
                },
                new ProblemaSaude
                {
                    ProblemaCategoria = "Endócrino",
                    ProblemaNome = "Diabetes Tipo 2",
                    ZonaAtingida = "Sistema Endócrino",
                    Gravidade = 8
                },
                new ProblemaSaude
                {
                    ProblemaCategoria = "Dermatológico",
                    ProblemaNome = "Dermatite Atópica",
                    ZonaAtingida = "Pele",
                    Gravidade = 4
                },
                new ProblemaSaude
                {
                    ProblemaCategoria = "Ocular",
                    ProblemaNome = "Miopia",
                    ZonaAtingida = "Olhos",
                    Gravidade = 3
                }
            };

            // Adiciona todos os problemas de saúde ao DbContext
            dbContext.ProblemaSaude.AddRange(problemas);
            dbContext.SaveChanges();
        }
    }
}