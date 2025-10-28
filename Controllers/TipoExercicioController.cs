using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;

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
        public async Task<IActionResult> Index()
        {
            return View(await _context.TipoExercicio.Include(t => t.Beneficios).ToListAsync());
        }

        // GET: TipoExercicio/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var tipoExercicio = await _context.TipoExercicio
                .Include(t => t.Beneficios)
                .FirstOrDefaultAsync(m => m.TipoExercicioId == id);

            if (tipoExercicio == null) return NotFound();

            return View(tipoExercicio);
        }

        // GET: TipoExercicio/Create
        public IActionResult Create()
        {
            ViewData["Beneficios"] = _context.Beneficios.ToList();
            return View();
        }

        // POST: TipoExercicio/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TipoExercicioId,NomeTipoExercicios,DescricaoTipoExercicios,NivelDificuldadeTipoExercicios")] TipoExercicio tipoExercicio, int[] beneficiosSelecionados)
        {
            if (ModelState.IsValid)
            {
                if (beneficiosSelecionados != null)
                {
                    tipoExercicio.Beneficios = new List<Beneficio>();
                    foreach (var id in beneficiosSelecionados)
                    {
                        var beneficio = await _context.Beneficios.FindAsync(id);
                        if (beneficio != null)
                            tipoExercicio.Beneficios.Add(beneficio);
                    }
                }

                _context.Add(tipoExercicio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Beneficios"] = _context.Beneficios.ToList();
            return View(tipoExercicio);
        }

        // GET: TipoExercicio/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var tipoExercicio = await _context.TipoExercicio
                .Include(t => t.Beneficios)
                .FirstOrDefaultAsync(t => t.TipoExercicioId == id);

            if (tipoExercicio == null) return NotFound();

            ViewData["Beneficios"] = _context.Beneficios.ToList();
            return View(tipoExercicio);
        }

        // POST: TipoExercicio/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TipoExercicioId,NomeTipoExercicios,DescricaoTipoExercicios,NivelDificuldadeTipoExercicios")] TipoExercicio tipoExercicio, int[] beneficiosSelecionados)
        {
            if (id != tipoExercicio.TipoExercicioId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Atualiza propriedades básicas
                    _context.Update(tipoExercicio);
                    await _context.SaveChangesAsync();

                    // Atualiza benefícios
                    var tipoExercicioExistente = await _context.TipoExercicio
                        .Include(t => t.Beneficios)
                        .FirstOrDefaultAsync(t => t.TipoExercicioId == id);

                    tipoExercicioExistente.Beneficios.Clear();

                    if (beneficiosSelecionados != null)
                    {
                        foreach (var beneficioId in beneficiosSelecionados)
                        {
                            var beneficio = await _context.Beneficios.FindAsync(beneficioId);
                            if (beneficio != null)
                                tipoExercicioExistente.Beneficios.Add(beneficio);
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoExercicioExists(tipoExercicio.TipoExercicioId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["Beneficios"] = _context.Beneficios.ToList();
            return View(tipoExercicio);
        }

        // GET: TipoExercicio/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var tipoExercicio = await _context.TipoExercicio
                .Include(t => t.Beneficios)
                .FirstOrDefaultAsync(m => m.TipoExercicioId == id);

            if (tipoExercicio == null) return NotFound();

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
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TipoExercicioExists(int id)
        {
            return _context.TipoExercicio.Any(e => e.TipoExercicioId == id);
        }



        public IActionResult CreateBeneficio()
        {
            return View("CreateBeneficio"); // View na pasta Views/TipoExercicio/CreateBeneficio.cshtml
        }

        // POST: TipoExercicio/CreateBeneficio
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBeneficio([Bind("Nome,Descricao")] Beneficio beneficio)
        {
            if (ModelState.IsValid)
            {
                _context.Beneficios.Add(beneficio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Create)); // redireciona para criar TipoExercicio
            }
            return View("CreateBeneficio", beneficio);
        }


        // GET: TipoExercicio/ListaBeneficios
        public IActionResult ListaBeneficios()
        {
            var beneficios = _context.Beneficios.ToList();
            return View(beneficios); // Vai procurar a view ListaBeneficios.cshtml
        }


    }
}
