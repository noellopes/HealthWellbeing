using HealthWellbeing.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HealthWellbeing.Data
{
    // A classe SeedDataProfissionalExecutante deve ser pública
    public class SeedDataProfissionalExecutante
    {
        // ✅ MÉTODO DE ENTRADA CORRIGIDO: Este método é chamado a partir do Program.cs
        public static void Populate(HealthWellbeingDbContext? dbContext)
        {
            if (dbContext == null) return;

            // Chama o método que insere os dados
            PopulateProfissionais(dbContext);
        }

        private static void PopulateProfissionais(HealthWellbeingDbContext dbContext)
        {
            // Se os Profissionais Executantes já existirem, não faz nada
            if (dbContext.ProfissionalExecutante.Any()) return;

            // --- 1. INSERIR OS PROFISSIONAIS ---
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
            // ✅ Salva todas as alterações na base de dados
            dbContext.SaveChanges();
        }
    }
}