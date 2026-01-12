using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellbeing.Controllers
{
    [Authorize(Roles = SeedData.Roles.Administrador + "," + SeedData.Roles.Profissional)]
    public class ObjetivoFisicoController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ObjetivoFisicoController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: ObjetivoFisico
        public async Task<IActionResult> Index(int page = 1, string searchNome = "")
        {
            var query = _context.ObjetivoFisico.AsQueryable();

            if (!string.IsNullOrEmpty(searchNome))
            {
                query = query.Where(o => o.NomeObjetivo.Contains(searchNome));
            }

            ViewBag.SearchNome = searchNome;

            int total = await query.CountAsync();
            var pagination = new PaginationInfo<ObjetivoFisico>(page, total);

            if (total > 0)
            {
                pagination.Items = await query.OrderBy(o => o.NomeObjetivo)
                    .Skip(pagination.ItemsToSkip).Take(pagination.ItemsPerPage).ToListAsync();
            }
            else pagination.Items = new List<ObjetivoFisico>();

            return View(pagination);
        }

        // GET: ObjetivoFisico/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var objetivoFisico = await _context.ObjetivoFisico.FirstOrDefaultAsync(m => m.ObjetivoFisicoId == id);
            if (objetivoFisico == null) return NotFound();
            return View(objetivoFisico);
        }

        // GET: ObjetivoFisico/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ObjetivoFisico/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ObjetivoFisicoId,NomeObjetivo")] ObjetivoFisico objetivoFisico)
        {
            if (ModelState.IsValid)
            {
                _context.Add(objetivoFisico);
                await _context.SaveChangesAsync();
                TempData["StatusMessage"] = "Sucesso: Objetivo Físico criado.";
                return RedirectToAction(nameof(Index));
            }
            return View(objetivoFisico);
        }

        // GET: ObjetivoFisico/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var objetivoFisico = await _context.ObjetivoFisico.FindAsync(id);
            if (objetivoFisico == null) return NotFound();
            return View(objetivoFisico);
        }

        // POST: ObjetivoFisico/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ObjetivoFisicoId,NomeObjetivo")] ObjetivoFisico objetivoFisico)
        {
            if (id != objetivoFisico.ObjetivoFisicoId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(objetivoFisico);
                    await _context.SaveChangesAsync();
                    TempData["StatusMessage"] = "Sucesso: Objetivo Físico atualizado.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ObjetivoFisicoExists(objetivoFisico.ObjetivoFisicoId)) return View("InvalidObjetivoFisico", objetivoFisico);
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(objetivoFisico);
        }

        // GET: ObjetivoFisico/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var objetivoFisico = await _context.ObjetivoFisico
                .Include(o => o.ExercicioObjetivos)
                .Include(o => o.Utentes)
                .FirstOrDefaultAsync(m => m.ObjetivoFisicoId == id);

            if (objetivoFisico == null)
            {
                TempData["ErrorMessage"] = "Este objetivo já não existe.";
                return RedirectToAction(nameof(Index));
            }

            int numExercicios = objetivoFisico.ExercicioObjetivos?.Count ?? 0;
            int numUtentes = objetivoFisico.Utentes?.Count ?? 0;

            if (numUtentes > 0)
            {
                ViewBag.PodeEliminar = false;
                ViewBag.MensagemErro = $"Não é possível eliminar '{objetivoFisico.NomeObjetivo}' porque está atribuído a {numUtentes} Utente(s).";
            }
            else if (numExercicios > 0)
            {
                ViewBag.PodeEliminar = false;
                ViewBag.MensagemErro = $"Não é possível eliminar '{objetivoFisico.NomeObjetivo}' porque está associado a {numExercicios} Exercício(s).";
            }
            else
            {
                ViewBag.PodeEliminar = true;
            }

            ViewBag.NumExercicios = numExercicios;
            ViewBag.NumUtentes = numUtentes;

            return View(objetivoFisico);
        }

        // POST: ObjetivoFisico/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int ObjetivoFisicoId)
        {
            var objetivoFisico = await _context.ObjetivoFisico.FindAsync(ObjetivoFisicoId);
            if (objetivoFisico == null) return RedirectToAction(nameof(Index));

            try
            {
                _context.ObjetivoFisico.Remove(objetivoFisico);
                await _context.SaveChangesAsync();
                TempData["StatusMessage"] = "Sucesso: Objetivo eliminado.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ObjetivoFisicoExists(ObjetivoFisicoId)) return RedirectToAction(nameof(Index));
                else throw;
            }
            catch (DbUpdateException)
            {
                TempData["StatusMessage"] = "Erro: Existem dependências que impedem a eliminação.";
            }
            catch (Exception)
            {
                TempData["StatusMessage"] = "Erro: Ocorreu um erro inesperado.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ObjetivoFisicoExists(int id)
        {
            return _context.ObjetivoFisico.Any(e => e.ObjetivoFisicoId == id);
        }
    }
}