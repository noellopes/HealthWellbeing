using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellbeing.Controllers
{
    public class UtenteBalnearioController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UtenteBalnearioController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // INDEX
        // =========================
        public async Task<IActionResult> Index()
        {
            var utentes = await _context.UtenteBalnearios
                .Include(u => u.Genero)
                .ToListAsync();

            return View(utentes);
        }

        // =========================
        // DETAILS
        // =========================
        public async Task<IActionResult> Details(int id)
        {
            var utente = await _context.UtenteBalnearios
                .Include(u => u.Genero)
                .FirstOrDefaultAsync(u => u.UtenteBalnearioId == id);

            if (utente == null)
                return NotFound();

            return View(utente);
        }

        // =========================
        // CREATE
        // =========================
        public IActionResult Create()
        {
            LoadGeneros();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UtenteBalneario utente)
        {
            

            if (!ModelState.IsValid)
            {
                LoadGeneros();
                return View(utente);
            }

            utente.DataInscricao = DateTime.Now;
            utente.Ativo = true;

            _context.Add(utente);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Utente criado com sucesso!";
            return RedirectToAction(nameof(Details), new { id = utente.UtenteBalnearioId });
        }

        // =========================
        // EDIT
        // =========================
        public async Task<IActionResult> Edit(int id)
        {
            var utente = await _context.UtenteBalnearios.FindAsync(id);

            if (utente == null)
                return NotFound();

            LoadGeneros();
            return View(utente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UtenteBalneario utente)
        {
            if (id != utente.UtenteBalnearioId)
                return NotFound();

           

            if (!ModelState.IsValid)
            {
                LoadGeneros();
                return View(utente);
            }

            try
            {
                _context.Update(utente);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Utente atualizado com sucesso!";
                return RedirectToAction(nameof(Details), new { id = utente.UtenteBalnearioId });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.UtenteBalnearios.Any(e => e.UtenteBalnearioId == id))
                    return NotFound();

                throw;
            }
        }

        // =========================
        // HELPERS
        // =========================
        private void LoadGeneros()
        {
            ViewBag.Generos = new SelectList(
                _context.Generos.OrderBy(g => g.Nome),
                "GeneroId",
                "Nome"
            );
        }
    }
}
