using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthWellbeing.Controllers
{
    public class AgendamentosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AgendamentosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // LISTAGEM
        // =========================
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var agendamentos = _context.Agendamentos
                .Include(a => a.Terapeuta);

            return View(await agendamentos.ToListAsync());
        }

        // =========================
        // DETALHES
        // =========================
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var agendamento = await _context.Agendamentos
                .Include(a => a.Terapeuta)
                .FirstOrDefaultAsync(a => a.AgendamentoId == id);

            if (agendamento == null)
                return NotFound();

            return View(agendamento);
        }

        // =========================
        // CRIAR
        // =========================
        [Authorize(Roles = "Admin,Funcionario")]
        public IActionResult Create()
        {
            ViewData["TerapeutaId"] =
                new SelectList(_context.Terapeutas, "TerapeutaId", "Nome");

            ViewData["ServicoId"] =
                new SelectList(_context.Servico, "ServicoId", "Nome");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> Create(Agendamento agendamento)
        {
            if (!ModelState.IsValid)
            {
                ViewData["TerapeutaId"] =
                    new SelectList(_context.Terapeutas, "TerapeutaId", "Nome", agendamento.TerapeutaId);

                ViewData["ServicoId"] =
                    new SelectList(_context.Servico, "ServicoId", "Nome", agendamento.ServicoId);

                return View(agendamento);
            }

            _context.Agendamentos.Add(agendamento);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // EDITAR
        // =========================
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var agendamento = await _context.Agendamentos.FindAsync(id);

            if (agendamento == null)
                return NotFound();

            ViewData["TerapeutaId"] =
                new SelectList(_context.Terapeutas, "TerapeutaId", "Nome", agendamento.TerapeutaId);

            ViewData["ServicoId"] =
                new SelectList(_context.Servico, "ServicoId", "Nome", agendamento.ServicoId);

            return View(agendamento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> Edit(int id, Agendamento agendamento)
        {
            if (id != agendamento.AgendamentoId)
                return NotFound();

            if (!ModelState.IsValid)
                return View(agendamento);

            _context.Update(agendamento);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // REMOVER
        // =========================
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var agendamento = await _context.Agendamentos
                .Include(a => a.Terapeuta)
                .FirstOrDefaultAsync(a => a.AgendamentoId == id);

            if (agendamento == null)
                return NotFound();

            return View(agendamento);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var agendamento = await _context.Agendamentos.FindAsync(id);

            if (agendamento != null)
            {
                _context.Agendamentos.Remove(agendamento);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}