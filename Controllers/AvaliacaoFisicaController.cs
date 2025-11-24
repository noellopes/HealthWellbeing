using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class AvaliacaoFisicaController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public AvaliacaoFisicaController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: AvaliacaoFisica
        public async Task<IActionResult> Index()
        {
            var healthWellbeingDbContext = _context.AvaliacaoFisica.Include(a => a.UtenteGrupo7);
            return View(await healthWellbeingDbContext.ToListAsync());
        }

        // GET: AvaliacaoFisica/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var avaliacaoFisica = await _context.AvaliacaoFisica
                .Include(a => a.UtenteGrupo7)
                .FirstOrDefaultAsync(m => m.AvaliacaoFisicaId == id);
            if (avaliacaoFisica == null)
            {
                return NotFound();
            }

            return View(avaliacaoFisica);
        }

        // GET: AvaliacaoFisica/Create
        public IActionResult Create()
        {
            ViewData["UtenteGrupo7Id"] = new SelectList(_context.UtenteGrupo7, "UtenteGrupo7Id", "UtenteGrupo7Id");
            return View();
        }

        // POST: AvaliacaoFisica/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AvaliacaoFisicaId,DataMedicao,Peso,Altura,GorduraCorporal,MassaMuscular,Pescoco,Ombros,Peitoral,BracoDireito,BracoEsquerdo,Cintura,Abdomen,Anca,CoxaDireita,CoxaEsquerda,GemeoDireito,GemeoEsquerdo,UtenteGrupo7Id")] AvaliacaoFisica avaliacaoFisica)
        {
            if (ModelState.IsValid)
            {
                _context.Add(avaliacaoFisica);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UtenteGrupo7Id"] = new SelectList(_context.UtenteGrupo7, "UtenteGrupo7Id", "UtenteGrupo7Id", avaliacaoFisica.UtenteGrupo7Id);
            return View(avaliacaoFisica);
        }

        // GET: AvaliacaoFisica/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var avaliacaoFisica = await _context.AvaliacaoFisica.FindAsync(id);
            if (avaliacaoFisica == null)
            {
                return NotFound();
            }
            ViewData["UtenteGrupo7Id"] = new SelectList(_context.UtenteGrupo7, "UtenteGrupo7Id", "UtenteGrupo7Id", avaliacaoFisica.UtenteGrupo7Id);
            return View(avaliacaoFisica);
        }

        // POST: AvaliacaoFisica/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AvaliacaoFisicaId,DataMedicao,Peso,Altura,GorduraCorporal,MassaMuscular,Pescoco,Ombros,Peitoral,BracoDireito,BracoEsquerdo,Cintura,Abdomen,Anca,CoxaDireita,CoxaEsquerda,GemeoDireito,GemeoEsquerdo,UtenteGrupo7Id")] AvaliacaoFisica avaliacaoFisica)
        {
            if (id != avaliacaoFisica.AvaliacaoFisicaId)
            {
                return NotFound();
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
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UtenteGrupo7Id"] = new SelectList(_context.UtenteGrupo7, "UtenteGrupo7Id", "UtenteGrupo7Id", avaliacaoFisica.UtenteGrupo7Id);
            return View(avaliacaoFisica);
        }

        // GET: AvaliacaoFisica/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var avaliacaoFisica = await _context.AvaliacaoFisica
                .Include(a => a.UtenteGrupo7)
                .FirstOrDefaultAsync(m => m.AvaliacaoFisicaId == id);
            if (avaliacaoFisica == null)
            {
                return NotFound();
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
