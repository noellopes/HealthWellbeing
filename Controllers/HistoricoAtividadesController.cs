using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HealthWellbeing.Controllers
{
    [Authorize]
    public class HistoricoAtividadesController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public HistoricoAtividadesController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: HistoricoAtividades
        public async Task<IActionResult> Index(
            string exercicio,
            string utente,
            DateTime? dataInicio,
            DateTime? dataFim,
            int page = 1)
        {
            int pageSize = 10;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isStaff =
                User.IsInRole(SeedData.Roles.Administrador) ||
                User.IsInRole(SeedData.Roles.Profissional);

            var query = _context.HistoricoAtividades
                .Include(h => h.Exercicio)
                .Include(h => h.UtenteGrupo7)
                .AsQueryable();

            // 🔐 utente normal só vê os seus
            if (!isStaff)
            {
                query = query.Where(h => h.UtenteGrupo7.UserId == userId);
            }

            // 🔍 FILTROS (IGUAL AO TEU EXEMPLO)
            if (!string.IsNullOrWhiteSpace(exercicio))
                query = query.Where(h => h.Exercicio.ExercicioNome.Contains(exercicio));

            if (isStaff && !string.IsNullOrWhiteSpace(utente))
                query = query.Where(h => h.UtenteGrupo7.Nome.Contains(utente));

            if (dataInicio.HasValue)
                query = query.Where(h => h.DataRealizacao >= dataInicio.Value);

            if (dataFim.HasValue)
                query = query.Where(h => h.DataRealizacao <= dataFim.Value);

            int totalItems = await query.CountAsync();

            var pagination = new PaginationInfo<HistoricoAtividade>(page, totalItems, pageSize);

            pagination.Items = await query
                .OrderByDescending(h => h.DataRealizacao)
                .Skip(pagination.ItemsToSkip)
                .Take(pagination.ItemsPerPage)
                .ToListAsync();

            // manter filtros
            ViewBag.Exercicio = exercicio;
            ViewBag.Utente = utente;
            ViewBag.DataInicio = dataInicio?.ToString("yyyy-MM-dd");
            ViewBag.DataFim = dataFim?.ToString("yyyy-MM-dd");

            return View(pagination);
        }
    }
}

