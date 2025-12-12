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

        // GET: AvaliacaoFisica
        public async Task<IActionResult> Index()
        {
            var query = _context.AvaliacaoFisica.Include(a => a.UtenteGrupo7).AsQueryable();

            if (User.IsInRole("Utente"))
            {
                var userId = _userManager.GetUserId(User);
                query = query.Where(a => a.UtenteGrupo7.UserId == userId);
            }
            // Se for Administrador ou ProfissionalSaude, vê tudo.

            return View(await query.ToListAsync());
        }

        // GET: AvaliacaoFisica/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var avaliacaoFisica = await _context.AvaliacaoFisica
                .Include(a => a.UtenteGrupo7)
                .FirstOrDefaultAsync(m => m.AvaliacaoFisicaId == id);

            if (avaliacaoFisica == null) return NotFound();

            // Segurança: Utente só vê os seus detalhes
            if (User.IsInRole("Utente"))
            {
                var userId = _userManager.GetUserId(User);
                if (avaliacaoFisica.UtenteGrupo7.UserId != userId)
                {
                    return Forbid();
                }
            }

            return View(avaliacaoFisica);
        }

        // GET: AvaliacaoFisica/Create
        public IActionResult Create()
        {
            
            bool isStaff = User.IsInRole("Administrador") || User.IsInRole("ProfissionalSaude");
            ViewBag.IsStaff = isStaff;

            if (isStaff)
            {
                
                ViewData["UtenteGrupo7Id"] = new SelectList(_context.UtenteGrupo7, "UtenteGrupo7Id", "Nome");
            }

            

            return View();
        }

        // POST: AvaliacaoFisica/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AvaliacaoFisicaId,DataMedicao,Peso,Altura,GorduraCorporal,MassaMuscular,Pescoco,Ombros,Peitoral,BracoDireito,BracoEsquerdo,Cintura,Abdomen,Anca,CoxaDireita,CoxaEsquerda,GemeoDireito,GemeoEsquerdo,UtenteGrupo7Id")] AvaliacaoFisica avaliacaoFisica)
        {
            bool isStaff = User.IsInRole("Administrador") || User.IsInRole("ProfissionalSaude");

            if (!isStaff)
            {
                // LÓGICA PARA UTENTE
                var userId = _userManager.GetUserId(User);
                var utenteLogado = await _context.UtenteGrupo7.FirstOrDefaultAsync(u => u.UserId == userId);

                if (utenteLogado == null)
                {
                    ModelState.AddModelError("", "Você precisa criar o seu Perfil de Utente antes de registar avaliações.");
                    return View(avaliacaoFisica);
                }

                // Força o ID do utente logado
                avaliacaoFisica.UtenteGrupo7Id = utenteLogado.UtenteGrupo7Id;

                // Remove erros de validação pois o campo não existe no HTML do utente
                ModelState.Remove("UtenteGrupo7Id");
                ModelState.Remove("UtenteGrupo7");
            }

            if (ModelState.IsValid)
            {
                _context.Add(avaliacaoFisica);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Recarregar em caso de erro
            ViewBag.IsStaff = isStaff;
            if (isStaff)
            {
                ViewData["UtenteGrupo7Id"] = new SelectList(_context.UtenteGrupo7, "UtenteGrupo7Id", "Nome", avaliacaoFisica.UtenteGrupo7Id);
            }

            return View(avaliacaoFisica);
        }

        // GET: AvaliacaoFisica/Edit/5
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
                if (avaliacaoFisica.UtenteGrupo7.UserId != userId)
                {
                    return Forbid();
                }
            }

            ViewBag.IsStaff = isStaff;
            if (isStaff)
            {
                ViewData["UtenteGrupo7Id"] = new SelectList(_context.UtenteGrupo7, "UtenteGrupo7Id", "Nome", avaliacaoFisica.UtenteGrupo7Id);
            }

            return View(avaliacaoFisica);
        }

        // POST: AvaliacaoFisica/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AvaliacaoFisicaId,DataMedicao,Peso,Altura,GorduraCorporal,MassaMuscular,Pescoco,Ombros,Peitoral,BracoDireito,BracoEsquerdo,Cintura,Abdomen,Anca,CoxaDireita,CoxaEsquerda,GemeoDireito,GemeoEsquerdo,UtenteGrupo7Id")] AvaliacaoFisica avaliacaoFisica)
        {
            if (id != avaliacaoFisica.AvaliacaoFisicaId) return NotFound();

            bool isStaff = User.IsInRole("Administrador") || User.IsInRole("ProfissionalSaude");

            if (!isStaff)
            {
                // Garante que o Utente não altera a propriedade do registo
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
                    if (!AvaliacaoFisicaExists(avaliacaoFisica.AvaliacaoFisicaId)) return NotFound();
                    else throw;
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

        // GET: AvaliacaoFisica/Delete/5
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

        // POST: AvaliacaoFisica/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var avaliacaoFisica = await _context.AvaliacaoFisica.FindAsync(id);
            if (avaliacaoFisica != null)
            {
                _context.AvaliacaoFisica.Remove(avaliacaoFisica);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AvaliacaoFisicaExists(int id)
        {
            return _context.AvaliacaoFisica.Any(e => e.AvaliacaoFisicaId == id);
        }
    }
}