using System;
using System.Linq;
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

        private const int PageSize = 10;
        private const int PageWindow = 5;

        public AgendamentosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // LISTAGEM
        // =========================
        [AllowAnonymous]
        public async Task<IActionResult> Index(int page = 1)
        {
            if (page < 1) page = 1;

            var query = _context.Agendamentos
                .Include(a => a.Terapeuta)
                .OrderBy(a => a.DataHoraInicio);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            if (page > totalPages && totalPages > 0)
                page = totalPages;

            var agendamentos = await query
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            // cálculo da janela de páginas
            var startPage = Math.Max(1, page - PageWindow / 2);
            var endPage = Math.Min(totalPages, startPage + PageWindow - 1);

            if (endPage - startPage + 1 < PageWindow)
            {
                startPage = Math.Max(1, endPage - PageWindow + 1);
            }

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.StartPage = startPage;
            ViewBag.EndPage = endPage;
            ViewBag.HasPrevious = page > 1;
            ViewBag.HasNext = page < totalPages;

            return View(agendamentos);
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
        [AllowAnonymous]
        public IActionResult Create()
        {
            ViewData["TerapeutaId"] =
                new SelectList(_context.Terapeutas, "TerapeutaId", "Nome");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Create(Agendamento agendamento)
        {
            // estado fixo na criação
            agendamento.Estado = "Pendente";

            if (!ModelState.IsValid)
            {
                ViewData["TerapeutaId"] =
                    new SelectList(_context.Terapeutas, "TerapeutaId", "Nome", agendamento.TerapeutaId);

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
            {
                ViewData["TerapeutaId"] =
                    new SelectList(_context.Terapeutas, "TerapeutaId", "Nome", agendamento.TerapeutaId);

                return View(agendamento);
            }

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