using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;

namespace HealthWellBeing.Controllers
{
    public class MaterialEquipamentoAssociadosController : Controller
    {
        private readonly HealthWellbeingDbContext _context;
        private const int ITEMS_PER_PAGE = 5; // Configuração do tamanho da página

        public MaterialEquipamentoAssociadosController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: MaterialEquipamentoAssociados
        public async Task<IActionResult> Index(
            int page = 1,
            string searchNome = "",
            string searchEstado = "")
        {
            ViewBag.SearchNome = searchNome;
            ViewBag.SearchEstado = searchEstado;

            var materialQuery = _context.MaterialEquipamentoAssociado.AsQueryable();

            if (!string.IsNullOrEmpty(searchNome))
            {
                materialQuery = materialQuery.Where(m => m.NomeEquipamento.Contains(searchNome));
            }

            if (!string.IsNullOrEmpty(searchEstado))
            {
                materialQuery = materialQuery.Where(m => m.EstadoComponente.Contains(searchEstado));
            }

            int totalMateriais = await materialQuery.CountAsync();

            var materialInfo = new PaginationInfo<MaterialEquipamentoAssociado>(page, totalMateriais, itemsPerPage: ITEMS_PER_PAGE);

            materialInfo.Items = await materialQuery
                .OrderBy(m => m.NomeEquipamento)
                .Skip(materialInfo.ItemsToSkip) // Pula os itens das páginas anteriores
                .Take(materialInfo.ItemsPerPage) // Pega apenas os 5 itens da página atual
                .ToListAsync();

            return View(materialInfo);
        }

        // GET: MaterialEquipamentoAssociados/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var materialEquipamentoAssociado = await _context.MaterialEquipamentoAssociado
                .FirstOrDefaultAsync(m => m.MaterialEquipamentoAssociadoId == id);
            if (materialEquipamentoAssociado == null)
            {
                return NotFound();
            }

            return View(materialEquipamentoAssociado);
        }

        // GET: MaterialEquipamentoAssociados/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MaterialEquipamentoAssociados/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaterialEquipamentoAssociadoId,NomeEquipamento,Quantidade,EstadoComponente")] MaterialEquipamentoAssociado materialEquipamentoAssociado)
        {
            if (ModelState.IsValid)
            {
                _context.Add(materialEquipamentoAssociado);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(materialEquipamentoAssociado);
        }

        // GET: MaterialEquipamentoAssociados/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var materialEquipamentoAssociado = await _context.MaterialEquipamentoAssociado.FindAsync(id);
            if (materialEquipamentoAssociado == null)
            {
                return NotFound();
            }
            return View(materialEquipamentoAssociado);
        }

        // POST: MaterialEquipamentoAssociados/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaterialEquipamentoAssociadoId,NomeEquipamento,Quantidade,EstadoComponente")] MaterialEquipamentoAssociado materialEquipamentoAssociado)
        {
            if (id != materialEquipamentoAssociado.MaterialEquipamentoAssociadoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(materialEquipamentoAssociado);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"O material '{materialEquipamentoAssociado.NomeEquipamento}' foi atualizado com sucesso!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaterialEquipamentoAssociadoExists(materialEquipamentoAssociado.MaterialEquipamentoAssociadoId))
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
            return View(materialEquipamentoAssociado);
        }

        // GET: MaterialEquipamentoAssociados/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var materialEquipamentoAssociado = await _context.MaterialEquipamentoAssociado
                .FirstOrDefaultAsync(m => m.MaterialEquipamentoAssociadoId == id);
            if (materialEquipamentoAssociado == null)
            {
                return NotFound();
            }

            return View(materialEquipamentoAssociado);
        }

        // POST: MaterialEquipamentoAssociados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var materialEquipamentoAssociado = await _context.MaterialEquipamentoAssociado.FindAsync(id);
            if (materialEquipamentoAssociado != null)
            {
                _context.MaterialEquipamentoAssociado.Remove(materialEquipamentoAssociado);
                TempData["SuccessMessage"] = $"O material '{materialEquipamentoAssociado.NomeEquipamento}' foi apagado com sucesso!";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MaterialEquipamentoAssociadoExists(int id)
        {
            return _context.MaterialEquipamentoAssociado.Any(e => e.MaterialEquipamentoAssociadoId == id);
        }
    }
}