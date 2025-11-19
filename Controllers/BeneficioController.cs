using HealthWellbeing.Models;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellbeing.Controllers
{
    public class BeneficioController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public BeneficioController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Beneficio
        public async Task<IActionResult> Index()
        {
            return View(await _context.Beneficio.ToListAsync());
        }

        // GET: Beneficio/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var beneficio = await _context.Beneficio
                .FirstOrDefaultAsync(m => m.BeneficioId == id);
            if (beneficio == null)
            {
                return NotFound();
            }

            return View(beneficio);
        }

        // GET: Beneficio/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Beneficio/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BeneficioId,NomeBeneficio,DescricaoBeneficio")] Beneficio beneficio)
        {
            if (ModelState.IsValid)
            {
                _context.Add(beneficio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(beneficio);
        }

        // GET: Beneficio/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var beneficio = await _context.Beneficio.FindAsync(id);
            if (beneficio == null)
            {
                return NotFound();
            }
            return View(beneficio);
        }

        // POST: Beneficio/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BeneficioId,NomeBeneficio,DescricaoBeneficio")] Beneficio beneficio)
        {
            if (id != beneficio.BeneficioId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(beneficio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BeneficioExists(beneficio.BeneficioId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(beneficio);
        }

        // GET: Beneficio/Delete/5
        public async Task<IActionResult> Delete(int? id, bool concurrencyError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var beneficio = await _context.Beneficio
                .FirstOrDefaultAsync(m => m.BeneficioId == id);

            if (beneficio == null)
            {
                // Se não for encontrado no GET, e não for por erro de concorrência, é NotFound
                if (!concurrencyError)
                {
                    return NotFound();
                }
                // Se for concurrencyError, o item já foi apagado. Redirecionar para Index
                return RedirectToAction(nameof(Index));
            }

            // --- Verificação de Associação ---
            var hasAssociations = await _context.TipoExercicioBeneficio
                .AnyAsync(tb => tb.BeneficioId == id);

            if (hasAssociations)
            {
                ViewData["ErrorMessage"] = $"Não é possível excluir o benefício '{beneficio.NomeBeneficio}', pois está **associado a um ou mais Tipos de Exercício**.";
                ViewData["DisableDelete"] = true; // Sinal para a View desabilitar o botão
            }
            // ------------------------------------

            if (concurrencyError)
            {
                ViewData["ErrorMessage"] = ViewData["ErrorMessage"] != null
                    ? ViewData["ErrorMessage"]
                    : "O benefício foi modificado por outro utilizador. Por favor, reveja e tente novamente.";

                // Se houver erro de concorrência, o botão deve estar ativo (a não ser que haja associação)
            }

            return View(beneficio);
        }

        // POST: Beneficio/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Busca o benefício e verifica se ele foi apagado por outro
            var beneficio = await _context.Beneficio.FindAsync(id);
            if (beneficio == null)
            {
                // Já foi apagado. Redireciona para o Index.
                return RedirectToAction(nameof(Index));
            }

            // --- Verificação de Associação (antes de tentar apagar) ---
            var hasAssociations = await _context.TipoExercicioBeneficio
                .AnyAsync(tb => tb.BeneficioId == id);

            if (hasAssociations)
            {
                // Redireciona de volta para o GET do Delete para exibir a mensagem.
                return RedirectToAction(nameof(Delete), new { id = id });
            }
            // ------------------------------------

            try
            {
                _context.Beneficio.Remove(beneficio);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Ocorreu um erro de concorrência (alguém editou/apagou antes).
                if (!BeneficioExists(id))
                {
                    // Não existe mais, foi apagado por outro
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Houve alteração, mas o registro ainda existe
                    return RedirectToAction(nameof(Delete), new { id = id, concurrencyError = true });
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool BeneficioExists(int id)
        {
            return _context.Beneficio.Any(e => e.BeneficioId == id);
        }
    }
}