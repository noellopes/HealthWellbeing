// Controllers/NutritionalCalculationController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using HealthWellbeing.Data;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class NutritionalCalculationController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public NutritionalCalculationController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: /NutritionalCalculation/Calculate/5
        public IActionResult Calculate(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = _context.Client
                .Include(c => c.Goals)
                .FirstOrDefault(c => c.ClientId == id);

            if (client == null)
            {
                return NotFound();
            }

            var goal = client.Goals?.FirstOrDefault();
            var calculation = NutritionalCalculation.CalculateForClient(client, goal);

            return View(calculation);
        }

        // GET: /NutritionalCalculation/CompareWithGoal/5
        public IActionResult CompareWithGoal(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = _context.Client
                .Include(c => c.Goals)
                .FirstOrDefault(c => c.ClientId == id);

            if (client == null)
            {
                return NotFound();
            }

            var goal = client.Goals?.FirstOrDefault();
            var calculation = NutritionalCalculation.CalculateForClient(client, goal);

            var viewModel = new
            {
                Client = client,
                Calculated = calculation,
                ExistingGoal = goal,
                HasGoal = goal != null
            };

            return View(viewModel);
        }

        // POST: /NutritionalCalculation/UpdateGoal/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateGoal(int id)
        {
            var client = _context.Client
                .Include(c => c.Goals)
                .FirstOrDefault(c => c.ClientId == id);

            if (client == null)
            {
                return NotFound();
            }

            var calculation = NutritionalCalculation.CalculateForClient(client, null);

            var existingGoal = client.Goals?.FirstOrDefault();

            if (existingGoal == null)
            {
                existingGoal = new Goal
                {
                    ClientId = id,
                    GoalName = calculation.GoalType,
                    DailyCalories = (int)calculation.DailyCalories,
                    DailyProtein = (int)calculation.DailyProtein,
                    DailyFat = (int)calculation.DailyFat,
                    DailyHydrates = (int)calculation.DailyCarbs,
                    DailyVitamins = 0
                };
                _context.Goal.Add(existingGoal);
            }
            else
            {
                existingGoal.GoalName = calculation.GoalType;
                existingGoal.DailyCalories = (int)calculation.DailyCalories;
                existingGoal.DailyProtein = (int)calculation.DailyProtein;
                existingGoal.DailyFat = (int)calculation.DailyFat;
                existingGoal.DailyHydrates = (int)calculation.DailyCarbs;
            }

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Meta nutricional atualizada com sucesso!";
            return RedirectToAction("Calculate", new { id });
        }

        // GET: /NutritionalCalculation/ListAll
        public IActionResult ListAll()
        {
            var clients = _context.Client
                .Include(c => c.Goals)
                .OrderBy(c => c.Name)
                .ToList();

            var viewModels = clients
                .Select(c => new ClientCalculationViewModel
                {
                    Client = c,
                    Calculation = NutritionalCalculation.CalculateForClient(c, c.Goals?.FirstOrDefault())
                })
                .ToList();

            return View(viewModels);
        }
    }
}