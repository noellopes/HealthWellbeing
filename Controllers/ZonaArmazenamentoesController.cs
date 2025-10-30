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
    public class ZonaArmazenamentoesController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ZonaArmazenamentoesController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: ZonaArmazenamentoes
        public async Task<IActionResult> Index()
        {
            return View(await _context.ZonaArmazenamento.ToListAsync());
        }

        // GET: ZonaArmazenamentoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zonaArmazenamento = await _context.ZonaArmazenamento
                .FirstOrDefaultAsync(m => m.Id == id);
            if (zonaArmazenamento == null)
            {
                return NotFound();
            }

            return View(zonaArmazenamento);
        }

        // GET: ZonaArmazenamentoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ZonaArmazenamentoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Descricao,Localizacao,CapacidadeMaxima,Ativa")] ZonaArmazenamento zonaArmazenamento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(zonaArmazenamento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(zonaArmazenamento);
        }

        // GET: ZonaArmazenamentoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zonaArmazenamento = await _context.ZonaArmazenamento.FindAsync(id);
            if (zonaArmazenamento == null)
            {
                return NotFound();
            }
            return View(zonaArmazenamento);
        }

        // POST: ZonaArmazenamentoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Descricao,Localizacao,CapacidadeMaxima,Ativa")] ZonaArmazenamento zonaArmazenamento)
        {
            if (id != zonaArmazenamento.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(zonaArmazenamento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ZonaArmazenamentoExists(zonaArmazenamento.Id))
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
            return View(zonaArmazenamento);
        }

        // GET: ZonaArmazenamentoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zonaArmazenamento = await _context.ZonaArmazenamento
                .FirstOrDefaultAsync(m => m.Id == id);
            if (zonaArmazenamento == null)
            {
                return NotFound();
            }

            return View(zonaArmazenamento);
        }

        // POST: ZonaArmazenamentoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var zonaArmazenamento = await _context.ZonaArmazenamento.FindAsync(id);
            if (zonaArmazenamento != null)
            {
                _context.ZonaArmazenamento.Remove(zonaArmazenamento);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ZonaArmazenamentoExists(int id)
        {
            return _context.ZonaArmazenamento.Any(e => e.Id == id);
        }
    }
}
