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
    public class AlergiasController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public AlergiasController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Alergias
        public async Task<IActionResult> Index(string nome, string alimento, int page = 1)
        {
            int pageSize = 10;

            var query = _context.Alergia
                .Include(a => a.AlimentosAssociados)
                    .ThenInclude(aa => aa.Alimento)
                .AsQueryable();

            var searchNome = nome?.ToLower();
            var searchAlimento = alimento?.ToLower();


            // FILTRO POR NOME DA ALERGIA
            if (!string.IsNullOrEmpty(searchNome))
                query = query.Where(a =>
                    EF.Functions.Collate(a.Nome, "SQL_Latin1_General_CP1_CI_AI")
                        .Contains(searchNome)
                );


            // FILTRO POR NOME DO ALIMENTO
            if (!string.IsNullOrEmpty(searchAlimento))
                query = query.Where(a => a.AlimentosAssociados
                    .Any(aa =>
                        EF.Functions.Collate(aa.Alimento.Name, "SQL_Latin1_General_CP1_CI_AI")
                            .Contains(searchAlimento)
                    )
                );

            // CONTAGEM TOTAL
            int totalCount = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            // PAGINAÇÃO
            var alergias = await query
                .OrderBy(a => a.Nome)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;

            ViewBag.SearchNome = nome;
            ViewBag.SearchAlimento = alimento;

            return View(alergias);
        }


        // GET: Alergias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var alergia = await _context.Alergia
                .Include(a => a.AlimentosAssociados)
                    .ThenInclude(aa => aa.Alimento)
                .FirstOrDefaultAsync(m => m.AlergiaId == id);

            if (alergia == null)
                return NotFound();

            return View(alergia);
        }

        // GET: Alergias/Create
        public IActionResult Create()
        {
            var alimentos = _context.Alimentos
                .Select(a => new { id = a.AlimentoId, nome = a.Name })
                .ToList();
            ViewBag.Alimentos = alimentos;

            // Passando os valores do enum GravidadeAlergia para a view
            ViewBag.Gravidades = Enum.GetValues(typeof(GravidadeAlergia))
                                     .Cast<GravidadeAlergia>()
                                     .Select(g => new SelectListItem
                                     {
                                         Value = g.ToString(),
                                         Text = g.ToString()
                                     }).ToList();

            return View();
        }



        // POST: Alergias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Alergia alergia, string selectedAlimentosIds)
        {
            if (ModelState.IsValid)
            {
                _context.Add(alergia);
                await _context.SaveChangesAsync();

                if (!string.IsNullOrEmpty(selectedAlimentosIds))
                {
                    var ids = selectedAlimentosIds.Split(',').Select(int.Parse).ToList();
                    foreach (var id in ids)
                    {
                        var alergiaAlimento = new AlergiaAlimento
                        {
                            AlergiaId = alergia.AlergiaId,
                            AlimentoId = id
                        };
                        _context.AlergiaAlimento.Add(alergiaAlimento);
                    }
                    await _context.SaveChangesAsync();
                }

                TempData["AlertType"] = "success";
                TempData["AlertMessage"] = "Alergia criada com sucesso!";

                return RedirectToAction(nameof(Details), new { id = alergia.AlergiaId });
            }

            return View(alergia);
        }



        // GET: Alergias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var alergia = await _context.Alergia
                .Include(a => a.AlimentosAssociados)
                    .ThenInclude(aa => aa.Alimento)
                .FirstOrDefaultAsync(a => a.AlergiaId == id);

            if (alergia == null) return NotFound();

            var alimentos = _context.Alimentos
                .Select(a => new { id = a.AlimentoId, nome = a.Name })
                .ToList();
            ViewBag.Alimentos = alimentos;

            // Passar alimentos já selecionados para JS
            ViewBag.SelectedAlimentos = alergia.AlimentosAssociados
                .Select(aa => new { id = aa.AlimentoId, nome = aa.Alimento.Name })
                .ToList();

            // Enum Gravidade
            ViewBag.Gravidades = Enum.GetValues(typeof(GravidadeAlergia))
                                     .Cast<GravidadeAlergia>()
                                     .Select(g => new SelectListItem
                                     {
                                         Value = g.ToString(),
                                         Text = g.ToString()
                                     }).ToList();

            return View(alergia);
        }


        // POST: Alergias/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Alergia alergia, string selectedAlimentosIds)
        {
            if (id != alergia.AlergiaId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Atualiza dados básicos
                    _context.Update(alergia);
                    await _context.SaveChangesAsync();

                    // Atualiza relações Alimento <-> Alergia
                    var existing = _context.AlergiaAlimento
                        .Where(aa => aa.AlergiaId == id)
                        .ToList();

                    _context.AlergiaAlimento.RemoveRange(existing);

                    if (!string.IsNullOrEmpty(selectedAlimentosIds))
                    {
                        var ids = selectedAlimentosIds.Split(',').Select(int.Parse).ToList();
                        foreach (var aid in ids)
                        {
                            _context.AlergiaAlimento.Add(new AlergiaAlimento
                            {
                                AlergiaId = id,
                                AlimentoId = aid
                            });
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlergiaExists(alergia.AlergiaId)) return NotFound();
                    else throw;
                }

                TempData["AlertType"] = "success";
                TempData["AlertMessage"] = "Alergia atualizada com sucesso!";

                return RedirectToAction(nameof(Details), new { id = id });
            }

            // Recarrega dados caso ModelState não seja válido
            var alimentos = _context.Alimentos
                .Select(a => new { id = a.AlimentoId, nome = a.Name })
                .ToList();
            ViewBag.Alimentos = alimentos;
            ViewBag.SelectedAlimentos = selectedAlimentosIds?.Split(',')
                .Select(id => new { id = int.Parse(id), nome = alimentos.First(a => a.id == int.Parse(id)).nome })
                .ToList();

            ViewBag.Gravidades = Enum.GetValues(typeof(GravidadeAlergia))
                                     .Cast<GravidadeAlergia>()
                                     .Select(g => new SelectListItem
                                     {
                                         Value = g.ToString(),
                                         Text = g.ToString()
                                     }).ToList();

            return View(alergia);
        }


        // GET: Alergias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var alergia = await _context.Alergia
                .FirstOrDefaultAsync(m => m.AlergiaId == id);

            if (alergia == null)
                return NotFound();

            return View(alergia);
        }

        // POST: Alergias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var alergia = await _context.Alergia.FindAsync(id);
            if (alergia != null)
                _context.Alergia.Remove(alergia);

            await _context.SaveChangesAsync();

            TempData["AlertType"] = "warning";
            TempData["AlertMessage"] = "Alergia apagada com sucesso!";

            return RedirectToAction(nameof(Index));
        }

        private bool AlergiaExists(int id)
        {
            return _context.Alergia.Any(e => e.AlergiaId == id);
        }
    }
}
