using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Data
{
    internal class SeedDataMetaCorporal
    {
        public static void Populate(HealthWellbeingDbContext dbContext)
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));

            // Check if MetaCorporal data already exists
            if (dbContext.MetaCorporal.Any())
            {
                return; // Data already seeded
            }

            // Get all clients from database
            var clients = dbContext.Client.ToList();

            if (!clients.Any())
            {
                return; // No clients to seed goals for
            }

            var metasCorporais = new List<MetaCorporal>();

            // Create sample goals for each client
            foreach (var client in clients)
            {
                // Create a realistic goal based on common health objectives
                var meta = new MetaCorporal
                {
                    ClientId = client.ClientId,
                    PesoObjetivo = 75.0m, // Target weight
                    GorduraCorporalObjetivo = 18.0m, // Target body fat %
                    ColesterolObjetivo = 180.0m, // Target cholesterol
                    IMCObjetivo = 23.5m, // Target BMI (healthy range)
                    MassaMuscularObjetivo = 35.0m, // Target muscle mass
                    DataInicio = DateTime.Now.AddMonths(-3), // Started 3 months ago
                    DataObjetivo = DateTime.Now.AddMonths(6), // Goal in 6 months
                    Notas = "Objetivo de emagrecimento saudável com ganho de massa muscular",
                    CriadoEm = DateTime.Now.AddMonths(-3),
                    CriadoPor = "system",
                    Ativo = true
                };

                metasCorporais.Add(meta);
            }

            // Add specific goals for variety (if we have at least 3 clients)
            if (metasCorporais.Count >= 3)
            {
                // Client 1: Weight loss goal
                metasCorporais[0].PesoObjetivo = 70.0m;
                metasCorporais[0].GorduraCorporalObjetivo = 15.0m;
                metasCorporais[0].IMCObjetivo = 22.0m;
                metasCorporais[0].Notas = "Objetivo principal: perda de peso e redução de gordura corporal";

                // Client 2: Muscle gain goal
                metasCorporais[1].PesoObjetivo = 85.0m;
                metasCorporais[1].GorduraCorporalObjetivo = 12.0m;
                metasCorporais[1].MassaMuscularObjetivo = 45.0m;
                metasCorporais[1].Notas = "Objetivo principal: ganho de massa muscular magra";

                // Client 3: Health improvement goal
                metasCorporais[2].ColesterolObjetivo = 170.0m;
                metasCorporais[2].IMCObjetivo = 24.0m;
                metasCorporais[2].Notas = "Objetivo principal: melhorar indicadores de saúde, especialmente colesterol";
            }

            // Add more varied goals if we have more clients
            for (int i = 3; i < metasCorporais.Count; i++)
            {
                // Randomize goals slightly for variety
                metasCorporais[i].PesoObjetivo = 70.0m + (i % 3) * 5.0m;
                metasCorporais[i].GorduraCorporalObjetivo = 15.0m + (i % 4) * 2.0m;
                metasCorporais[i].ColesterolObjetivo = 175.0m + (i % 3) * 5.0m;
                metasCorporais[i].IMCObjetivo = 22.0m + (i % 3) * 1.0m;
                metasCorporais[i].MassaMuscularObjetivo = 33.0m + (i % 4) * 3.0m;
            }

            // Save to database
            dbContext.MetaCorporal.AddRange(metasCorporais);
            dbContext.SaveChanges();
        }
    }
}
