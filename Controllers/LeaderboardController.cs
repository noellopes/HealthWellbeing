using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace HealthWellbeing.Controllers
{
    [Authorize(Roles = "Utente")]
    public class LeaderboardController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public LeaderboardController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var userEmail = User.Identity?.Name;
            int pageSize = 10; // 10 utilizadores por página na tabela

            // 1. Query Base (Ordenada por Pontos)
            var query = _context.Customer
                .Include(c => c.Level)
                .Include(c => c.CustomerBadges)
                .OrderByDescending(c => c.TotalPoints);

            // 2. Obter Top 3 Global para o Pódio (Independente da paginação)
            var top3Entities = await query.Take(3).AsNoTracking().ToListAsync();
            var top3ViewModel = top3Entities.Select((c, index) => new LeaderboardItemViewModel
            {
                Rank = index + 1,
                CustomerName = c.Name,
                LevelName = c.Level?.Description ?? "Beginner",
                LevelNumber = c.Level?.LevelNumber ?? 0,
                TotalPoints = c.TotalPoints,
                BadgesCount = c.CustomerBadges.Count,
                IsCurrentUser = c.Email == userEmail
            }).ToList();

            ViewBag.Top3 = top3ViewModel;

            // 3. Paginação da Lista Completa
            int totalItems = await query.CountAsync();
            var paginationInfo = new PaginationInfo<LeaderboardItemViewModel>(page, totalItems, pageSize);

            // Obter os itens da página atual
            var pagedEntities = await query
                .Skip(paginationInfo.ItemsToSkip)
                .Take(paginationInfo.ItemsPerPage)
                .AsNoTracking()
                .ToListAsync();

            // 4. Mapear para ViewModel e Calcular Rank correto
            // O Rank é: (PáginaAnterior * TamanhoPagina) + ÍndiceNaLista + 1
            int startRank = (page - 1) * pageSize + 1;

            var pagedViewModel = pagedEntities.Select((c, index) => new LeaderboardItemViewModel
            {
                Rank = startRank + index,
                CustomerName = c.Name,
                LevelName = c.Level?.Description ?? "Beginner",
                LevelNumber = c.Level?.LevelNumber ?? 0,
                TotalPoints = c.TotalPoints,
                BadgesCount = c.CustomerBadges.Count,
                IsCurrentUser = c.Email == userEmail
            }).ToList();

            paginationInfo.Items = pagedViewModel;

            return View(paginationInfo);
        }
    }
}