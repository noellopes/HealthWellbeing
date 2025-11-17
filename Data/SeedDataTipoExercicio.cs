using HealthWellbeing.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Data
{
    public class SeedDataTipoExercicio
    {
        internal static void Populate(HealthWellbeingDbContext? dbContext)
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));

            dbContext.Database.EnsureCreated();

            PopulateTipoExercicios(dbContext);
        }

        private static void PopulateTipoExercicios(HealthWellbeingDbContext dbContext)
        {
            // Se já houver tipos de exercício ou benefícios, não faz nada
            if (dbContext.TipoExercicio.Any() || dbContext.Beneficio.Any()) return;

            // ------------------------------
            // Benefícios fictícios
            // ------------------------------
            var beneficios = new[]
            {
                new Beneficio
                {
                    NomeBeneficio = "Melhora da resistência cardiovascular",
                    DescricaoBeneficio = "Aumenta a capacidade do coração e pulmões, melhorando a circulação sanguínea e a respiração."
                },
                new Beneficio
                {
                    NomeBeneficio = "Fortalecimento muscular",
                    DescricaoBeneficio = "Aumenta a força e a massa muscular, contribuindo para uma melhor postura e desempenho físico."
                },
                new Beneficio
                {
                    NomeBeneficio = "Aumento da flexibilidade",
                    DescricaoBeneficio = "Melhora a amplitude de movimento das articulações e previne lesões musculares."
                },
                new Beneficio
                {
                    NomeBeneficio = "Redução do stress",
                    DescricaoBeneficio = "Ajuda a reduzir a ansiedade e melhora o humor através da liberação de endorfinas."
                },
                new Beneficio
                {
                    NomeBeneficio = "Perda de peso",
                    DescricaoBeneficio = "Auxilia no controle do peso corporal através do aumento do gasto calórico."
                }
            };
            dbContext.Beneficio.AddRange(beneficios);
            dbContext.SaveChanges();

            // ------------------------------
            // Tipos de exercício fictícios
            // ------------------------------
            var tipoExercicios = new[]
            {
                new TipoExercicio
                {
                    NomeTipoExercicios = "Cardiovascular",
                    DescricaoTipoExercicios = "Atividades que aumentam a frequência cardíaca e melhoram a resistência aeróbica.",
                    CaracteristicasTipoExercicios = "Alta queima calórica, movimento contínuo, melhora da capacidade respiratória.",
                    Beneficios = new List<Beneficio> { beneficios[0], beneficios[4], beneficios[3] } // resistência, perda de peso, stress
                },
                new TipoExercicio
                {
                    NomeTipoExercicios = "Força",
                    DescricaoTipoExercicios = "Exercícios focados no fortalecimento dos músculos e ossos.",
                    CaracteristicasTipoExercicios = "Uso de pesos ou resistência corporal, aumento da força e massa muscular.",
                    Beneficios = new List<Beneficio> { beneficios[1], beneficios[4] } // força, perda de peso
                },
                new TipoExercicio
                {
                    NomeTipoExercicios = "Flexibilidade",
                    DescricaoTipoExercicios = "Atividades que melhoram o alongamento e mobilidade articular.",
                    CaracteristicasTipoExercicios = "Movimentos suaves, foco em amplitude e postura.",
                    Beneficios = new List<Beneficio> { beneficios[2], beneficios[3] } // flexibilidade, stress
                },
                new TipoExercicio
                {
                    NomeTipoExercicios = "Funcional",
                    DescricaoTipoExercicios = "Exercícios que simulam movimentos do dia a dia, melhorando equilíbrio e coordenação.",
                    CaracteristicasTipoExercicios = "Trabalho com peso corporal, foco em movimentos compostos e equilíbrio.",
                    Beneficios = new List<Beneficio> { beneficios[1], beneficios[3], beneficios[4] } // força, stress, perda de peso
                },
                new TipoExercicio
                {
                    NomeTipoExercicios = "Alongamento",
                    DescricaoTipoExercicios = "Atividades voltadas para relaxamento muscular e mobilidade.",
                    CaracteristicasTipoExercicios = "Movimentos lentos e controlados, foco na respiração e relaxamento.",
                    Beneficios = new List<Beneficio> { beneficios[2], beneficios[3] } // flexibilidade, stress
                }
            };

            dbContext.TipoExercicio.AddRange(tipoExercicios);
            dbContext.SaveChanges();
        }
    }
}
