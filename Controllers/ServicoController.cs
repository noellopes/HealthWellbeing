using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Controllers
{
    public class ServicoController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ServicoController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Servico
        public async Task<IActionResult> Index()
        {
            var servicos = await _context.Servicos
                .Include(s => s.TipoServico)
                .ToListAsync();

            return View(servicos);
        }

        // GET: Servico/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var servico = await _context.Servicos
                .Include(s => s.TipoServico)
                .FirstOrDefaultAsync(m => m.ServicoId == id);

            if (servico == null)
            {
                // Certifique-se que esta View existe ou use NotFound()
                return View("InvalidServico");
            }

            return View(servico);
        }
        // GET: Servico/Create
        public IActionResult Create()
        {
            // Alterado para TipoServicoId (singular) para bater com a View
            ViewBag.TipoServicosId = new SelectList(_context.TipoServicos, "TipoServicosId", "Nome");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
     [Bind("ServicoId,Nome,Descricao,Preco,DuracaoMinutos,TipoServicosId")] Servico servico)
        {
            ModelState.Remove("TipoServico");

            if (ModelState.IsValid)
            {
                _context.Add(servico);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Serviço criado com sucesso!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.TipoServicosId = new SelectList(
                _context.TipoServicos,
                "TipoServicosId",
                "Nome",
                servico.TipoServicosId
            );

            return View(servico);
        }

        // GET: Servico/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var servico = await _context.Servicos.FindAsync(id);
            if (servico == null) return NotFound();

            ViewData["TipoServicoId"] = new SelectList(_context.TipoServicos, "TipoServicoId", "Nome", servico.TipoServicosId);
            return View(servico);
        }

        // POST: Servico/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ServicoId,Nome,Descricao,Preco,DuracaoMinutos,TipoServicoId")] Servico servico)
        {
            if (id != servico.ServicoId) return NotFound();

            ModelState.Remove("TipoServicos");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(servico);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServicoExists(servico.ServicoId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Details), new { id = servico.ServicoId, successMessage = "Serviço criado com sucesso!" });
            }

            ViewData["TipoServicosId"] = new SelectList(_context.TipoServicos, "TipoServicosId", "Nome", servico.TipoServicosId);
            return View(servico);
        }

        // GET: Servico/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var servico = await _context.Servicos
                .Include(s => s.TipoServico)
                .FirstOrDefaultAsync(m => m.ServicoId == id);

            if (servico == null) return NotFound();

            return View(servico);
        }

        // POST: Servico/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var servico = await _context.Servicos.FindAsync(id);
            if (servico != null)
            {
                _context.Servicos.Remove(servico);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ServicoExists(int id)
        {
            return _context.Servicos.Any(e => e.ServicoId == id);
        }
    }
}