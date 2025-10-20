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
    public class ServicoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServicoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Servico
        public async Task<IActionResult> Index()
        {
            return View(await _context.Servicos.ToListAsync());
        }

        // GET: Servico/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicoModel = await _context.Servicos
                .FirstOrDefaultAsync(m => m.ServicoId == id);
            if (servicoModel == null)
            {
                return NotFound();
            }

            return View(servicoModel);
        }

        // GET: Servico/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Servico/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ServicoId,Nome,Descricao,Preco,DuracaoMinutos,Tipo")] ServicoModel servicoModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(servicoModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(servicoModel);
        }

        // GET: Servico/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicoModel = await _context.Servicos.FindAsync(id);
            if (servicoModel == null)
            {
                return NotFound();
            }
            return View(servicoModel);
        }

        // POST: Servico/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ServicoId,Nome,Descricao,Preco,DuracaoMinutos,Tipo")] ServicoModel servicoModel)
        {
            if (id != servicoModel.ServicoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(servicoModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServicoModelExists(servicoModel.ServicoId))
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
            return View(servicoModel);
        }

        // GET: Servico/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicoModel = await _context.Servicos
                .FirstOrDefaultAsync(m => m.ServicoId == id);
            if (servicoModel == null)
            {
                return NotFound();
            }

            return View(servicoModel);
        }

        // POST: Servico/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var servicoModel = await _context.Servicos.FindAsync(id);
            if (servicoModel != null)
            {
                _context.Servicos.Remove(servicoModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServicoModelExists(int id)
        {
            return _context.Servicos.Any(e => e.ServicoId == id);
        }
    }
}
