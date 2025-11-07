using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;

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
        public async Task<IActionResult> Index()
        {
            return View(await _context.TypeMaterial.ToListAsync());
        }

        // GET: TypeMaterials/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var typeMaterial = await _context.TypeMaterial
                .FirstOrDefaultAsync(m => m.TypeMaterialID == id);
            if (typeMaterial == null)
            {
                return NotFound();
            }

            return View(typeMaterial);
        }

        // GET: TypeMaterials/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TypeMaterials/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TypeMaterialID,Name,Description")] TypeMaterial typeMaterial)
        {
            if (ModelState.IsValid)
            {
                _context.Add(typeMaterial);
                await _context.SaveChangesAsync();

                //Mensagem de sucesso
                TempData["SuccessMessage"] = "Tipo de material criado com sucesso!";

                //Volta para o Index (lista)
                return RedirectToAction(nameof(Index));
            }

            return View(typeMaterial);
        }

        // GET: TypeMaterials/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var typeMaterial = await _context.TypeMaterial.FindAsync(id);
            if (typeMaterial == null)
            {
                return NotFound();
            }
            return View(typeMaterial);
        }

        // POST: TypeMaterials/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TypeMaterialID,Name,Description")] TypeMaterial typeMaterial)
        {
            if (id != typeMaterial.TypeMaterialID)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(typeMaterial);
                    await _context.SaveChangesAsync();

                    //  Mensagem de sucesso
                    TempData["SuccessMessage"] = "Tipo de material atualizado com sucesso!";

                    //  Redireciona para os detalhes do mesmo registo
                    return RedirectToAction(nameof(Details), new { id = typeMaterial.TypeMaterialID });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TypeMaterialExists(typeMaterial.TypeMaterialID))
                        return NotFound();
                    else
                        throw;
                }
            }

            return View(typeMaterial);
        }


        // GET: TypeMaterials/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var typeMaterial = await _context.TypeMaterial
                .FirstOrDefaultAsync(m => m.TypeMaterialID == id);
            if (typeMaterial == null)
            {
                return NotFound();
            }

            return View(typeMaterial);
        }

        // POST: TypeMaterials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var typeMaterial = await _context.TypeMaterial.FindAsync(id);
            if (typeMaterial != null)
            {
                _context.TypeMaterial.Remove(typeMaterial);
                await _context.SaveChangesAsync();
            }

            // Mensagem de sucesso (igual ao padrão do prof)
            TempData["SuccessMessage"] = "Tipo de material eliminado com sucesso!";

            return RedirectToAction(nameof(Index));
        }

        private bool TypeMaterialExists(int id)
        {
            return _context.TypeMaterial.Any(e => e.TypeMaterialID == id);
        }
    }
}
