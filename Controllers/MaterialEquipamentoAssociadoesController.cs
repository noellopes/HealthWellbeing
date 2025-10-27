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
    public class MaterialEquipamentoAssociadoesController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public MaterialEquipamentoAssociadoesController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: MaterialEquipamentoAssociadoes
        public async Task<IActionResult> Index()
        {
            return View(await _context.MaterialEquipamentoAssociado.ToListAsync());
        }

        // GET: MaterialEquipamentoAssociadoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var materialEquipamentoAssociado = await _context.MaterialEquipamentoAssociado
                .FirstOrDefaultAsync(m => m.MaterialEquipamentoAssociadoId == id);
            if (materialEquipamentoAssociado == null)
            {
                return NotFound();
            }

            return View(materialEquipamentoAssociado);
        }

        // GET: MaterialEquipamentoAssociadoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MaterialEquipamentoAssociadoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaterialEquipamentoAssociadoId,NomeEquipamento,Quantidade,EstadoComponente")] MaterialEquipamentoAssociado materialEquipamentoAssociado)
        {
            if (ModelState.IsValid)
            {
                _context.Add(materialEquipamentoAssociado);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(materialEquipamentoAssociado);
        }

        // GET: MaterialEquipamentoAssociadoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var materialEquipamentoAssociado = await _context.MaterialEquipamentoAssociado.FindAsync(id);
            if (materialEquipamentoAssociado == null)
            {
                return NotFound();
            }
            return View(materialEquipamentoAssociado);
        }

        // POST: MaterialEquipamentoAssociadoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaterialEquipamentoAssociadoId,NomeEquipamento,Quantidade,EstadoComponente")] MaterialEquipamentoAssociado materialEquipamentoAssociado)
        {
            if (id != materialEquipamentoAssociado.MaterialEquipamentoAssociadoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(materialEquipamentoAssociado);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaterialEquipamentoAssociadoExists(materialEquipamentoAssociado.MaterialEquipamentoAssociadoId))
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
            return View(materialEquipamentoAssociado);
        }

        // GET: MaterialEquipamentoAssociadoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var materialEquipamentoAssociado = await _context.MaterialEquipamentoAssociado
                .FirstOrDefaultAsync(m => m.MaterialEquipamentoAssociadoId == id);
            if (materialEquipamentoAssociado == null)
            {
                return NotFound();
            }

            return View(materialEquipamentoAssociado);
        }

        // POST: MaterialEquipamentoAssociadoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var materialEquipamentoAssociado = await _context.MaterialEquipamentoAssociado.FindAsync(id);
            if (materialEquipamentoAssociado != null)
            {
                _context.MaterialEquipamentoAssociado.Remove(materialEquipamentoAssociado);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MaterialEquipamentoAssociadoExists(int id)
        {
            return _context.MaterialEquipamentoAssociado.Any(e => e.MaterialEquipamentoAssociadoId == id);
        }
    }
}
