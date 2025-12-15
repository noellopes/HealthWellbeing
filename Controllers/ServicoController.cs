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
            var servicosContext = await _context.Servicos
                .Include(s => s.TipoServico)
                .ToListAsync();
            return View(await _context.Servicos.ToListAsync());
        }

        // GET: Servico/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servico = await _context.Servicos
                .Include(s => s.TipoServico)
                .FirstOrDefaultAsync(m => m.ServicoId == id);
            if (servico == null)
            {
                return View("InvalidServico");
            }

            return View(servico);
        }

        // GET: Servico/Create
        public IActionResult Create()
        {
            ViewData["TipoServicoId"] = new SelectList(_context.TipoServicos, "TipoServicoId", "Nome");
            return View();
        }

        // POST: Servico/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ServicoId,Nome,Descricao,Preco,DuracaoMinutos,TipoServicoId")] Servico servico)

        {
            ModelState.Remove("TipoServico");
            if (ModelState.IsValid)
            {
                _context.Add(servico);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index),
                    new
                    {
                        id = servico.ServicoId,
                        SuccessMessage = "Serviço criado com sucesso!"
                    }
                    );
            }

            ViewData["TipoServicoId"] = new SelectList(_context.TipoServicos, "TipoServicoId", "Nome", servico.TipoServicoId);
            return View(servico);
        }

        // GET: Servico/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servico = await _context.Servicos.FindAsync(id);
            if (servico == null)
            {
                return NotFound();
            }
            return View(servico);
        }

        // POST: Servico/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ServicoId,Nome,Descricao,Preco,DuracaoMinutos,TipoServico")] Servico servico)
        {
            if (id != servico.ServicoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(servico);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServicoExists(servico.ServicoId))
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
            return View(servico);
        }

        // GET: Servico/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servico = await _context.Servicos
                .FirstOrDefaultAsync(m => m.ServicoId == id);
            if (servico == null)
            {
                return NotFound();
            }

            return View(servico);
        }

        // POST: Servico/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var servico = await _context.Servicos.FindAsync(id);
            if (servico != null)
            {
                _context.Servicos.Remove(servico);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServicoExists(int id)
        {
            return _context.Servicos.Any(e => e.ServicoId == id);
        }
    }
}
