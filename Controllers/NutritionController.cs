// Controllers/NutritionController.cs
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace HealthWellbeing.Controllers
{
    public class NutritionController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public NutritionController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Nutrition/ListAll
        public IActionResult ListAll()
        {
            var clients = _context.Client.ToList();
            return View(clients);
        }

        // GET: Nutrition/Calculate/ClientId-String (optional)
        [Authorize]
        public async Task<IActionResult> Calculate(string? id)
        {
            Client? client = null;

            // If no ID is provided, get the current user's client record
            if (string.IsNullOrWhiteSpace(id))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return Forbid();
                }
                client = await _context.Client.FirstOrDefaultAsync(c => c.IdentityUserId == userId);
            }
            else
            {
                // Try to find by ClientId first
                client = await _context.Client.FindAsync(id);
                
                // If not found, try to find by IdentityUserId
                if (client == null)
                {
                    client = await _context.Client.FirstOrDefaultAsync(c => c.IdentityUserId == id);
                }
            }

            if (client == null)
            {
                return NotFound();
            }

            var needs = CalculateClientNeeds(client);
            return View(needs);
        }

        // GET: Nutrition/CompareWithGoal/ClientId-String (optional)
        [Authorize]
        public async Task<IActionResult> CompareWithGoal(string? id)
        {
            Client? client = null;

            // If no ID is provided, get the current user's client record
            if (string.IsNullOrWhiteSpace(id))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return Forbid();
                }
                client = await _context.Client.FirstOrDefaultAsync(c => c.IdentityUserId == userId);
            }
            else
            {
                // Try to find by ClientId first
                client = await _context.Client.FindAsync(id);
                
                // If not found, try to find by IdentityUserId
                if (client == null)
                {
                    client = await _context.Client.FirstOrDefaultAsync(c => c.IdentityUserId == id);
                }
            }

            if (client == null)
            {
                return NotFound();
            }

            // Buscar meta do cliente
            var meta = _context.Meta
                .FirstOrDefault(m => m.ClientId == client.ClientId);

            if (meta == null)
            {
                TempData["Error"] = "Este cliente não tem meta definida.";
                return RedirectToAction(nameof(ListAll));
            }

            // Calcular necessidades
            var calculatedNeeds = CalculateClientNeeds(client);

            // Criar modelo de comparação
            var viewModel = new CompareWithGoalViewModel
            {
                Client = client,
                Meta = meta,
                CalculatedNeeds = calculatedNeeds,

                // Calcular diferenças
                CaloriesDifference = meta.DailyCalories - (int)calculatedNeeds.DailyCalories,
                ProteinDifference = meta.DailyProtein - (int)calculatedNeeds.ProteinGrams,
                CarbsDifference = meta.DailyHydrates - (int)calculatedNeeds.CarbohydratesGrams,
                FatsDifference = meta.DailyFat - (int)calculatedNeeds.FatsGrams,

                // Calcular percentagens de compliance
                CaloriesCompliance = calculatedNeeds.DailyCalories > 0
                    ? Math.Round((meta.DailyCalories / calculatedNeeds.DailyCalories) * 100, 1)
                    : 0,
                ProteinCompliance = calculatedNeeds.ProteinGrams > 0
                    ? Math.Round((meta.DailyProtein / calculatedNeeds.ProteinGrams) * 100, 1)
                    : 0,
                CarbsCompliance = calculatedNeeds.CarbohydratesGrams > 0
                    ? Math.Round((meta.DailyHydrates / calculatedNeeds.CarbohydratesGrams) * 100, 1)
                    : 0,
                FatsCompliance = calculatedNeeds.FatsGrams > 0
                    ? Math.Round((meta.DailyFat / calculatedNeeds.FatsGrams) * 100, 1)
                    : 0
            };

            return View(viewModel);
        }

        // ========== MÉTODOS PRIVADOS ==========

        private NutritionalNeeds CalculateClientNeeds(Client client)
        {
            // Calcular idade
            int age = CalculateAge(client.BirthDate);

            // Valores padrão (você pode buscar de outro lugar se tiver)
            decimal weight = 70; // kg padrão
            decimal height = 170; // cm padrão
            decimal activityFactor = 1.5m; // moderado padrão

            // Calcular TMB e TDEE
            decimal tmb = CalculateTMB(weight, height, age, client.Gender);
            decimal tdee = tmb * activityFactor;

            // Ajustar para objetivo (assumindo manutenção se não tiver meta)
            decimal targetCalories = tdee;

            // Calcular macronutrientes (distribuição padrão)
            var macros = CalculateMacronutrients(targetCalories, "Maintenance", (double)weight);

            return new NutritionalNeeds
            {
                ClientId = client.ClientId,
                ClientName = client.Name,
                DailyCalories = Math.Round(targetCalories, 0),
                TMB = Math.Round(tmb, 0),
                TDEE = Math.Round(tdee, 0),
                ProteinGrams = Math.Round(macros.Protein, 1),
                CarbohydratesGrams = Math.Round(macros.Carbohydrates, 1),
                FatsGrams = Math.Round(macros.Fats, 1),
                ProteinPercentage = Math.Round(macros.ProteinPercent, 1),
                CarbsPercentage = Math.Round(macros.CarbsPercent, 1),
                FatsPercentage = Math.Round(macros.FatsPercent, 1),
                FiberGrams = CalculateFiber(client.Gender),
                SodiumMg = 2000,
                CalciumMg = age > 50 ? 1200 : 1000,
                IronMg = CalculateIron(client.Gender, age),
                CalculatedDate = DateTime.Now
            };
        }

        private int CalculateAge(DateTime? birthDate)
        {
            if (!birthDate.HasValue) return 30;

            var today = DateTime.Today;
            var age = today.Year - birthDate.Value.Year;
            if (birthDate.Value.Date > today.AddYears(-age)) age--;
            return age;
        }

        private decimal CalculateTMB(decimal weight, decimal height, int age, string gender)
        {
            bool isMale = gender?.ToUpper() == "MALE" ||
                         gender?.ToUpper() == "M" ||
                         gender?.ToUpper() == "MASCULINO";

            if (isMale)
            {
                return 88.362m + (13.397m * weight) + (4.799m * height) - (5.677m * age);
            }
            else
            {
                return 447.593m + (9.247m * weight) + (3.098m * height) - (4.330m * age);
            }
        }

        private (decimal Protein, decimal Carbohydrates, decimal Fats,
                 decimal ProteinPercent, decimal CarbsPercent, decimal FatsPercent)
            CalculateMacronutrients(decimal calories, string goal, double weightKg)
        {
            decimal proteinPercent = 30m;
            decimal carbsPercent = 40m;
            decimal fatsPercent = 30m;

            decimal proteinGrams = (calories * (proteinPercent / 100)) / 4;
            decimal carbsGrams = (calories * (carbsPercent / 100)) / 4;
            decimal fatsGrams = (calories * (fatsPercent / 100)) / 9;

            return (proteinGrams, carbsGrams, fatsGrams,
                    proteinPercent, carbsPercent, fatsPercent);
        }

        private decimal CalculateFiber(string gender)
        {
            bool isMale = gender?.ToUpper() == "MALE" ||
                         gender?.ToUpper() == "M" ||
                         gender?.ToUpper() == "MASCULINO";
            return isMale ? 38 : 25;
        }

        private decimal CalculateIron(string gender, int age)
        {
            bool isMale = gender?.ToUpper() == "MALE" ||
                         gender?.ToUpper() == "M" ||
                         gender?.ToUpper() == "MASCULINO";
            return isMale ? 8 : (age > 50 ? 8 : 18);
        }
    }
}