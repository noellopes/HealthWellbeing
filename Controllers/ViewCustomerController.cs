using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace HealthWellbeing.Controllers
{
    [Authorize(Roles = "Utente")]
    public class ViewCustomerController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ViewCustomerController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Index (Dashboard)
        public async Task<IActionResult> Index()
        {
            var userEmail = User.Identity?.Name;

            // 1. Carregar Cliente com Nível
            var customer = await _context.Customer
                .Include(c => c.Level).ThenInclude(l => l.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Email == userEmail);

            if (customer == null) return View("Error");

            var viewModel = new CustomerDashboardViewModel
            {
                CustomerName = customer.Name,
                Email = customer.Email,
                TotalPoints = customer.TotalPoints,
                CurrentLevel = customer.Level
            };

            // 2. Lógica de Nível (Cálculo de Progresso)
            var nextLevel = await _context.Level
                .Where(l => l.LevelNumber > (customer.Level != null ? customer.Level.LevelNumber : 0))
                .OrderBy(l => l.LevelNumber)
                .FirstOrDefaultAsync();

            if (nextLevel != null)
            {
                viewModel.NextLevel = nextLevel;
                viewModel.IsMaxLevel = false;

                int target = nextLevel.LevelPointsLimit;
                viewModel.PointsNeeded = target - customer.TotalPoints;
                if (viewModel.PointsNeeded < 0) viewModel.PointsNeeded = 0;

                if (target > 0)
                    viewModel.ProgressPercentage = (int)(((double)customer.TotalPoints / target) * 100);

                if (viewModel.ProgressPercentage > 100) viewModel.ProgressPercentage = 100;
            }
            else
            {
                viewModel.IsMaxLevel = true;
                viewModel.ProgressPercentage = 100;
            }

            // 3. Atividades Recentes (Últimas 5)
            viewModel.RecentActivities = await _context.CustomerActivity
                .Include(ca => ca.Activity).ThenInclude(a => a.ActivityType)
                .Where(ca => ca.CustomerId == customer.CustomerId)
                .OrderByDescending(ca => ca.CompletionDate)
                .Take(5)
                .ToListAsync();

            // 4. Eventos Ativos (Inscrito e não expirado)
            var now = DateTime.Now;
            viewModel.ActiveEnrollments = await _context.CustomerEvent
                .Include(ce => ce.Event).ThenInclude(e => e.EventType)
                .Where(ce => ce.CustomerId == customer.CustomerId
                             && ce.Status == "Enrolled"
                             && ce.Event.EventEnd >= now)
                .OrderBy(ce => ce.Event.EventStart)
                .ToListAsync();

            // 5. Badges (Crachás)
            viewModel.BadgesEarnedCount = await _context.CustomerBadge
                .CountAsync(cb => cb.CustomerId == customer.CustomerId);

            viewModel.BadgesTotalCount = await _context.Badge.CountAsync();

            // Últimos 3 crachás ganhos
            viewModel.RecentBadges = await _context.CustomerBadge
                .Include(cb => cb.Badge).ThenInclude(b => b.BadgeType)
                .Where(cb => cb.CustomerId == customer.CustomerId)
                .OrderByDescending(cb => cb.DateAwarded)
                .Take(3)
                .Select(cb => cb.Badge)
                .ToListAsync();

            return View(viewModel);
        }
    }
}