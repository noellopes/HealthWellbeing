using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    [Authorize]
    public class RestricaoAlimentarController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public RestricaoAlimentarController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: RestricaoAlimentar
        public async Task<IActionResult> Index(string nome, string tipo, int page = 1)
        {
            int pageSize = 10;

            var query = _context.RestricaoAlimentar
                .Include(r => r.AlimentosAssociados)
                    .ThenInclude(ra => ra.Alimento)
                .AsQueryable();

            if (!string.IsNullOrEmpty(nome))
                query = query.Where(r => r.Nome.Contains(nome));

            if (!string.IsNullOrEmpty(tipo) && Enum.TryParse<TipoRestricao>(tipo, out var tipoRestricao))
                query = query.Where(r => r.Tipo == tipoRestricao);

            int totalCount = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var restricoes = await query
                .OrderBy(r => r.Nome)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;

            ViewBag.SearchNome = nome;
            ViewBag.SearchTipo = tipo;

            return View(restricoes);
        }

        // GET: RestricaoAlimentar/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var restricao = await _context.RestricaoAlimentar
                .Include(r => r.AlimentosAssociados)
                    .ThenInclude(ra => ra.Alimento)
                .FirstOrDefaultAsync(r => r.RestricaoAlimentarId == id);

            if (restricao == null)
                return NotFound();

            return View(restricao);
        }

        // GET: RestricaoAlimentar/Create
        [Authorize(Roles = "Nutricionista")]
        public IActionResult Create()
        {
            ViewBag.Alimentos = _context.Alimentos
                .Select(a => new { id = a.AlimentoId, nome = a.Name })
                .ToList();

            ViewBag.Gravidades = Enum.GetValues(typeof(GravidadeRestricao))
                .Cast<GravidadeRestricao>()
                .Select(g => new SelectListItem { Value = g.ToString(), Text = g.ToString() })
                .ToList();

            return View();
        }

        // POST: RestricaoAlimentar/Create
        [Authorize(Roles = "Nutricionista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RestricaoAlimentar restricao, string selectedAlimentosIds)
        {
            if (ModelState.IsValid)
            {
                _context.Add(restricao);
                await _context.SaveChangesAsync();

                if (!string.IsNullOrEmpty(selectedAlimentosIds))
                {
                    var ids = selectedAlimentosIds.Split(',').Select(int.Parse).ToList();

                    foreach (var alimentoId in ids)
                    {
                        _context.RestricaoAlimentarAlimento.Add(new RestricaoAlimentarAlimento
                        {
                            RestricaoAlimentarId = restricao.RestricaoAlimentarId,
                            AlimentoId = alimentoId
                        });
                    }

                    await _context.SaveChangesAsync();
                }

                TempData["AlertType"] = "success";
                TempData["AlertMessage"] = "Restrição criada com sucesso!";

                return RedirectToAction(nameof(Index));
            }

            // Recarrega dados em caso de erro
            ViewBag.Alimentos = _context.Alimentos
                .Select(a => new { id = a.AlimentoId, nome = a.Name })
                .ToList();

            ViewBag.Gravidades = Enum.GetValues(typeof(GravidadeRestricao))
                .Cast<GravidadeRestricao>()
                .Select(g => new SelectListItem { Value = g.ToString(), Text = g.ToString() })
                .ToList();

            return View(restricao);
        }

        // GET: RestricaoAlimentar/Edit/5
        [Authorize(Roles = "Nutricionista")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var restricao = await _context.RestricaoAlimentar
                .Include(r => r.AlimentosAssociados)
                    .ThenInclude(ra => ra.Alimento)
                .FirstOrDefaultAsync(r => r.RestricaoAlimentarId == id);

            if (restricao == null) return NotFound();

            var alimentos = _context.Alimentos
                .Select(a => new { id = a.AlimentoId, nome = a.Name })
                .ToList();
            ViewBag.Alimentos = alimentos;

            ViewBag.SelectedAlimentos = restricao.AlimentosAssociados
                .Select(ra => new { id = ra.AlimentoId, nome = ra.Alimento.Name })
                .ToList();

            ViewBag.Gravidades = Enum.GetValues(typeof(GravidadeRestricao))
                .Cast<GravidadeRestricao>()
                .Select(g => new SelectListItem { Value = g.ToString(), Text = g.ToString() })
                .ToList();

            return View(restricao);
        }

        // POST: RestricaoAlimentar/Edit/5
        [Authorize(Roles = "Nutricionista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RestricaoAlimentar restricao, string selectedAlimentosIds)
        {
            if (id != restricao.RestricaoAlimentarId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Atualiza a restrição principal
                    _context.Update(restricao);

                    // Remove associações antigas
                    var existingAssociations = _context.RestricaoAlimentarAlimento
                        .Where(ra => ra.RestricaoAlimentarId == id)
                        .ToList();

                    _context.RestricaoAlimentarAlimento.RemoveRange(existingAssociations);

                    // Adiciona associações novas
                    if (!string.IsNullOrEmpty(selectedAlimentosIds))
                    {
                        var ids = selectedAlimentosIds.Split(',').Select(int.Parse).ToList();

                        foreach (var alimentoId in ids)
                        {
                            _context.RestricaoAlimentarAlimento.Add(new RestricaoAlimentarAlimento
                            {
                                RestricaoAlimentarId = id,
                                AlimentoId = alimentoId
                            });
                        }
                    }

                    await _context.SaveChangesAsync();

                    TempData["AlertType"] = "success";
                    TempData["AlertMessage"] = "Restrição atualizada com sucesso!";

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RestricaoAlimentarExists(restricao.RestricaoAlimentarId))
                        return NotFound();
                    else throw;
                }
            }

            // Recarrega dados em caso de erro de validação
            var alimentos = _context.Alimentos
                .Select(a => new { id = a.AlimentoId, nome = a.Name })
                .ToList();

            ViewBag.Alimentos = alimentos;

            // Recria a lista de alimentos selecionados
            if (!string.IsNullOrEmpty(selectedAlimentosIds))
            {
                ViewBag.SelectedAlimentos = selectedAlimentosIds.Split(',')
                    .Select(idStr => new
                    {
                        id = int.Parse(idStr),
                        nome = alimentos.FirstOrDefault(a => a.id == int.Parse(idStr))?.nome ?? "Alimento não encontrado"
                    }).ToList();
            }
            else
            {
                ViewBag.SelectedAlimentos = new List<object>();
            }

            ViewBag.Gravidades = Enum.GetValues(typeof(GravidadeRestricao))
                .Cast<GravidadeRestricao>()
                .Select(g => new SelectListItem { Value = g.ToString(), Text = g.ToString() })
                .ToList();

            return View(restricao);
        }

        // GET: RestricaoAlimentar/Delete/5
        [Authorize(Roles = "Nutricionista")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var restricao = await _context.RestricaoAlimentar
                .Include(r => r.AlimentosAssociados)
                    .ThenInclude(ra => ra.Alimento)
                .FirstOrDefaultAsync(r => r.RestricaoAlimentarId == id);

            if (restricao == null) return NotFound();

            return View(restricao);
        }

        // POST: RestricaoAlimentar/Delete/5
        [Authorize(Roles = "Nutricionista")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var restricao = await _context.RestricaoAlimentar
                .Include(r => r.AlimentosAssociados)
                .FirstOrDefaultAsync(r => r.RestricaoAlimentarId == id);

            if (restricao != null)
            {
                // Remove as associações primeiro (cascata)
                _context.RestricaoAlimentarAlimento.RemoveRange(restricao.AlimentosAssociados);
                
                // Remove a restrição
                _context.RestricaoAlimentar.Remove(restricao);
                
                await _context.SaveChangesAsync();
            }

            TempData["AlertType"] = "warning";
            TempData["AlertMessage"] = "Restrição apagada com sucesso!";

            return RedirectToAction(nameof(Index));
        }

        private bool RestricaoAlimentarExists(int id)
        {
            return _context.RestricaoAlimentar.Any(e => e.RestricaoAlimentarId == id);
        }
    }
}





