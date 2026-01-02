using HealthWellbeing.Data;
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
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var query = _context.HistoricoAtividades
                .Include(h => h.Exercicio)
                .Include(h => h.UtenteGrupo7)
                .AsQueryable();

            // 👉 Cliente normal: só vê os seus
            if (!User.IsInRole(SeedData.Roles.Administrador) &&
                !User.IsInRole(SeedData.Roles.Profissional))
            {
                query = query.Where(h => h.UtenteGrupo7.UserId == userId);
            }

            var historico = await query
                .OrderByDescending(h => h.DataRealizacao)
                .ToListAsync();

            return View(historico);
        }
    }
}
