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
        // ITEMS_PER_PAGE não é mais necessário para a listagem simples
        // private const int ITEMS_PER_PAGE = 5; 

        public MaterialEquipamentoAssociadosController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: MaterialEquipamentoAssociados
        // Revertido para listagem simples sem Paginação/Filtros
        public async Task<IActionResult> Index()
        {

            return View(await _context.MaterialEquipamentoAssociado.ToListAsync());

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
                TempData["SuccessMessage"] = $"O material '{materialEquipamentoAssociado.NomeEquipamento}' foi adicionado com sucesso!";
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