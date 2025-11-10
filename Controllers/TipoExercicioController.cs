using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellbeing.Controllers
{
    public class TipoExercicioController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public TipoExercicioController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: TipoExercicio
        public async Task<IActionResult> Index(
    int page = 1,
    string searchNome = "",
    string searchDescricao = "",
    string searchBeneficio = "")
        {
            var query = _context.TipoExercicio
                .Include(t => t.Beneficios)
                .AsQueryable();

            // 🔍 Filtragem por Nome
            if (!string.IsNullOrEmpty(searchNome))
            {
                query = query.Where(t => t.NomeTipoExercicios.Contains(searchNome));
            }

            // 🔍 Filtragem por Descrição
            if (!string.IsNullOrEmpty(searchDescricao))
            {
                query = query.Where(t => t.DescricaoTipoExercicios.Contains(searchDescricao));
            }

            // 🔍 Filtragem por Benefício
            if (!string.IsNullOrEmpty(searchBeneficio))
            {
                query = query.Where(t => t.Beneficios.Any(b => b.NomeBeneficio.Contains(searchBeneficio)));
            }

            // Guardar os valores de pesquisa para a view
            ViewBag.SearchNome = searchNome;
            ViewBag.SearchDescricao = searchDescricao;
            ViewBag.SearchBeneficio = searchBeneficio;

            // Paginação
            int total = await query.CountAsync();
            var pagination = new PaginationInfoExercicios<TipoExercicio>(page, total);

            pagination.Items = await query
                .OrderBy(t => t.NomeTipoExercicios)
                .Skip(pagination.ItemsToSkip)
                .Take(pagination.ItemsPerPage)
                .ToListAsync();

            return View(pagination);
        }

        // GET: TipoExercicio/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoExercicio = await _context.TipoExercicio
                .Include(t => t.Beneficios) // ✅ ADICIONADO: Carrega os benefícios relacionados
                .FirstOrDefaultAsync(m => m.TipoExercicioId == id);
            if (tipoExercicio == null)
            {
                return NotFound();
            }

            return View(tipoExercicio);
        }

        // GET: TipoExercicio/Create
        public IActionResult Create()
        {
            // ✅ ADICIONADO: Carrega os benefícios para mostrar no checkbox list
            ViewData["Beneficios"] = _context.Beneficio.ToList();
            return View();
        }

        // POST: TipoExercicio/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("TipoExercicioId,NomeTipoExercicios,DescricaoTipoExercicios,CaracteristicasTipoExercicios")]
            TipoExercicio tipoExercicio,
            int[] selectedBeneficios) // ✅ ADICIONADO: Parâmetro para benefícios selecionados
        {
            if (ModelState.IsValid)
            {
                // ✅ ADICIONADO: Lógica para associar benefícios selecionados
                if (selectedBeneficios != null && selectedBeneficios.Any())
                {
                    tipoExercicio.Beneficios = new List<Beneficio>();
                    foreach (var beneficioId in selectedBeneficios)
                    {
                        var beneficio = await _context.Beneficio.FindAsync(beneficioId);
                        if (beneficio != null)
                        {
                            tipoExercicio.Beneficios.Add(beneficio);
                        }
                    }
                }

                _context.Add(tipoExercicio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // ✅ ADICIONADO: Recarrega benefícios em caso de erro
            ViewData["Beneficios"] = _context.Beneficio.ToList();
            return View(tipoExercicio);
        }

        // GET: TipoExercicio/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoExercicio = await _context.TipoExercicio
                .Include(t => t.Beneficios) // ✅ ADICIONADO: Carrega benefícios relacionados
                .FirstOrDefaultAsync(m => m.TipoExercicioId == id);
            if (tipoExercicio == null)
            {
                return NotFound();
            }

            // ✅ ADICIONADO: Carrega lista de benefícios e os selecionados
            ViewData["Beneficios"] = _context.Beneficio.ToList();
            ViewData["SelectedBeneficios"] = tipoExercicio.Beneficios?
                .Select(b => b.BeneficioId).ToList() ?? new List<int>();

            return View(tipoExercicio);
        }

        // POST: TipoExercicio/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("TipoExercicioId,NomeTipoExercicios,DescricaoTipoExercicios,CaracteristicasTipoExercicios")]
            TipoExercicio tipoExercicio,
            int[] selectedBeneficios) // ✅ ADICIONADO: Parâmetro para benefícios selecionados
        {
            if (id != tipoExercicio.TipoExercicioId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // ✅ ADICIONADO: Carrega entidade existente com benefícios
                    var tipoExercicioExistente = await _context.TipoExercicio
                        .Include(t => t.Beneficios)
                        .FirstOrDefaultAsync(t => t.TipoExercicioId == id);

                    if (tipoExercicioExistente == null)
                    {
                        return NotFound();
                    }

                    // ✅ ADICIONADO: Atualiza propriedades básicas
                    tipoExercicioExistente.NomeTipoExercicios = tipoExercicio.NomeTipoExercicios;
                    tipoExercicioExistente.DescricaoTipoExercicios = tipoExercicio.DescricaoTipoExercicios;
                    tipoExercicioExistente.CaracteristicasTipoExercicios = tipoExercicio.CaracteristicasTipoExercicios;

                    // ✅ ADICIONADO: Atualiza benefícios
                    tipoExercicioExistente.Beneficios?.Clear();
                    if (selectedBeneficios != null && selectedBeneficios.Any())
                    {
                        tipoExercicioExistente.Beneficios ??= new List<Beneficio>();
                        foreach (var beneficioId in selectedBeneficios)
                        {
                            var beneficio = await _context.Beneficio.FindAsync(beneficioId);
                            if (beneficio != null)
                            {
                                tipoExercicioExistente.Beneficios.Add(beneficio);
                            }
                        }
                    }

                    _context.Update(tipoExercicioExistente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoExercicioExists(tipoExercicio.TipoExercicioId))
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

            // ✅ ADICIONADO: Recarrega dados em caso de erro
            ViewData["Beneficios"] = _context.Beneficio.ToList();
            var tipoExercicioComBeneficios = await _context.TipoExercicio
                .Include(t => t.Beneficios)
                .FirstOrDefaultAsync(t => t.TipoExercicioId == id);
            ViewData["SelectedBeneficios"] = tipoExercicioComBeneficios?.Beneficios?
                .Select(b => b.BeneficioId).ToList() ?? new List<int>();

            return View(tipoExercicio);
        }

        // GET: TipoExercicio/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoExercicio = await _context.TipoExercicio
                .Include(t => t.Beneficios) // ✅ ADICIONADO: Carrega benefícios para mostrar relações
                .FirstOrDefaultAsync(m => m.TipoExercicioId == id);
            if (tipoExercicio == null)
            {
                return NotFound();
            }

            return View(tipoExercicio);
        }

        // POST: TipoExercicio/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipoExercicio = await _context.TipoExercicio.FindAsync(id);
            if (tipoExercicio != null)
            {
                _context.TipoExercicio.Remove(tipoExercicio);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TipoExercicioExists(int id)
        {
            return _context.TipoExercicio.Any(e => e.TipoExercicioId == id);
        }
    }
}