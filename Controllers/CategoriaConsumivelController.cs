using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Models;
using HealthWellbeing.Data;

namespace HealthWellbeing.Controllers
{
    public class CategoriaConsumivelController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public CategoriaConsumivelController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: CategoriaConsumivel
        public async Task<IActionResult> Index(string searchString, int page = 1)
        {
            int pageSize = 10;
            

            // Consulta base
            var categoriasQuery = _context.CategoriaConsumivel.AsQueryable();

            // Aplicar filtro de pesquisa se houver
            if (!string.IsNullOrEmpty(searchString))
            {
                categoriasQuery = categoriasQuery.Where(c => c.Nome.Contains(searchString));
            }

            var totalCategorias = await categoriasQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCategorias / (double)pageSize);

            var categoriasPagina = await categoriasQuery
                .OrderBy(c => c.Nome)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.SearchString = searchString;

            return View(categoriasPagina);
        }

        // GET: CategoriaConsumivel/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var categoriaConsumivel = await _context.CategoriaConsumivel
                .FirstOrDefaultAsync(m => m.CategoriaId == id);
            if (categoriaConsumivel == null) return NotFound();

            return View(categoriaConsumivel);
        }

        // GET: CategoriaConsumivel/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CategoriaConsumivel/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoriaId,Nome,Descricao")] CategoriaConsumivel categoriaConsumivel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(categoriaConsumivel);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Categoria criada com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            return View(categoriaConsumivel);
        }

        // GET: CategoriaConsumivel/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var categoriaConsumivel = await _context.CategoriaConsumivel.FindAsync(id);
            if (categoriaConsumivel == null) return NotFound();

            return View(categoriaConsumivel);
        }

        // POST: CategoriaConsumivel/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoriaId,Nome,Descricao")] CategoriaConsumivel categoriaConsumivel)
        {
            if (id != categoriaConsumivel.CategoriaId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoriaConsumivel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriaConsumivelExists(categoriaConsumivel.CategoriaId)) return NotFound();
                    else throw;
                }
                TempData["SuccessMessage"] = "Categoria alterada com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            return View(categoriaConsumivel);
        }

        // GET: CategoriaConsumivel/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var categoriaConsumivel = await _context.CategoriaConsumivel
                .FirstOrDefaultAsync(m => m.CategoriaId == id);
            if (categoriaConsumivel == null) return NotFound();

            return View(categoriaConsumivel);
        }

        // POST: CategoriaConsumivel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoriaConsumivel = await _context.CategoriaConsumivel.FindAsync(id);

            if (categoriaConsumivel == null)
            {
                TempData["ErrorMessage"] = "A categoria selecionada não foi encontrada ou já foi eliminada.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.CategoriaConsumivel.Remove(categoriaConsumivel);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"A categoria \"{categoriaConsumivel.Nome}\" foi eliminada com sucesso.";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = $"Não foi possível eliminar a categoria \"{categoriaConsumivel.Nome}\" porque está associada a outros registos.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Ocorreu um erro inesperado ao eliminar a categoria.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CategoriaConsumivelExists(int id)
        {
            return _context.CategoriaConsumivel.Any(e => e.CategoriaId == id);
        }
    }
}