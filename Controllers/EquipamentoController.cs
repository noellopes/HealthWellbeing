using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using static HealthWellbeing.Data.SeedData;

namespace HealthWellbeing.Controllers
{
    [Authorize]
    public class EquipamentoController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public EquipamentoController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Equipamento
        public async Task<IActionResult> Index(string searchNomeEquipamento, int page = 1)
        {
            var equipamentosQuery = _context.Equipamento.AsQueryable();

            if (!string.IsNullOrEmpty(searchNomeEquipamento))
            {
                equipamentosQuery = equipamentosQuery
                    .Where(e => e.NomeEquipamento.Contains(searchNomeEquipamento));
                ViewBag.SearchNomeEquipamento = searchNomeEquipamento;
            }

            int totalEquipamentos = await equipamentosQuery.CountAsync();
            var equipamentosInfo = new PaginationInfo<Equipamento>(page, totalEquipamentos);

            equipamentosInfo.Items = await equipamentosQuery
                .OrderBy(e => e.NomeEquipamento)
                .Skip(equipamentosInfo.ItemsToSkip)
                .Take(equipamentosInfo.ItemsPerPage)
                .ToListAsync();

            return View(equipamentosInfo);
        }

        // GET: Equipamento/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var equipamento = await _context.Equipamento
                .FirstOrDefaultAsync(m => m.EquipamentoId == id);

            if (equipamento == null) return NotFound();

            return View(equipamento);
        }

        // GET: Equipamento/Create
        [Authorize(Roles = SeedData.Roles.Administrador + "," + SeedData.Roles.Profissional)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Equipamento/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SeedData.Roles.Administrador + "," + SeedData.Roles.Profissional)]
        public async Task<IActionResult> Create([Bind("EquipamentoId,NomeEquipamento,RequerPeso")] Equipamento equipamento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(equipamento);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Details), new { id = equipamento.EquipamentoId, SuccessMessage = "Equipamento criado com sucesso" });
            }
            return View(equipamento);
        }

        // GET: Equipamento/Edit/5
        [Authorize(Roles = SeedData.Roles.Administrador + "," + SeedData.Roles.Profissional)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var equipamento = await _context.Equipamento.FindAsync(id);
            if (equipamento == null) return NotFound();

            return View(equipamento);
        }

        // POST: Equipamento/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SeedData.Roles.Administrador + "," + SeedData.Roles.Profissional)]
        public async Task<IActionResult> Edit(int id, [Bind("EquipamentoId,NomeEquipamento,RequerPeso")] Equipamento equipamento)
        {
            if (id != equipamento.EquipamentoId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var equipamentoExistente = await _context.Equipamento.FindAsync(id);
                    if (equipamentoExistente == null)
                    {
                        return View("InvalidEquipamento", equipamento);
                    }

                    _context.Entry(equipamentoExistente).CurrentValues.SetValues(equipamento);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Details), new { id = equipamento.EquipamentoId, SuccessMessage = "Equipamento editado com sucesso" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EquipamentoExists(equipamento.EquipamentoId))
                    {
                        return View("EquipamentoDeleted", equipamento);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(equipamento);
        }

        // GET: Equipamento/Delete/5
        [Authorize(Roles = SeedData.Roles.Administrador + "," + SeedData.Roles.Profissional)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var equipamento = await _context.Equipamento
                .Include(e => e.ExercicioEquipamentos)
                .FirstOrDefaultAsync(m => m.EquipamentoId == id);

            if (equipamento == null)
            {
                TempData["ErrorMessage"] = "Este equipamento já não existe.";
                return RedirectToAction(nameof(Index));
            }

            int numExercicios = equipamento.ExercicioEquipamentos?.Count ?? 0;
            ViewBag.NumExercicios = numExercicios;
            ViewBag.PodeEliminar = numExercicios == 0;

            return View(equipamento);
        }

        // POST: Equipamento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SeedData.Roles.Administrador + "," + SeedData.Roles.Profissional)]
        public async Task<IActionResult> DeleteConfirmed(int EquipamentoId)
        {
            var equipamento = await _context.Equipamento.FindAsync(EquipamentoId);
            if (equipamento == null) return RedirectToAction(nameof(Index));

            try
            {
                _context.Equipamento.Remove(equipamento);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Equipamento eliminado com sucesso.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Não é possível eliminar este equipamento porque ele está associado a um ou mais exercícios.";
                return RedirectToAction(nameof(Delete), new { id = EquipamentoId });
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Ocorreu um erro inesperado.";
                return RedirectToAction(nameof(Delete), new { id = EquipamentoId });
            }
        }

        private bool EquipamentoExists(int id)
        {
            return _context.Equipamento.Any(e => e.EquipamentoId == id);
        }
    }
}