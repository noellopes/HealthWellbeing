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
    public class TipoServicoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TipoServicoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TipoServico
        public async Task<IActionResult> Index()
        {
            return View(await _context.TipoServicos.ToListAsync());
        }

        // GET: TipoServico/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoServicoModel = await _context.TipoServicos
                .FirstOrDefaultAsync(m => m.TipoServicoId == id);
            if (tipoServicoModel == null)
            {
                return NotFound();
            }

            return View(tipoServicoModel);
        }

        // GET: TipoServico/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoServico/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TipoServicoId,Nome,Descricao")] TipoServicoModel tipoServicoModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipoServicoModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipoServicoModel);
        }

        // GET: TipoServico/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoServicoModel = await _context.TipoServicos.FindAsync(id);
            if (tipoServicoModel == null)
            {
                return NotFound();
            }
            return View(tipoServicoModel);
        }

        // POST: TipoServico/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TipoServicoId,Nome,Descricao")] TipoServicoModel tipoServicoModel)
        {
            if (id != tipoServicoModel.TipoServicoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipoServicoModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoServicoModelExists(tipoServicoModel.TipoServicoId))
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
            return View(tipoServicoModel);
        }

        // GET: TipoServico/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoServicoModel = await _context.TipoServicos
                .FirstOrDefaultAsync(m => m.TipoServicoId == id);
            if (tipoServicoModel == null)
            {
                return NotFound();
            }

            return View(tipoServicoModel);
        }

        // POST: TipoServico/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipoServicoModel = await _context.TipoServicos.FindAsync(id);
            if (tipoServicoModel != null)
            {
                _context.TipoServicos.Remove(tipoServicoModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TipoServicoModelExists(int id)
        {
            return _context.TipoServicos.Any(e => e.TipoServicoId == id);
        }
    }
}
