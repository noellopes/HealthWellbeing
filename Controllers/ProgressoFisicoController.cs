using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;

namespace HealthWellbeing.Controllers
{
    [Authorize(Roles = "Utente")]
    public class ProgressoFisicoController : Controller
    {
        private readonly HealthWellbeingDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ProgressoFisicoController(
            HealthWellbeingDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Peso()
        {
            var userId = _userManager.GetUserId(User);

            var utente = await _context.UtenteGrupo7
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (utente == null)
            {
                return RedirectToAction("Create", "UtenteGrupo7");
            }

            var dados = await _context.AvaliacaoFisica
                .Where(a => a.UtenteGrupo7Id == utente.UtenteGrupo7Id)
                .OrderBy(a => a.DataMedicao)
                .Select(a => new
                {
                    Data = a.DataMedicao.ToString("dd/MM/yyyy"),
                    Peso = a.Peso,
                    MassaMuscular = a.MassaMuscular,
                    Gordura = a.GorduraCorporal
                })
                .ToListAsync();

            ViewBag.Datas = dados.Select(d => d.Data).ToList();
            ViewBag.Pesos = dados.Select(d => d.Peso).ToList();
            ViewBag.MassaMuscular = dados.Select(d => d.MassaMuscular).ToList();
            ViewBag.Gordura = dados.Select(d => d.Gordura).ToList();
            ViewBag.NomeUtente = utente.Nome;

            return View();
        }
    }
}
