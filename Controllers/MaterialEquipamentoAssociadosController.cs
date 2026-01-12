using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels; // Necessário para PaginationInfo
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq; // Necessário para .Where e .OrderBy
using System.Threading.Tasks;

namespace HealthWellbeing.Controllers
{
    [Authorize(Roles = "Admin, Medico, Supervisor Tecnico")]
    public class MaterialEquipamentoAssociadoController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public MaterialEquipamentoAssociadoController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: MaterialEquipamentoAssociado
        // Adicionados parâmetros para paginação e pesquisa
        public async Task<IActionResult> Index(int page = 1, string searchNome = "", string searchEstado = "")
        {
            // 1. Guardar os termos de pesquisa na ViewBag para a View não os perder
            ViewBag.SearchNome = searchNome;
            ViewBag.SearchEstado = searchEstado;

            // 2. Preparar a Query
            var query = _context.MaterialEquipamentoAssociado.AsQueryable();

            // 3. Aplicar Filtros (Pesquisa)
            if (!string.IsNullOrEmpty(searchNome))
            {
                query = query.Where(m => m.NomeEquipamento.Contains(searchNome));
            }
            // Nota: O filtro de estado foi removido da BD, mas mantemos o parametro para não quebrar a View se ela o enviar

            // 4. Calcular Totais para a Paginação
            int pageSize = 10; // Define quantos itens queres por página
            int totalItems = await query.CountAsync();

            // 5. Criar o objeto que a View está à espera (PaginationInfo)
            var paginationInfo = new PaginationInfo<MaterialEquipamentoAssociado>(page, totalItems, pageSize);

            // 6. Carregar apenas os itens da página atual
            paginationInfo.Items = await query
                .OrderBy(m => m.NomeEquipamento)
                .Skip(paginationInfo.ItemsToSkip)
                .Take(paginationInfo.ItemsPerPage)
                .ToListAsync();

            // 7. Retornar o PaginationInfo (Isto resolve o erro de InvalidOperationException)
            return View(paginationInfo);
        }

        // GET: MaterialEquipamentoAssociado/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var material = await _context.MaterialEquipamentoAssociado
                .Include(m => m.ExameTipoRecursos)
                    .ThenInclude(etr => etr.ExameTipo)
                .FirstOrDefaultAsync(m => m.MaterialEquipamentoAssociadoId == id);

            if (material == null) return NotFound();

            return View(material);
        }

        // GET: MaterialEquipamentoAssociado/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MaterialEquipamentoAssociado material)
        {
            if (ModelState.IsValid)
            {
                _context.Add(material);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(material);
        }

        // GET: MaterialEquipamentoAssociado/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var material = await _context.MaterialEquipamentoAssociado.FindAsync(id);
            if (material == null) return NotFound();

            return View(material);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MaterialEquipamentoAssociado material)
        {
            if (id != material.MaterialEquipamentoAssociadoId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(material);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaterialExists(material.MaterialEquipamentoAssociadoId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(material);
        }

        // GET: MaterialEquipamentoAssociado/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var material = await _context.MaterialEquipamentoAssociado
                .FirstOrDefaultAsync(m => m.MaterialEquipamentoAssociadoId == id);

            if (material == null) return NotFound();

            return View(material);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var material = await _context.MaterialEquipamentoAssociado.FindAsync(id);

            // Verifica se está a ser usado nalgum protocolo
            bool estaEmUsoNoProtocolo = await _context.ExameTipoRecursos
                .AnyAsync(etr => etr.MaterialEquipamentoAssociadoId == id);

            if (estaEmUsoNoProtocolo)
            {
                TempData["ErrorMessage"] = "Não pode apagar este material porque ele faz parte da receita padrão de um Tipo de Exame.";
                return RedirectToAction(nameof(Index));
            }

            if (material != null)
            {
                _context.MaterialEquipamentoAssociado.Remove(material);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Material removido do dicionário.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool MaterialExists(int id)
        {
            return _context.MaterialEquipamentoAssociado.Any(e => e.MaterialEquipamentoAssociadoId == id);
        }
    }
}