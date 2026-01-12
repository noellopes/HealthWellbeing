using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Data
{
    internal class SeedDataProgressRecord
    {
        public static void Populate(HealthWellbeingDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));

            // Check if ProgressRecord data already exists
            if (dbContext.ProgressRecord.Any())
            {
                return; // Data already seeded
            }

            // Get all clients from database
            var clients = dbContext.Client.ToList();

            if (!clients.Any())
            {
                return; // No clients to seed records for
            }

            // Get nutritionist user ID
            var nutritionist = userManager.FindByEmailAsync("nutri@health.com").GetAwaiter().GetResult();
            if (nutritionist == null)
            {
                return; // No nutritionist found
            }

            var progressRecords = new List<ProgressRecord>();

            // Create realistic progress records for each client
            foreach (var client in clients)
            {
                // Generate 3-6 progress records per client over the last 6 months
                var numberOfRecords = new Random().Next(3, 7);
                var startDate = DateTime.Now.AddMonths(-6);

                // Starting metrics (varied by client)
                var baseWeight = 70.0m + (clients.IndexOf(client) % 5) * 5.0m; // 70-90 kg
                var baseBodyFat = 25.0m + (clients.IndexOf(client) % 4) * 2.0m; // 25-31%
                var baseCholesterol = 200.0m + (clients.IndexOf(client) % 3) * 10.0m; // 200-220
                var baseBMI = 25.0m + (clients.IndexOf(client) % 4) * 1.5m; // 25-29.5
                var baseMuscleMass = 30.0m + (clients.IndexOf(client) % 3) * 2.0m; // 30-34 kg

                for (int i = 0; i < numberOfRecords; i++)
                {
                    // Calculate record date (spread over 6 months)
                    var recordDate = startDate.AddDays((180.0 / numberOfRecords) * i);

                    // Simulate gradual improvement over time
                    var progressFactor = (decimal)i / numberOfRecords;

                    // Weight: gradually decreasing for most clients
                    var weight = baseWeight - (progressFactor * (clients.IndexOf(client) % 2 == 0 ? 5.0m : 2.0m));
                    
                    // Body fat: gradually decreasing
                    var bodyFat = baseBodyFat - (progressFactor * 3.0m);
                    
                    // Cholesterol: gradually decreasing
                    var cholesterol = baseCholesterol - (progressFactor * 15.0m);
                    
                    // BMI: gradually decreasing (correlated with weight)
                    var bmi = baseBMI - (progressFactor * 2.0m);
                    
                    // Muscle mass: gradually increasing (for some clients)
                    var muscleMass = baseMuscleMass + (progressFactor * (clients.IndexOf(client) % 3 == 0 ? 3.0m : 1.0m));

                    // Add some realistic variation (±5%)
                    var random = new Random(client.ClientId.GetHashCode() + i);
                    weight *= (1.0m + ((decimal)random.NextDouble() - 0.5m) * 0.05m);
                    bodyFat *= (1.0m + ((decimal)random.NextDouble() - 0.5m) * 0.05m);
                    cholesterol *= (1.0m + ((decimal)random.NextDouble() - 0.5m) * 0.05m);
                    bmi *= (1.0m + ((decimal)random.NextDouble() - 0.5m) * 0.05m);
                    muscleMass *= (1.0m + ((decimal)random.NextDouble() - 0.5m) * 0.05m);

                    var record = new ProgressRecord
                    {
                        ClientId = client.ClientId,
                        NutritionistId = nutritionist.Id,
                        Weight = Math.Round(weight, 1),
                        BodyFatPercentage = Math.Round(bodyFat, 1),
                        Cholesterol = Math.Round(cholesterol, 0),
                        BMI = Math.Round(bmi, 1),
                        MuscleMass = Math.Round(muscleMass, 1),
                        RecordDate = recordDate,
                        Notes = GenerateNotes(i, numberOfRecords),
                        CreatedAt = recordDate.AddHours(1),
                        CreatedBy = nutritionist.Id
                    };

                    progressRecords.Add(record);
                }
            }

            // Add more varied scenarios for specific clients if we have enough
            if (clients.Count >= 3)
            {
                // Client 1: Excellent progress (weight loss)
                var client1Records = progressRecords.Where(pr => pr.ClientId == clients[0].ClientId).OrderBy(pr => pr.RecordDate).ToList();
                for (int i = 0; i < client1Records.Count; i++)
                {
                    var progress = (decimal)i / client1Records.Count;
                    client1Records[i].Weight = Math.Round(85.0m - (progress * 10.0m), 1);
                    client1Records[i].BodyFatPercentage = Math.Round(28.0m - (progress * 6.0m), 1);
                    client1Records[i].BMI = Math.Round(28.5m - (progress * 3.5m), 1);
                    client1Records[i].Notes = i == 0 ? "Avaliação inicial - Objetivo: perda de peso" :
                        i == client1Records.Count - 1 ? "Excelente progresso! Meta quase alcançada." :
                        $"Boa evolução. Cliente motivado e comprometido com o plano.";
                }

                // Client 2: Muscle gain focus
                var client2Records = progressRecords.Where(pr => pr.ClientId == clients[1].ClientId).OrderBy(pr => pr.RecordDate).ToList();
                for (int i = 0; i < client2Records.Count; i++)
                {
                    var progress = (decimal)i / client2Records.Count;
                    client2Records[i].Weight = Math.Round(70.0m + (progress * 8.0m), 1);
                    client2Records[i].BodyFatPercentage = Math.Round(20.0m - (progress * 4.0m), 1);
                    client2Records[i].MuscleMass = Math.Round(32.0m + (progress * 8.0m), 1);
                    client2Records[i].BMI = Math.Round(23.0m + (progress * 2.0m), 1);
                    client2Records[i].Notes = i == 0 ? "Avaliação inicial - Objetivo: ganho de massa muscular" :
                        i == client2Records.Count - 1 ? "Excelente ganho de massa magra. Continue o treino de força." :
                        $"Progresso consistente no ganho de massa muscular.";
                }

                // Client 3: Health improvement (cholesterol focus)
                var client3Records = progressRecords.Where(pr => pr.ClientId == clients[2].ClientId).OrderBy(pr => pr.RecordDate).ToList();
                for (int i = 0; i < client3Records.Count; i++)
                {
                    var progress = (decimal)i / client3Records.Count;
                    client3Records[i].Cholesterol = Math.Round(230.0m - (progress * 45.0m), 0);
                    client3Records[i].Weight = Math.Round(78.0m - (progress * 5.0m), 1);
                    client3Records[i].BodyFatPercentage = Math.Round(26.0m - (progress * 4.0m), 1);
                    client3Records[i].Notes = i == 0 ? "Avaliação inicial - Objetivo: reduzir colesterol e melhorar saúde cardiovascular" :
                        i == client3Records.Count - 1 ? "Colesterol normalizado! Manter dieta equilibrada." :
                        $"Melhoria significativa nos indicadores de saúde.";
                }
            }

            // Save to database
            dbContext.ProgressRecord.AddRange(progressRecords);
            dbContext.SaveChanges();
        }

        private static string GenerateNotes(int recordIndex, int totalRecords)
        {
            if (recordIndex == 0)
            {
                return "Avaliação inicial. Cliente apresenta boa disposição para mudança de hábitos.";
            }
            else if (recordIndex == totalRecords - 1)
            {
                return "Avaliação final do período. Cliente demonstra compromisso com o plano alimentar.";
            }
            else
            {
                var notes = new[]
                {
                    "Cliente segue o plano alimentar conforme orientado.",
                    "Observa-se evolução positiva nos indicadores.",
                    "Cliente relata melhor disposição e qualidade de sono.",
                    "Continuar com o plano atual. Resultados satisfatórios.",
                    "Cliente demonstra bom comprometimento com as orientações nutricionais.",
                    "Ajustes menores no plano para otimizar resultados."
                };
                
                return notes[recordIndex % notes.Length];
            }
        }
    }
}
