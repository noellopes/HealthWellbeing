using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace HealthWellbeing.Controllers
{
    public class AgendamentoBalnearioController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public AgendamentoBalnearioController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Agendamento
        public async Task<IActionResult> Index()
        {
            var HealthWellbeingDbContext = _context.Agendamentos.Include(a => a.Servico).Include(a => a.Terapeuta).Include(a => a.UtenteBalneario);
            return View(await HealthWellbeingDbContext.ToListAsync());
        }

        // GET: Agendamento/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var agendamentos = await _context.Agendamentos
                .Include(a => a.Servico)
                .Include(a => a.Terapeuta)
                .Include(a => a.UtenteBalneario)
                .FirstOrDefaultAsync(m => m.AgendamentoId == id);
            if (agendamentos == null)
            {
                return NotFound();
            }

            return View(agendamentos);
        }

        // GET: Agendamento/Create
        public IActionResult Create()
        {
            ViewData["ServicoId"] = new SelectList(_context.Servicos, "ServicoId", "ServicoId");
            ViewData["TerapeutaId"] = new SelectList(_context.Terapeuta, "TerapeutaId", "TerapeutaId");
            ViewData["UtenteBalnearioId"] = new SelectList(_context.Set<UtenteBalneario>(), "UtenteBalnearioId", "UtenteBalnearioId");
            return View();
        }

        // POST: Agendamento/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AgendamentoId,DataHoraInicio,DataHoraFim,Estado,UtenteBalnearioId,TerapeutaId,ServicoId")] Models.AgendamentoBalneario agendamentobalneario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(agendamentobalneario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ServicoId"] = new SelectList(_context.Servicos, "ServicoId", "ServicoId", agendamentobalneario.ServicoId);
            ViewData["TerapeutaId"] = new SelectList(_context.Terapeuta, "TerapeutaId", "TerapeutaId", agendamentobalneario.TerapeutaId);
            ViewData["UtenteBalnearioId"] = new SelectList(_context.Set<UtenteBalneario>(), "UtenteBalnearioId", "UtenteBalnearioId", agendamentobalneario.UtenteBalnearioId);
            return View(agendamentobalneario);
        }

        // GET: Agendamento/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var agendamento = await _context.Agendamentos.FindAsync(id);
            if (agendamento == null)
            {
                return NotFound();
            }
            ViewData["ServicoId"] = new SelectList(_context.Servicos, "ServicoId", "ServicoId", agendamento.ServicoId);
            ViewData["TerapeutaId"] = new SelectList(_context.Terapeuta, "TerapeutaId", "TerapeutaId", agendamento.TerapeutaId);
            ViewData["UtenteBalnearioId"] = new SelectList(_context.Set<UtenteBalneario>(), "UtenteBalnearioId", "UtenteBalnearioId", agendamento.UtenteBalnearioId);
            return View(agendamento);
        }

        // POST: Agendamento/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AgendamentoId,DataHoraInicio,DataHoraFim,Estado,UtenteBalnearioId,TerapeutaId,ServicoId")] AgendamentoBalneario agendamento)
        {
            if (id != agendamento.AgendamentoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(agendamento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AgendamentoModelExists(agendamento.AgendamentoId))
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
            ViewData["ServicoId"] = new SelectList(_context.Servicos, "ServicoId", "ServicoId", agendamento.ServicoId);
            ViewData["TerapeutaId"] = new SelectList(_context.Terapeuta, "TerapeutaId", "TerapeutaId", agendamento.TerapeutaId);
            ViewData["UtenteBalnearioId"] = new SelectList(_context.Set<UtenteBalneario>(), "UtenteBalnearioId", "UtenteBalnearioId", agendamento.UtenteBalnearioId);
            return View(agendamento);
        }

        // GET: Agendamento/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var agendamentos = await _context.Agendamentos
                .Include(a => a.Servico)
                .Include(a => a.Terapeuta)
                .Include(a => a.UtenteBalneario)
                .FirstOrDefaultAsync(m => m.AgendamentoId == id);
            if (agendamentos == null)
            {
                return NotFound();
            }

            return View(agendamentos);
        }

        // POST: Agendamento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var agendamento = await _context.Agendamentos.FindAsync(id);
            if (agendamento != null)
            {
                _context.Agendamentos.Remove(agendamento);
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
