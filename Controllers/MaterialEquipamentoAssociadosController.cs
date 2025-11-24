using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellBeing.Models; // Ensure all necessary namespaces are present
using HealthWellbeing.ViewModels; // Assuming this contains PaginationInfo
using Microsoft.AspNetCore.Mvc.Rendering; // Needed for SelectList

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

        // Helper method to load status options for dropdowns (Select Lists)
        private void LoadEstadoMaterialOptions(object selectedId = null)
        {
            var estados = _context.EstadosMaterial.OrderBy(e => e.Nome).AsNoTracking().ToList();
            ViewData["MaterialStatusId"] = new SelectList(estados, "MaterialStatusId", "Nome", selectedId);
        }

        // GET: MaterialEquipamentoAssociados
        public async Task<IActionResult> Index(
            int page = 1,
            string searchNome = "",
            string searchEstado = "") // searchEstado is now the Status Name
        {
            ViewBag.SearchNome = searchNome;
            ViewBag.SearchEstado = searchEstado;

            // 1. Eager Load the Material Status (EstadoMaterial)
            var materialQuery = _context.MaterialEquipamentoAssociado
                .Include(m => m.EstadoMaterial) // MUST include to access the status name
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchNome))
            {
                materialQuery = materialQuery.Where(m => m.NomeEquipamento.Contains(searchNome));
            }

            // 2. Filter by Status Name (EstadoMaterial.Nome)
            if (!string.IsNullOrEmpty(searchEstado))
            {
                // We use the navigation property to filter by the related entity's name
                materialQuery = materialQuery.Where(m =>
                    m.EstadoMaterial != null &&
                    m.EstadoMaterial.Nome.Contains(searchEstado)
                );
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

            // Eager Load the Material Status
            var materialEquipamentoAssociado = await _context.MaterialEquipamentoAssociado
                .Include(m => m.EstadoMaterial) // Include the status for display
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
            LoadEstadoMaterialOptions(); // Load options for the dropdown
            return View();
        }

        // POST: MaterialEquipamentoAssociados/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        // BIND: Removed 'EstadoComponente' and added the Foreign Key 'MaterialStatusId'
        public async Task<IActionResult> Create([Bind("MaterialEquipamentoAssociadoId,NomeEquipamento,Quantidade,MaterialStatusId")] MaterialEquipamentoAssociado materialEquipamentoAssociado)
        {
            // Note: Since MaterialStatusId is a simple integer, ModelState.IsValid should be true 
            // if the user selected a valid ID (which comes from the ViewData SelectList).
            if (ModelState.IsValid)
            {
                _context.Add(materialEquipamentoAssociado);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"O material '{materialEquipamentoAssociado.NomeEquipamento}' foi criado com sucesso!";
                return RedirectToAction(nameof(Index));
            }

            // If model is invalid, reload the dropdown options before returning to the view
            LoadEstadoMaterialOptions(materialEquipamentoAssociado.MaterialStatusId);
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

            // Load the options for the dropdown, selecting the current status ID
            LoadEstadoMaterialOptions(materialEquipamentoAssociado.MaterialStatusId);
            return View(materialEquipamentoAssociado);
        }

        // POST: MaterialEquipamentoAssociados/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        // BIND: Removed 'EstadoComponente' and added the Foreign Key 'MaterialStatusId'
        public async Task<IActionResult> Edit(int id, [Bind("MaterialEquipamentoAssociadoId,NomeEquipamento,Quantidade,MaterialStatusId")] MaterialEquipamentoAssociado materialEquipamentoAssociado)
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

            // If model is invalid, reload the dropdown options before returning to the view
            LoadEstadoMaterialOptions(materialEquipamentoAssociado.MaterialStatusId);
            return View(materialEquipamentoAssociado);
        }

        // GET: MaterialEquipamentoAssociados/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Eager Load the Material Status for display on the Delete confirmation page
            var materialEquipamentoAssociado = await _context.MaterialEquipamentoAssociado
                .Include(m => m.EstadoMaterial)
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