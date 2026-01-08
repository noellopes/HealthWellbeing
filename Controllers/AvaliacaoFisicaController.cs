using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using HealthWellbeing.ViewModels;

namespace HealthWellbeing.Controllers
{
    [Authorize]
    public class AvaliacaoFisicaController : Controller
    {
        private readonly HealthWellbeingDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AvaliacaoFisicaController(HealthWellbeingDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index(
            int page = 1,
            string searchUtente = "",
            string dataInicio = "",
            string dataFim = "")
        {
            var query = _context.AvaliacaoFisica
                .Include(a => a.UtenteGrupo7)
                .AsQueryable();


            if (User.IsInRole("Utente"))
            {
                var userId = _userManager.GetUserId(User);
                query = query.Where(a => a.UtenteGrupo7.UserId == userId);
            }
            else
            {

                if (!string.IsNullOrEmpty(searchUtente))
                {
                    query = query.Where(a => a.UtenteGrupo7.Nome.Contains(searchUtente));
                }
            }


            if (DateTime.TryParse(dataInicio, out DateTime dtInicio))
            {
                query = query.Where(a => a.DataMedicao >= dtInicio);
            }

            if (DateTime.TryParse(dataFim, out DateTime dtFim))
            {
                query = query.Where(a => a.DataMedicao <= dtFim.AddDays(1).AddTicks(-1));
            }


            ViewBag.SearchUtente = searchUtente;
            ViewBag.DataInicio = dataInicio;
            ViewBag.DataFim = dataFim;


            int total = await query.CountAsync();
            var pagination = new PaginationInfo<AvaliacaoFisica>(page, total);

            if (total > 0)
            {
                pagination.Items = await query
                    .OrderByDescending(a => a.DataMedicao)
                    .Skip(pagination.ItemsToSkip)
                    .Take(pagination.ItemsPerPage)
                    .ToListAsync();
            }
            else
            {
                pagination.Items = new List<AvaliacaoFisica>();
            }

            return View(pagination);
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var avaliacaoFisica = await _context.AvaliacaoFisica
                .Include(a => a.UtenteGrupo7)
                .FirstOrDefaultAsync(m => m.AvaliacaoFisicaId == id);

            if (avaliacaoFisica == null) return NotFound();


            if (User.IsInRole("Utente"))
            {
                var userId = _userManager.GetUserId(User);
                if (avaliacaoFisica.UtenteGrupo7.UserId != userId) return Forbid();
            }

            return View(avaliacaoFisica);
        }


        public async Task<IActionResult> Create()
        {
            bool isStaff = User.IsInRole("Administrador") || User.IsInRole("ProfissionalSaude");
            ViewBag.IsStaff = isStaff;

            if (isStaff)
            {
                ViewData["UtenteGrupo7Id"] = new SelectList(_context.UtenteGrupo7, "UtenteGrupo7Id", "Nome");
            }
            else
            {
                var userId = _userManager.GetUserId(User);
                var utente = await _context.UtenteGrupo7.FirstOrDefaultAsync(u => u.UserId == userId);

                if (utente == null)
                {
                    TempData["StatusMessage"] = "Aviso: Tem de terminar a configuração do seu perfil.";
                    return RedirectToAction("Create", "UtenteGrupo7");
                }
            }

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AvaliacaoFisicaId,DataMedicao,Peso,Altura,GorduraCorporal,MassaMuscular,Pescoco,Ombros,Peitoral,BracoDireito,BracoEsquerdo,Cintura,Abdomen,Anca,CoxaDireita,CoxaEsquerda,GemeoDireito,GemeoEsquerdo,UtenteGrupo7Id")] AvaliacaoFisica avaliacaoFisica)
        {
            bool isStaff = User.IsInRole("Administrador") || User.IsInRole("ProfissionalSaude");

            if (!isStaff)
            {

                var userId = _userManager.GetUserId(User);
                var utenteLogado = await _context.UtenteGrupo7.FirstOrDefaultAsync(u => u.UserId == userId);

                if (utenteLogado == null)
                {
                    TempData["StatusMessage"] = "Erro: Perfil de Utente não encontrado.";
                    return RedirectToAction("Create", "UtenteGrupo7");
                }

                avaliacaoFisica.UtenteGrupo7Id = utenteLogado.UtenteGrupo7Id;
                ModelState.Remove("UtenteGrupo7Id");
                ModelState.Remove("UtenteGrupo7");
            }

            if (ModelState.IsValid)
            {
                _context.Add(avaliacaoFisica);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }


            ViewBag.IsStaff = isStaff;
            if (isStaff)
            {
                ViewData["UtenteGrupo7Id"] = new SelectList(_context.UtenteGrupo7, "UtenteGrupo7Id", "Nome", avaliacaoFisica.UtenteGrupo7Id);
            }

            return View(avaliacaoFisica);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var avaliacaoFisica = await _context.AvaliacaoFisica
                .Include(a => a.UtenteGrupo7)
                .FirstOrDefaultAsync(m => m.AvaliacaoFisicaId == id);

            if (avaliacaoFisica == null) return NotFound();

            bool isStaff = User.IsInRole("Administrador") || User.IsInRole("ProfissionalSaude");

            if (!isStaff)
            {
                var userId = _userManager.GetUserId(User);
                if (avaliacaoFisica.UtenteGrupo7.UserId != userId) return Forbid();
            }

            ViewBag.IsStaff = isStaff;
            if (isStaff)
            {
                ViewData["UtenteGrupo7Id"] = new SelectList(_context.UtenteGrupo7, "UtenteGrupo7Id", "Nome", avaliacaoFisica.UtenteGrupo7Id);
            }

            return View(avaliacaoFisica);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AvaliacaoFisicaId,DataMedicao,Peso,Altura,GorduraCorporal,MassaMuscular,Pescoco,Ombros,Peitoral,BracoDireito,BracoEsquerdo,Cintura,Abdomen,Anca,CoxaDireita,CoxaEsquerda,GemeoDireito,GemeoEsquerdo,UtenteGrupo7Id")] AvaliacaoFisica avaliacaoFisica)
        {
            if (id != avaliacaoFisica.AvaliacaoFisicaId) return NotFound();

            bool isStaff = User.IsInRole("Administrador") || User.IsInRole("ProfissionalSaude");

            if (!isStaff)
            {

                var original = await _context.AvaliacaoFisica.AsNoTracking().FirstOrDefaultAsync(a => a.AvaliacaoFisicaId == id);
                if (original != null)
                {
                    avaliacaoFisica.UtenteGrupo7Id = original.UtenteGrupo7Id;
                    ModelState.Remove("UtenteGrupo7Id");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(avaliacaoFisica);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AvaliacaoFisicaExists(avaliacaoFisica.AvaliacaoFisicaId))
                    {

                        return View("InvalidAvaliacaoFisica", avaliacaoFisica);
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.IsStaff = isStaff;
            if (isStaff)
            {
                ViewData["UtenteGrupo7Id"] = new SelectList(_context.UtenteGrupo7, "UtenteGrupo7Id", "Nome", avaliacaoFisica.UtenteGrupo7Id);
            }
            return View(avaliacaoFisica);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var avaliacaoFisica = await _context.AvaliacaoFisica
                .Include(a => a.UtenteGrupo7)
                .FirstOrDefaultAsync(m => m.AvaliacaoFisicaId == id);

            if (avaliacaoFisica == null) return NotFound();

            if (User.IsInRole("Utente"))
            {
                var userId = _userManager.GetUserId(User);
                if (avaliacaoFisica.UtenteGrupo7.UserId != userId) return Forbid();
            }

            return View(avaliacaoFisica);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var avaliacaoFisica = await _context.AvaliacaoFisica.FindAsync(id);
            if (avaliacaoFisica != null)
            {
                _context.AvaliacaoFisica.Remove(avaliacaoFisica);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool AvaliacaoFisicaExists(int id)
        {
            return _context.AvaliacaoFisica.Any(e => e.AvaliacaoFisicaId == id);
        }
    }
}