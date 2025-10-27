using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class TipoTreinosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TipoTreinosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TipoTreinos
        public async Task<IActionResult> Index()
        {
            var tiposTreino = await _context.TipoTreinos.ToListAsync();
            return View(tiposTreino);
        }

        // GET: TipoTreinos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var tipoTreino = await _context.TipoTreinos
                .FirstOrDefaultAsync(m => m.TipoTreinoId == id);

            if (tipoTreino == null)
                return NotFound();

            return View(tipoTreino);
        }

        // GET: TipoTreinos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoTreinos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TipoTreinoId,Nome,Descricao,DuracaoMinutos,Intensidade,Ativo")] TipoTreino tipoTreino)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipoTreino);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipoTreino);
        }

        // GET: TipoTreinos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var tipoTreino = await _context.TipoTreinos.FindAsync(id);
            if (tipoTreino == null)
                return NotFound();

            return View(tipoTreino);
        }

        // POST: TipoTreinos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TipoTreinoId,Nome,Descricao,DuracaoMinutos,Intensidade,Ativo")] TipoTreino tipoTreino)
        {
            if (id != tipoTreino.TipoTreinoId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipoTreino);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoTreinoExists(tipoTreino.TipoTreinoId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tipoTreino);
        }

        // GET: TipoTreinos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var tipoTreino = await _context.TipoTreinos
                .FirstOrDefaultAsync(m => m.TipoTreinoId == id);

            if (tipoTreino == null)
                return NotFound();

            return View(tipoTreino);
        }

        // POST: TipoTreinos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipoTreino = await _context.TipoTreinos.FindAsync(id);
            if (tipoTreino != null)
            {
                _context.TipoTreinos.Remove(tipoTreino);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TipoTreinoExists(int id)
        {
            return _context.TipoTreinos.Any(e => e.TipoTreinoId == id);
        }
    }
}
