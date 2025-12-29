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
    public class AgendamentoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AgendamentoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Agendamento
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Agendamento.Include(a => a.Servico).Include(a => a.Terapeuta).Include(a => a.UtenteBalneario);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Agendamento/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var agendamentoModel = await _context.Agendamento
                .Include(a => a.Servico)
                .Include(a => a.Terapeuta)
                .Include(a => a.UtenteBalneario)
                .FirstOrDefaultAsync(m => m.AgendamentoId == id);
            if (agendamentoModel == null)
            {
                return NotFound();
            }

            return View(agendamentoModel);
        }

        // GET: Agendamento/Create
        public IActionResult Create()
        {
            ViewData["ServicoId"] = new SelectList(_context.Servicos, "ServicoId", "ServicoId");
            ViewData["TerapeutaId"] = new SelectList(_context.Terapeutas, "TerapeutaId", "TerapeutaId");
            ViewData["UtenteBalnearioId"] = new SelectList(_context.Set<UtenteBalneario>(), "UtenteBalnearioId", "UtenteBalnearioId");
            return View();
        }

        // POST: Agendamento/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AgendamentoId,DataHoraInicio,DataHoraFim,Estado,UtenteBalnearioId,TerapeutaId,ServicoId")] AgendamentoModel agendamentoModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(agendamentoModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ServicoId"] = new SelectList(_context.Servicos, "ServicoId", "ServicoId", agendamentoModel.ServicoId);
            ViewData["TerapeutaId"] = new SelectList(_context.Terapeutas, "TerapeutaId", "TerapeutaId", agendamentoModel.TerapeutaId);
            ViewData["UtenteBalnearioId"] = new SelectList(_context.Set<UtenteBalneario>(), "UtenteBalnearioId", "UtenteBalnearioId", agendamentoModel.UtenteBalnearioId);
            return View(agendamentoModel);
        }

        // GET: Agendamento/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var agendamentoModel = await _context.Agendamentos.FindAsync(id);
            if (agendamentoModel == null)
            {
                return NotFound();
            }
            ViewData["ServicoId"] = new SelectList(_context.Servicos, "ServicoId", "ServicoId", agendamentoModel.ServicoId);
            ViewData["TerapeutaId"] = new SelectList(_context.Terapeutas, "TerapeutaId", "TerapeutaId", agendamentoModel.TerapeutaId);
            ViewData["UtenteBalnearioId"] = new SelectList(_context.Set<UtenteBalneario>(), "UtenteBalnearioId", "UtenteBalnearioId", agendamentoModel.UtenteBalnearioId);
            return View(agendamentoModel);
        }

        // POST: Agendamento/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AgendamentoId,DataHoraInicio,DataHoraFim,Estado,UtenteBalnearioId,TerapeutaId,ServicoId")] AgendamentoModel agendamentoModel)
        {
            if (id != agendamentoModel.AgendamentoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(agendamentoModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AgendamentoModelExists(agendamentoModel.AgendamentoId))
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
            ViewData["ServicoId"] = new SelectList(_context.Servicos, "ServicoId", "ServicoId", agendamentoModel.ServicoId);
            ViewData["TerapeutaId"] = new SelectList(_context.Terapeutas, "TerapeutaId", "TerapeutaId", agendamentoModel.TerapeutaId);
            ViewData["UtenteBalnearioId"] = new SelectList(_context.Set<UtenteBalneario>(), "UtenteBalnearioId", "UtenteBalnearioId", agendamentoModel.UtenteBalnearioId);
            return View(agendamentoModel);
        }

        // GET: Agendamento/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var agendamentoModel = await _context.Agendamentos
                .Include(a => a.Servico)
                .Include(a => a.Terapeuta)
                .Include(a => a.UtenteBalneario)
                .FirstOrDefaultAsync(m => m.AgendamentoId == id);
            if (agendamentoModel == null)
            {
                return NotFound();
            }

            return View(agendamentoModel);
        }

        // POST: Agendamento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var agendamentoModel = await _context.Agendamentos.FindAsync(id);
            if (agendamentoModel != null)
            {
                _context.Agendamentos.Remove(agendamentoModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AgendamentoModelExists(int id)
        {
            return _context.Agendamentos.Any(e => e.AgendamentoId == id);
        }
    }
}
