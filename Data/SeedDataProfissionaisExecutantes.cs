// Data/SeedDataProfissionalExecutante.cs (CORRIGIDO)
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellBeing.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HealthWellbeing.Data // <<-- Corrigido o namespace se necessário
{
    public class SeedDataProfissionalExecutante
    {

        private static void PopulateProfissionais(HealthWellbeingDbContext dbContext) // <<-- Nome do DbContext Corrigido
        {
            if (dbContext.ProfissionalExecutante.Any()) return;

            

            var profissionais = new[]
            {
                new ProfissionalExecutante
                {
                    Nome = "André Kandonga",
                    Funcao = "Técnico de Radiologia",
                    Telefone = "912912915",
                    Email = "Kandonga123@gmail.com"
                },
                new ProfissionalExecutante
                {
                    Nome = "Miguel Santos",
                    Funcao = "Fisioterapeuta",
                    Telefone = "912912914",
                    Email = "MiguelSantos123@gmail.com"
                },
                new ProfissionalExecutante
                {
                    Nome = "Dostoevsky",
                    Funcao = "Técnico de Cardiopneumologia",
                    Telefone = "912913914",
                    Email = "DostoevskySuba@gmail.com"
                },
                new ProfissionalExecutante
                {
                    Nome = "Ricardo Quaresma",
                    Funcao = "Técnico de Análises Clínicas",
                    Telefone = "910101010",
                    Email = "QuaresmaPorto@gmail.com"
                },
                new ProfissionalExecutante
                {
                    Nome = "Mai Da Silva",
                    Funcao = "Terapeuta Ocupacional em Funções",
                    Telefone = "912912222",
                    Email = "Mai123222suba@gmail.com",
                },
                new ProfissionalExecutante
                {
                    Nome = "Diogo Rodrigues",
                    Funcao = "Ortopedista",
                    Telefone = "912912522",
                    Email = "DiogoRodrigues04@gmail.com",
                },
            };

            dbContext.ProfissionalExecutante.AddRange(profissionais);
            dbContext.SaveChanges();
        }
    }
}