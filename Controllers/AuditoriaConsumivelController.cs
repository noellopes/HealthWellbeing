using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;

namespace HealthWellbeing.Controllers
{
    public class AuditoriaConsumivelController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public AuditoriaConsumivelController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // INDEX COM PAGINAÇÃO + PESQUISA (igual ao Consumivel)
        public async Task<IActionResult> Index(string? searchConsumivel, int page = 1)
        {
            var query = _context.AuditoriaConsumivel
                .Include(a => a.Consumivel)
                .AsQueryable();

            // incluir Sala apenas se existir DbSet<Sala> no DbContext
            if (_context.GetType().GetProperty("Sala") != null)
                query = query.Include(a => a.Sala);

            if (!string.IsNullOrWhiteSpace(searchConsumivel))
                query = query.Where(a => a.Consumivel != null && a.Consumivel.Nome.Contains(searchConsumivel));

            int totalItems = await query.CountAsync();
            var model = new PaginationInfo<AuditoriaConsumivel>(page, totalItems);

            model.Items = await query
                .OrderByDescending(a => a.DataConsumo)
                .Skip(model.ItemsToSkip)
                .Take(model.ItemsPerPage)
                .ToListAsync();

            ViewBag.SearchConsumivel = searchConsumivel;

            return View(model);
        }

        // DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var auditoriaQuery = _context.AuditoriaConsumivel
                .Include(a => a.Consumivel)
                .AsQueryable();

            if (_context.GetType().GetProperty("Sala") != null)
                auditoriaQuery = auditoriaQuery.Include(a => a.Sala);

            var item = await auditoriaQuery.FirstOrDefaultAsync(a => a.AuditoriaConsumivelId == id);
            if (item == null)
                return NotFound();

            return View(item);
        }

        // CREATE GET
        public IActionResult Create()
        {
            // NOTE: usamos ViewBag com as chaves exatas que a tua view espera (SalaID, ConsumivelID)
            ViewBag.ConsumivelID = new SelectList(_context.Consumivel.OrderBy(c => c.Nome), "ConsumivelId", "Nome");

            if (_context.GetType().GetProperty("Sala") != null)
            {
                // Só cria o SelectList se existir DbSet<Sala> no DbContext
                ViewBag.SalaID = new SelectList(_context.Set<Sala>().OrderBy(s => s.TipoSala), "SalaId", "TipoSala");
            }
            else
            {
                ViewBag.SalaID = null;
            }

            return View();
        }

        // CREATE POST: desconta stock e valida
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AuditoriaConsumivelId,SalaId,ConsumivelID,QuantidadeUsada,DataConsumo")] AuditoriaConsumivel auditoria)
        {
            // Recarregar dropdowns caso seja necessário retornar a view com erros
            ViewBag.ConsumivelID = new SelectList(_context.Consumivel.OrderBy(c => c.Nome), "ConsumivelId", "Nome", auditoria.ConsumivelID);
            if (_context.GetType().GetProperty("Sala") != null)
                ViewBag.SalaID = new SelectList(_context.Set<Sala>().OrderBy(s => s.TipoSala), "SalaId", "TipoSala", auditoria.SalaId);

            // Validações básicas
            if (auditoria.ConsumivelID == null)
            {
                ModelState.AddModelError("ConsumivelID", "Escolha um consumível.");
            }

            if (auditoria.QuantidadeUsada <= 0)
            {
                ModelState.AddModelError("QuantidadeUsada", "A quantidade usada deve ser maior que 0.");
            }

            // Se já houver erros, devolve a view com os dropdowns carregados
            if (!ModelState.IsValid)
            {
                return View(auditoria);
            }

            // Obter consumível do DB
            var consumivel = await _context.Consumivel
                .FirstOrDefaultAsync(c => c.ConsumivelId == auditoria.ConsumivelID);

            if (consumivel == null)
            {
                ModelState.AddModelError("ConsumivelID", "Consumível selecionado não existe.");
                return View(auditoria);
            }

            // Verificar stock suficiente
            if (auditoria.QuantidadeUsada > consumivel.QuantidadeAtual)
            {
                ModelState.AddModelError("QuantidadeUsada", $"Quantidade insuficiente! Só existem {consumivel.QuantidadeAtual} unidades em stock.");
                return View(auditoria);
            }

            // Tudo OK -> descontar e guardar (numa única operação)
            consumivel.QuantidadeAtual -= auditoria.QuantidadeUsada;
            _context.Update(consumivel);

            // Se DataConsumo não tiver valor (0) assume agora
            if (auditoria.DataConsumo == default)
                auditoria.DataConsumo = DateTime.Now;

            _context.Add(auditoria);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Auditoria registada e stock atualizado!";
            return RedirectToAction(nameof(Index));
        }

        // EDIT GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var auditoria = await _context.AuditoriaConsumivel.FindAsync(id);
            if (auditoria == null) return NotFound();

            ViewBag.ConsumivelID = new SelectList(_context.Consumivel.OrderBy(c => c.Nome), "ConsumivelId", "Nome", auditoria.ConsumivelID);
            if (_context.GetType().GetProperty("Sala") != null)
                ViewBag.SalaID = new SelectList(_context.Set<Sala>().OrderBy(s => s.TipoSala), "SalaId", "TipoSala", auditoria.SalaId);

            return View(auditoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AuditoriaConsumivel auditoria)
        {
            if (id != auditoria.AuditoriaConsumivelId)
                return NotFound();

            if (!ModelState.IsValid)
                return View(auditoria);

            // Buscar auditoria original
            var original = await _context.AuditoriaConsumivel
                .Include(a => a.Consumivel)
                .FirstOrDefaultAsync(a => a.AuditoriaConsumivelId == id);

            if (original == null)
                return NotFound();

            // Buscar consumível associado
            var consumivel = await _context.Consumivel.FindAsync(auditoria.ConsumivelID);

            if (consumivel == null)
            {
                ModelState.AddModelError("ConsumivelID", "Consumível inválido.");
                return View(auditoria);
            }

            // Calcular diferença
            int quantidadeOriginal = original.QuantidadeUsada;
            int quantidadeNova = auditoria.QuantidadeUsada;
            int diferenca = quantidadeNova - quantidadeOriginal;

            // Se a diferença for positiva → retirar stock adicional
            if (diferenca > 0)
            {
                if (consumivel.QuantidadeAtual < diferenca)
                {
                    ModelState.AddModelError("QuantidadeUsada",
                        $"Quantidade insuficiente! Só existem {consumivel.QuantidadeAtual} unidades em stock.");
                    return View(auditoria);
                }

                consumivel.QuantidadeAtual -= diferenca;
            }
            // Se for negativa → devolver stock
            else if (diferenca < 0)
            {
                consumivel.QuantidadeAtual += Math.Abs(diferenca);
            }

            // Atualizar auditoria
            original.ConsumivelID = auditoria.ConsumivelID;
            original.SalaId = auditoria.SalaId;
            original.QuantidadeUsada = auditoria.QuantidadeUsada;
            original.DataConsumo = auditoria.DataConsumo;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Auditoria atualizada com sucesso!";

            return RedirectToAction(nameof(Index));
        }

        // DELETE GET
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var auditoriaQuery = _context.AuditoriaConsumivel.Include(a => a.Consumivel).AsQueryable();
            if (_context.GetType().GetProperty("Sala") != null)
                auditoriaQuery = auditoriaQuery.Include(a => a.Sala);

            var item = await auditoriaQuery.FirstOrDefaultAsync(a => a.AuditoriaConsumivelId == id);
            if (item == null) return NotFound();

            return View(item);
        }

        // DELETE POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var auditoria = await _context.AuditoriaConsumivel.FindAsync(id);
            if (auditoria != null)
            {
                _context.AuditoriaConsumivel.Remove(auditoria);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Auditoria eliminada com sucesso!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
