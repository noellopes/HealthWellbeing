using HealthWellbeing.Models;
using Microsoft.EntityFrameworkCore;
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
            // Se já houver tipos de exercício ou benefícios, não faz nada
            if (dbContext.TipoExercicio.Any() || dbContext.Beneficio.Any()) return;

            // ------------------------------
            // 1. Criar e Salvar os Benefícios
            // ------------------------------
            var beneficios = new[]
            {
                new Beneficio // Index 0
                {
                    NomeBeneficio = "Melhora da resistência cardiovascular",
                    DescricaoBeneficio = "Aumenta a capacidade do coração e pulmões, melhorando a circulação sanguínea e a respiração."
                },
                new Beneficio // Index 1
                {
                    NomeBeneficio = "Fortalecimento muscular",
                    DescricaoBeneficio = "Aumenta a força e a massa muscular, contribuindo para uma melhor postura e desempenho físico."
                },
                new Beneficio // Index 2
                {
                    NomeBeneficio = "Aumento da flexibilidade",
                    DescricaoBeneficio = "Melhora a amplitude de movimento das articulações e previne lesões musculares."
                },
                new Beneficio // Index 3
                {
                    NomeBeneficio = "Redução do stress",
                    DescricaoBeneficio = "Ajuda a reduzir a ansiedade e melhora o humor através da liberação de endorfinas."
                },
                new Beneficio // Index 4
                {
                    NomeBeneficio = "Perda de peso",
                    DescricaoBeneficio = "Auxilia no controle do peso corporal através do aumento do gasto calórico."
                }
            };

            dbContext.Beneficio.AddRange(beneficios);
            dbContext.SaveChanges();

            // ------------------------------
            // 2. Criar Tipos de Exercício com a Relação N:N
            // ------------------------------
            var tipoExercicios = new[]
            {
                new TipoExercicio
                {
                    NomeTipoExercicios = "Cardiovascular",
                    DescricaoTipoExercicios = "Atividades que aumentam a frequência cardíaca e melhoram a resistência aeróbica.",
                    CaracteristicasTipoExercicios = "Alta queima calórica, movimento contínuo, melhora da capacidade respiratória.",
                    // MUDANÇA AQUI: Usar a tabela intermédia
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { Beneficio = beneficios[0] }, // Resistência
                        new TipoExercicioBeneficio { Beneficio = beneficios[4] }, // Perda de peso
                        new TipoExercicioBeneficio { Beneficio = beneficios[3] }  // Stress
                    }
                },
                new TipoExercicio
                {
                    NomeTipoExercicios = "Força",
                    DescricaoTipoExercicios = "Exercícios focados no fortalecimento dos músculos e ossos.",
                    CaracteristicasTipoExercicios = "Uso de pesos ou resistência corporal, aumento da força e massa muscular.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { Beneficio = beneficios[1] }, // Força
                        new TipoExercicioBeneficio { Beneficio = beneficios[4] }  // Perda de peso
                    }
                },
                new TipoExercicio
                {
                    NomeTipoExercicios = "Flexibilidade",
                    DescricaoTipoExercicios = "Atividades que melhoram o alongamento e mobilidade articular.",
                    CaracteristicasTipoExercicios = "Movimentos suaves, foco em amplitude e postura.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { Beneficio = beneficios[2] }, // Flexibilidade
                        new TipoExercicioBeneficio { Beneficio = beneficios[3] }  // Stress
                    }
                },
                new TipoExercicio
                {
                    NomeTipoExercicios = "Funcional",
                    DescricaoTipoExercicios = "Exercícios que simulam movimentos do dia a dia, melhorando equilíbrio e coordenação.",
                    CaracteristicasTipoExercicios = "Trabalho com peso corporal, foco em movimentos compostos e equilíbrio.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { Beneficio = beneficios[1] }, // Força
                        new TipoExercicioBeneficio { Beneficio = beneficios[3] }, // Stress
                        new TipoExercicioBeneficio { Beneficio = beneficios[4] }  // Perda de peso
                    }
                },
                new TipoExercicio
                {
                    NomeTipoExercicios = "Alongamento",
                    DescricaoTipoExercicios = "Atividades voltadas para relaxamento muscular e mobilidade.",
                    CaracteristicasTipoExercicios = "Movimentos lentos e controlados, foco na respiração e relaxamento.",
                    TipoExercicioBeneficios = new List<TipoExercicioBeneficio>
                    {
                        new TipoExercicioBeneficio { Beneficio = beneficios[2] }, // Flexibilidade
                        new TipoExercicioBeneficio { Beneficio = beneficios[3] }  // Stress
                    }
                }
            };

            dbContext.TipoExercicio.AddRange(tipoExercicios);
            dbContext.SaveChanges();
        }
    }
}