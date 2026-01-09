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
    [Authorize(Roles = "Gestor")] // Apenas para Gestores
    public class RankingManagerController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public RankingManagerController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: RankingManager
        public async Task<IActionResult> Index(string searchName, int page = 1)
        {
            int pageSize = 20; // Gestores preferem ver mais linhas por página

            // 1. Query Base
            var query = _context.Customer
                .Include(c => c.Level)
                .Include(c => c.CustomerBadges)
                .AsQueryable();

            // 2. Filtro de Pesquisa
            if (!string.IsNullOrEmpty(searchName))
            {
                query = query.Where(c => c.Name.Contains(searchName) || c.Email.Contains(searchName));
            }

            // 3. Ordenação (Sempre por pontos decrescente para definir o Rank)
            query = query.OrderByDescending(c => c.TotalPoints);

            // 4. Paginação
            int totalItems = await query.CountAsync();
            var paginationInfo = new ViewModels.PaginationInfo<Customer>(page, totalItems, pageSize);

            var customers = await query
                .Skip(paginationInfo.ItemsToSkip)
                .Take(paginationInfo.ItemsPerPage)
                .AsNoTracking()
                .ToListAsync();

            // Guardar os dados na PaginationInfo
            paginationInfo.Items = customers;

            // ViewBags para manter o estado da pesquisa
            ViewBag.SearchName = searchName;

            // Para calcular o Rank correto na tabela (ex: página 2 começa no rank 21)
            ViewBag.StartRank = (page - 1) * pageSize + 1;

            return View(paginationInfo);
        }
    }
}