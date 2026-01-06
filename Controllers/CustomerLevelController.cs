using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels; // Necessário para PaginationInfo
using Microsoft.AspNetCore.Authorization;

namespace HealthWellbeing.Controllers
{
    [Authorize(Roles = "Utente")]
    public class CustomerLevelController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public CustomerLevelController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Index
        public async Task<IActionResult> Index(int page = 1)
        {
            var userEmail = User.Identity?.Name;

            // 1. Carregar Cliente (Lógica do Dashboard - Mantém-se igual)
            var customer = await _context.Customer
                .Include(c => c.Level)
                .ThenInclude(l => l.Category)
                .FirstOrDefaultAsync(c => c.Email == userEmail);

            if (customer == null) return View("Error");

            // --- Lógica do Dashboard (Progresso) ---
            var currentLevel = customer.Level;
            int currentLevelNum = currentLevel?.LevelNumber ?? 0;
            int currentPoints = customer.TotalPoints;

            var nextLevel = await _context.Level
                .Where(l => l.LevelNumber > currentLevelNum)
                .OrderBy(l => l.LevelNumber)
                .FirstOrDefaultAsync();

            ViewBag.CurrentPoints = currentPoints;
            ViewBag.CurrentLevelName = currentLevel?.Description ?? "No Level";
            ViewBag.CurrentLevelNumber = currentLevelNum;
            ViewBag.CategoryName = currentLevel?.Category?.Name ?? "General";

            if (nextLevel != null)
            {
                int targetPoints = nextLevel.LevelPointsLimit;
                int pointsNeeded = targetPoints - currentPoints;
                double progressPercentage = 0;
                if (targetPoints > 0) progressPercentage = ((double)currentPoints / targetPoints) * 100;
                if (progressPercentage > 100) progressPercentage = 100;

                ViewBag.NextLevelNumber = nextLevel.LevelNumber;
                ViewBag.PointsNeeded = pointsNeeded > 0 ? pointsNeeded : 0;
                ViewBag.TargetPoints = targetPoints;
                ViewBag.ProgressPercentage = (int)progressPercentage;
                ViewBag.IsMaxLevel = false;
            }
            else
            {
                ViewBag.NextLevelNumber = currentLevelNum;
                ViewBag.PointsNeeded = 0;
                ViewBag.TargetPoints = currentPoints;
                ViewBag.ProgressPercentage = 100;
                ViewBag.IsMaxLevel = true;
            }

            // 2. Carregar Lista de Níveis COM PAGINAÇÃO
            var levelsQuery = _context.Level
                .Include(l => l.Category)
                .OrderBy(l => l.LevelNumber);

            int totalItems = await levelsQuery.CountAsync();
            int pageSize = 5; // Mostra 5 níveis por página (ajusta se quiseres)

            var paginationInfo = new ViewModels.PaginationInfo<Level>(page, totalItems, pageSize);

            paginationInfo.Items = await levelsQuery
                .Skip(paginationInfo.ItemsToSkip)
                .Take(paginationInfo.ItemsPerPage)
                .ToListAsync();

            return View(paginationInfo);
        }
    }
}