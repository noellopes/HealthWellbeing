using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace HealthWellbeing.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HealthWellbeingDbContext _context; // Adicionar o Contexto

        // Atualizar o construtor para receber o Contexto
        public HomeController(ILogger<HomeController> logger, HealthWellbeingDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // 1. Buscar os Planos (como já tínhamos)
            var topPlans = await _context.Plan
                .OrderBy(p => p.Price)
                .Take(6)
                .ToListAsync();

            // 2. Lógica NOVA: Descobrir o ID do Membro Logado
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                // Procura na tabela de Membros alguém cujo Cliente tenha este email
                var member = await _context.Member
                    .Include(m => m.Client)
                    .FirstOrDefaultAsync(m => m.Client.Email == User.Identity.Name);

                if (member != null)
                {
                    // Se encontrou, guarda o ID para usar na Vista
                    ViewBag.MemberId = member.MemberId;
                }
            }

            return View(topPlans);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}