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
    public class AlimentoSubstitutoController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public AlimentoSubstitutoController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: AlimentoSubstituto
        public async Task<IActionResult> Index(string originalName, string substitutoName, string factorOp, decimal? factorValue, int? page)
        {
            int pageSize = 10; // agora representa número de ALIMENTOS ORIGINAIS por página
            int pageNumber = page ?? 1;

            var query = _context.AlimentoSubstitutos
                .Include(a => a.AlimentoOriginal)
                .Include(a => a.AlimentoSubstitutoRef)
                .AsQueryable();


            if (!string.IsNullOrWhiteSpace(originalName))
            {
                var pattern = $"%{originalName}%";

                query = query.Where(a =>
                    a.AlimentoOriginal != null &&
                    EF.Functions.Like(EF.Functions.Collate(a.AlimentoOriginal.Name, "Latin1_General_CI_AI"), pattern));
            }

            if (!string.IsNullOrWhiteSpace(substitutoName))
            {
                var pattern = $"%{substitutoName}%";
                query = query.Where(a =>
                    a.AlimentoSubstitutoRef != null &&
                    EF.Functions.Like(EF.Functions.Collate(a.AlimentoSubstitutoRef.Name, "Latin1_General_CI_AI"), pattern));
            }

            if (factorValue.HasValue)
            {

                switch (factorOp)
                {
                    case "gt":
                        query = query.Where(a => a.FatorSimilaridade.HasValue && a.FatorSimilaridade.Value > (double)factorValue.Value);
                        break;
                    case "eq":

                        query = query.Where(a => a.FatorSimilaridade.HasValue && a.FatorSimilaridade.Value == (double)factorValue.Value);
                        break;
                    case "lt":
                        query = query.Where(a => a.FatorSimilaridade.HasValue && a.FatorSimilaridade.Value < (double)factorValue.Value);
                        break;
                    default:
                        break;
                }
            }

            // Ordenação base (mantida)
            query = query.OrderBy(a => a.AlimentoOriginal!.Name).ThenBy(a => a.AlimentoSubstitutoRef!.Name).ThenBy(a => a.AlimentoSubstitutoId);

            // obter todos os itens filtrados (agruparemos por AlimentoOriginal)
            var allItems = await query.ToListAsync();

            // agrupa por alimento original
            var groups = allItems
                .GroupBy(a => a.AlimentoOriginal != null ? a.AlimentoOriginal.Name : "(Sem Nome)")
                .OrderBy(g => g.Key)
                .ToList();

            int totalOriginals = groups.Count; // TOTAL agora é o número de alimentos originais

            // paginar por grupos (cada "page" tem até pageSize grupos)
            int totalPages = Math.Max(1, (int)Math.Ceiling((double)totalOriginals / pageSize));

            if (pageNumber < 1) pageNumber = 1;
            if (pageNumber > totalPages) pageNumber = totalPages;

            var groupsForPage = groups.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // items mostrados na página = todos os substitutos dos grupos selecionados
            var items = groupsForPage
                .SelectMany(g => g.OrderBy(a => a.AlimentoSubstitutoRef!.Name).ThenBy(a => a.AlimentoSubstitutoId))
                .ToList();

            // índices de grupos (alimentos originais) mostrados
            int skippedGroups = (pageNumber - 1) * pageSize;
            int startOriginalIndex = totalOriginals == 0 ? 0 : skippedGroups + 1;
            int endOriginalIndex = totalOriginals == 0 ? 0 : skippedGroups + groupsForPage.Count;

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalOriginals; // agora representa total de alimentos originais

            ViewBag.StartItem = startOriginalIndex; // start em termos de alimentos originais
            ViewBag.EndItem = endOriginalIndex;

            ViewBag.SearchOriginalName = originalName;
            ViewBag.SearchSubstitutoName = substitutoName;
            ViewBag.SearchFactorOp = factorOp;
            ViewBag.SearchFactorValue = factorValue;

            return View(items);
        }

        // GET: AlimentoSubstituto/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alimentoSubstituto = await _context.AlimentoSubstitutos
                .Include(a => a.AlimentoOriginal)
                .Include(a => a.AlimentoSubstitutoRef)
                .FirstOrDefaultAsync(m => m.AlimentoSubstitutoId == id);
            if (alimentoSubstituto == null)
            {
                return NotFound();
            }

            return View(alimentoSubstituto);
        }

        // GET: AlimentoSubstituto/Create
        public IActionResult Create()
        {
            ViewData["AlimentoOriginalId"] = new SelectList(_context.Alimentos, "AlimentoId", "Name");
            ViewData["AlimentoSubstitutoRefId"] = new SelectList(_context.Alimentos, "AlimentoId", "Name");
            return View();
        }

        // POST: AlimentoSubstituto/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AlimentoSubstitutoId,AlimentoOriginalId,AlimentoSubstitutoRefId,Motivo,ProporcaoEquivalente,Observacoes,FatorSimilaridade")] AlimentoSubstituto alimentoSubstituto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(alimentoSubstituto);
                await _context.SaveChangesAsync();
                TempData["AlertMessage"] = "Registro criado com sucesso.";
                TempData["AlertType"] = "success";

                return RedirectToAction(nameof(Details), new { id = alimentoSubstituto.AlimentoSubstitutoId });
            }
            ViewData["AlimentoOriginalId"] = new SelectList(_context.Alimentos, "AlimentoId", "Name", alimentoSubstituto.AlimentoOriginalId);
            ViewData["AlimentoSubstitutoRefId"] = new SelectList(_context.Alimentos, "AlimentoId", "Name", alimentoSubstituto.AlimentoSubstitutoRefId);
            return View(alimentoSubstituto);
        }

        // GET: AlimentoSubstituto/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alimentoSubstituto = await _context.AlimentoSubstitutos.FindAsync(id);
            if (alimentoSubstituto == null)
            {
                return NotFound();
            }
            ViewData["AlimentoOriginalId"] = new SelectList(_context.Alimentos, "AlimentoId", "Name", alimentoSubstituto.AlimentoOriginalId);
            ViewData["AlimentoSubstitutoRefId"] = new SelectList(_context.Alimentos, "AlimentoId", "Name", alimentoSubstituto.AlimentoSubstitutoRefId);
            return View(alimentoSubstituto);
        }

        // POST: AlimentoSubstituto/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AlimentoSubstitutoId,AlimentoOriginalId,AlimentoSubstitutoRefId,Motivo,ProporcaoEquivalente,Observacoes,FatorSimilaridade")] AlimentoSubstituto alimentoSubstituto)
        {
            if (id != alimentoSubstituto.AlimentoSubstitutoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(alimentoSubstituto);
                    await _context.SaveChangesAsync();
                    TempData["AlertMessage"] = "Registro atualizado com sucesso.";
                    TempData["AlertType"] = "success";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlimentoSubstitutoExists(alimentoSubstituto.AlimentoSubstitutoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Details), new { id = alimentoSubstituto.AlimentoSubstitutoId });
            }
            ViewData["AlimentoOriginalId"] = new SelectList(_context.Alimentos, "AlimentoId", "Name", alimentoSubstituto.AlimentoOriginalId);
            ViewData["AlimentoSubstitutoRefId"] = new SelectList(_context.Alimentos, "AlimentoId", "Name", alimentoSubstituto.AlimentoSubstitutoRefId);
            return View(alimentoSubstituto);
        }

        // GET: AlimentoSubstituto/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alimentoSubstituto = await _context.AlimentoSubstitutos
                .Include(a => a.AlimentoOriginal)
                .Include(a => a.AlimentoSubstitutoRef)
                .FirstOrDefaultAsync(m => m.AlimentoSubstitutoId == id);
            if (alimentoSubstituto == null)
            {
                return NotFound();
            }

            return View(alimentoSubstituto);
        }

        // POST: AlimentoSubstituto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var alimentoSubstituto = await _context.AlimentoSubstitutos.FindAsync(id);
            if (alimentoSubstituto != null)
            {
                _context.AlimentoSubstitutos.Remove(alimentoSubstituto);
            }

            await _context.SaveChangesAsync();
            TempData["AlertMessage"] = "Registro apagado com sucesso.";
            TempData["AlertType"] = "success";
            return RedirectToAction(nameof(Index));
        }

        private bool AlimentoSubstitutoExists(int id)
        {
            return _context.AlimentoSubstitutos.Any(e => e.AlimentoSubstitutoId == id);
        }
    }
}
