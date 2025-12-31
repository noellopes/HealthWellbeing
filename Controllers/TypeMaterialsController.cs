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

namespace HealthWellbeingRoom.Controllers
{
    public class TypeMaterialsController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public TypeMaterialsController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: TypeMaterials
        [Authorize(Roles = "logisticsTechnician,Administrator")]
        public async Task<IActionResult> Index(
            int page = 1,
            string searchName = "",
            string searchDescription = "")
        {
            int itemsPerPage = 7;

            // Query base
            var query = _context.TypeMaterial.AsQueryable();

            // FILTROS
            if (!string.IsNullOrEmpty(searchName))
                query = query.Where(t => t.Name.Contains(searchName));

            if (!string.IsNullOrEmpty(searchDescription))
                query = query.Where(t => t.Description.Contains(searchDescription));

            // Guardar valores dos filtros para a View
            ViewBag.SearchName = searchName;
            ViewBag.SearchDescription = searchDescription;

            // PAGINAÇÃO
            int totalItems = await query.CountAsync();
            var pagination = new RPaginationInfo<TypeMaterial>(page, totalItems, itemsPerPage);

            pagination.Items = await query
                .OrderBy(t => t.Name)
                .Skip(pagination.ItemsToSkip)
                .Take(itemsPerPage)
                .ToListAsync();

            return View(pagination);
        }

        // GET: TypeMaterials/Details/5
        [Authorize(Roles = "logisticsTechnician,Administrator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var typeMaterial = await _context.TypeMaterial
                .FirstOrDefaultAsync(m => m.TypeMaterialID == id);

            if (typeMaterial == null) return NotFound();

            return View(typeMaterial);
        }

        // GET: TypeMaterials/Create
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: TypeMaterials/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([Bind("TypeMaterialID,Name,Description")] TypeMaterial typeMaterial)
        {
            // Normalizar nome (tirar espaços e ignorar maiúsculas/minúsculas)
            string normalizedName = typeMaterial.Name.Trim().ToLower();

            // Verificar duplicados
            bool exists = await _context.TypeMaterial
                .AnyAsync(t => t.Name.Trim().ToLower() == normalizedName);

            if (exists)
            {
                ModelState.AddModelError("Name", "Já existe um tipo com este nome. Escolha outro.");
                return View(typeMaterial);
            }

            if (!ModelState.IsValid)
                return View(typeMaterial);

            _context.Add(typeMaterial);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Tipo de material criado com sucesso!";
            return RedirectToAction(nameof(Details), new { id = typeMaterial.TypeMaterialID });
        }



        // GET: TypeMaterials/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var typeMaterial = await _context.TypeMaterial.FindAsync(id);
            if (typeMaterial == null) return NotFound();

            return View(typeMaterial);
        }

        // POST: TypeMaterials/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id, [Bind("TypeMaterialID,Name,Description")] TypeMaterial typeMaterial)
        {
            if (id != typeMaterial.TypeMaterialID) return NotFound();

            // Normalizar nome
            string normalizedName = typeMaterial.Name.Trim().ToLower();

            // Verificar duplicados (ignorando o próprio registro)
            bool exists = await _context.TypeMaterial
                .AnyAsync(t => t.Name.Trim().ToLower() == normalizedName && t.TypeMaterialID != id);

            if (exists)
            {
                ModelState.AddModelError("Name", "Já existe um tipo com este nome. Escolha outro.");
                return View(typeMaterial);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(typeMaterial);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Tipo de material atualizado com sucesso!";
                    return RedirectToAction(nameof(Details), new { id = typeMaterial.TypeMaterialID });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.TypeMaterial.Any(e => e.TypeMaterialID == typeMaterial.TypeMaterialID))
                        return NotFound();
                    else
                        throw;
                }
            }

            return View(typeMaterial);
        }


        // GET: TypeMaterials/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var typeMaterial = await _context.TypeMaterial
                .FirstOrDefaultAsync(m => m.TypeMaterialID == id);

            if (typeMaterial == null) return NotFound();

            return View(typeMaterial);
        }

        // POST: TypeMaterials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var typeMaterial = await _context.TypeMaterial.FindAsync(id);

            if (typeMaterial == null)
                return NotFound();

            // Verificar se este tipo está a ser usado por algum dispositivo médico
            bool isInUse = await _context.MedicalDevices
                .AnyAsync(d => d.TypeMaterialID == id);

            if (isInUse)
            {
                TempData["ErrorMessage"] = "Não é possível eliminar este tipo, pois existem dispositivos médicos associados.";
                return RedirectToAction(nameof(Index));
            }

            // Se não estiver em uso  pode eliminar
            _context.TypeMaterial.Remove(typeMaterial);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Tipo de dispositivo médico eliminado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

    }
}
