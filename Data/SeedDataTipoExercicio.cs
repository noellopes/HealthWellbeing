using HealthWellbeing.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthWellbeing.Data
{
    public class SeedDataTipoExercicio
    {
        internal static void Populate(HealthWellbeingDbContext? dbContext)
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));

            PopulateTipoExercicios(dbContext);
        }

        private static void PopulateTipoExercicios(HealthWellbeingDbContext dbContext)
        {
            // ------------------------------
            // 1. Garantir que os Benefícios existem
            // ------------------------------
            if (!dbContext.Beneficio.Any())
            {
                var novosBeneficios = new[]
                {
                    new Beneficio { NomeBeneficio = "Melhora da resistência cardiovascular", DescricaoBeneficio = "Aumenta a capacidade do coração e pulmões..." },
                    new Beneficio { NomeBeneficio = "Fortalecimento muscular", DescricaoBeneficio = "Aumenta a força e a massa muscular..." },
                    new Beneficio { NomeBeneficio = "Aumento da flexibilidade", DescricaoBeneficio = "Melhora a amplitude de movimento..." },
                    new Beneficio { NomeBeneficio = "Redução do stress", DescricaoBeneficio = "Ajuda a reduzir a ansiedade..." },
                    new Beneficio { NomeBeneficio = "Perda de peso", DescricaoBeneficio = "Auxilia no controle do peso..." }
                };

                dbContext.Beneficio.AddRange(novosBeneficios);
                dbContext.SaveChanges(); 
            }

            // ------------------------------
            // 2. Verificar se já existem Exercícios (Se sim, sai)
            // ------------------------------
            if (dbContext.TipoExercicio.Any()) return;

            // ------------------------------
            // 3. Recuperar os Benefícios da BD
            // ------------------------------
            
            var beneficiosDb = dbContext.Beneficio.ToDictionary(b => b.NomeBeneficio, b => b);

            // Verificação de segurança caso falte algum benefício
            if (beneficiosDb.Count == 0) return;

            // Helpers para buscar o objeto facilmente pelo nome (evita erros de índice)
            Beneficio GetBen(string nome) => beneficiosDb.ContainsKey(nome) ? beneficiosDb[nome] : beneficiosDb.First().Value;

            // ------------------------------
            // 4. Criar Tipos de Exercício
            // ------------------------------
            var tipoExercicios = new[]
            {
                new TipoExercicio
                {
                    NomeTipoExercicios = "Cardiovascular",
                    DescricaoTipoExercicios = "Atividades que aumentam a frequência cardíaca.",
                    CaracteristicasTipoExercicios = "Alta queima calórica, movimento contínuo.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { Beneficio = GetBen("Melhora da resistência cardiovascular") },
                        new TipoExercicioBeneficio { Beneficio = GetBen("Perda de peso") },
                        new TipoExercicioBeneficio { Beneficio = GetBen("Redução do stress") }
                    }
                },
                new TipoExercicio
                {
                    NomeTipoExercicios = "Força",
                    DescricaoTipoExercicios = "Exercícios focados no fortalecimento.",
                    CaracteristicasTipoExercicios = "Uso de pesos ou resistência corporal.",
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
                    CaracteristicasTipoExercicios = "Movimentos suaves, foco em amplitude.",
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
                    CaracteristicasTipoExercicios = "Trabalho com peso corporal, equilíbrio.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { Beneficio = GetBen("Fortalecimento muscular") },
                        new TipoExercicioBeneficio { Beneficio = GetBen("Redução do stress") },
                        new TipoExercicioBeneficio { Beneficio = GetBen("Perda de peso") }
                    }
                },
                new TipoExercicio
                {
                    NomeTipoExercicios = "Alongamento",
                    DescricaoTipoExercicios = "Atividades voltadas para relaxamento muscular.",
                    CaracteristicasTipoExercicios = "Movimentos lentos, foco na respiração.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { Beneficio = GetBen("Aumento da flexibilidade") },
                        new TipoExercicioBeneficio { Beneficio = GetBen("Redução do stress") }
                    }
                }
            };

            dbContext.TipoExercicio.AddRange(tipoExercicios);
            dbContext.SaveChanges();
        }
    }
}