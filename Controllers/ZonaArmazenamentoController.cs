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
    public class ZonaArmazenamentoController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ZonaArmazenamentoController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: ZonaArmazenamento
        public async Task<IActionResult> Index()
        {
            return View(await _context.ZonaArmazenamento.ToListAsync());
        }

        // GET: ZonaArmazenamento/Details/5
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

        // GET: ZonaArmazenamento/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ZonaArmazenamento/Create
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

        // GET: ZonaArmazenamento/Edit/5
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

        // POST: ZonaArmazenamento/Edit/5
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

        // GET: ZonaArmazenamento/Delete/5
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

        // POST: ZonaArmazenamento/Delete/5
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
