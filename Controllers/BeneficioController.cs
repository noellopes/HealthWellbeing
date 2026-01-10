using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellbeing.Controllers
{
    [Authorize(Roles = SeedData.Roles.Administrador + "," + SeedData.Roles.Profissional)]
    public class BeneficioController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public BeneficioController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Beneficio
        public async Task<IActionResult> Index(string searchNome, string searchDescricao, int page = 1)
        {
            var query = _context.Beneficio.AsQueryable();

            if (!string.IsNullOrEmpty(searchNome))
                query = query.Where(b => b.NomeBeneficio.Contains(searchNome));

            if (!string.IsNullOrEmpty(searchDescricao))
                query.Where(b => b.DescricaoBeneficio.Contains(searchDescricao));

            int totalItems = await query.CountAsync();
            var pagination = new PaginationInfo<Beneficio>(page, totalItems, 5);

            var items = await query
                .OrderBy(b => b.NomeBeneficio)
                .Skip(pagination.ItemsToSkip)
                .Take(pagination.ItemsPerPage)
                .ToListAsync();

            pagination.Items = items;
            ViewBag.SearchNome = searchNome;
            ViewBag.SearchDescricao = searchDescricao;

            return View(pagination);
        }

        // GET: Beneficio/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var beneficio = await _context.Beneficio.FirstOrDefaultAsync(m => m.BeneficioId == id);
            if (beneficio == null) return NotFound();
            return View(beneficio);
        }

        // GET: Beneficio/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Beneficio/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BeneficioId,NomeBeneficio,DescricaoBeneficio")] Beneficio beneficio)
        {
            var existingBeneficio = await _context.Beneficio
                .FirstOrDefaultAsync(b => b.NomeBeneficio.ToLower() == beneficio.NomeBeneficio.ToLower());

            if (existingBeneficio != null)
            {
                TempData["StatusMessage"] = $"Erro: O Benefício '{beneficio.NomeBeneficio}' já existe.";
                return View(beneficio);
            }

            if (ModelState.IsValid)
            {
                _context.Add(beneficio);
                await _context.SaveChangesAsync();
                TempData["StatusMessage"] = $"Sucesso: Benefício '{beneficio.NomeBeneficio}' criado.";
                return RedirectToAction(nameof(Index));
            }
            return View(beneficio);
        }

        // GET: Beneficio/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var beneficio = await _context.Beneficio.FindAsync(id);
            if (beneficio == null) return NotFound();
            return View(beneficio);
        }

        // POST: Beneficio/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BeneficioId,NomeBeneficio,DescricaoBeneficio")] Beneficio beneficio)
        {
            if (id != beneficio.BeneficioId) return NotFound();

            var existingBeneficioWithSameName = await _context.Beneficio
                .FirstOrDefaultAsync(b => b.NomeBeneficio.ToLower() == beneficio.NomeBeneficio.ToLower() && b.BeneficioId != id);

            if (existingBeneficioWithSameName != null)
            {
                ViewData["StatusMessage"] = $"Erro: Já existe um benefício com o nome '{beneficio.NomeBeneficio}'.";
                return View(beneficio);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(beneficio);
                    await _context.SaveChangesAsync();
                    TempData["StatusMessage"] = "Sucesso: Benefício atualizado.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BeneficioExists(beneficio.BeneficioId)) return View("InvalidBeneficio", beneficio);
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(beneficio);
        }

        // GET: Beneficio/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var beneficio = await _context.Beneficio.FirstOrDefaultAsync(m => m.BeneficioId == id);
            if (beneficio == null) return NotFound();

            // Contar quantas vezes é usado
            int numAssociacoes = await _context.TipoExercicioBeneficio.CountAsync(tb => tb.BeneficioId == id);

            if (numAssociacoes > 0)
            {
                ViewBag.PodeEliminar = false;
                ViewBag.MensagemErro = $"Não é possível eliminar o benefício '{beneficio.NomeBeneficio}' porque ele está associado a {numAssociacoes} Tipo(s) de Exercício.";
            }
            else
            {
                ViewBag.PodeEliminar = true;
            }

            return View(beneficio);
        }

        // POST: Beneficio/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var beneficio = await _context.Beneficio.FindAsync(id);
            if (beneficio == null) return RedirectToAction(nameof(Index));

            try
            {
                _context.Beneficio.Remove(beneficio);
                await _context.SaveChangesAsync();
                TempData["StatusMessage"] = "Sucesso: Benefício eliminado.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BeneficioExists(id)) return RedirectToAction(nameof(Index));
                else throw;
            }
            catch (DbUpdateException)
            {
                TempData["StatusMessage"] = "Erro: Não é possível eliminar devido a dependências.";
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Invalido(Beneficio beneficio)
        {
            return View("InvalidBeneficio", beneficio);
        }

        private bool BeneficioExists(int id)
        {
            return _context.Beneficio.Any(e => e.BeneficioId == id);
        }
    }
}