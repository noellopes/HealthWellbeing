using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellbeing.Controllers
{
    public class EquipamentoController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public EquipamentoController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Equipamento
        // CORREÇÃO: Adicionado parâmetro searchNomeEquipamento para a pesquisa funcionar
        public async Task<IActionResult> Index(string searchNomeEquipamento, int page = 1)
        {
            // Consulta base
            var equipamentosQuery = _context.Equipamento.AsQueryable();

            // CORREÇÃO: Lógica de Filtro (Pesquisa)
            if (!string.IsNullOrEmpty(searchNomeEquipamento))
            {
                equipamentosQuery = equipamentosQuery
                    .Where(e => e.NomeEquipamento.Contains(searchNomeEquipamento));

                // Manter o texto na caixa de pesquisa
                ViewBag.SearchNomeEquipamento = searchNomeEquipamento;
            }

            // Contar total de itens (já filtrados)
            int totalEquipamentos = await equipamentosQuery.CountAsync();

            // Criar objeto de paginação
            var equipamentosInfo = new PaginationInfo<Equipamento>(page, totalEquipamentos);

            // Buscar os itens da página atual
            equipamentosInfo.Items = await equipamentosQuery
                .OrderBy(e => e.NomeEquipamento)
                .Skip(equipamentosInfo.ItemsToSkip)
                .Take(equipamentosInfo.ItemsPerPage)
                .ToListAsync();

            return View(equipamentosInfo);
        }

        // GET: Equipamento/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipamento = await _context.Equipamento
                .FirstOrDefaultAsync(m => m.EquipamentoId == id);

            if (equipamento == null)
            {
                return NotFound();
            }

            return View(equipamento);
        }

        // GET: Equipamento/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Equipamento/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EquipamentoId,NomeEquipamento")] Equipamento equipamento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(equipamento);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Details),
                    new
                    {
                        id = equipamento.EquipamentoId,
                        SuccessMessage = "Equipamento criado com sucesso"
                    }
                );
            }
            return View(equipamento);
        }

        // GET: Equipamento/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipamento = await _context.Equipamento.FindAsync(id);
            if (equipamento == null)
            {
                return NotFound();
            }
            return View(equipamento);
        }

        // POST: Equipamento/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EquipamentoId,NomeEquipamento")] Equipamento equipamento)
        {
            if (id != equipamento.EquipamentoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var equipamentoExistente = await _context.Equipamento.FindAsync(id);

                    if (equipamentoExistente == null)
                    {
                        return View("InvalidEquipamento", equipamento);
                    }

                    _context.Entry(equipamentoExistente).CurrentValues.SetValues(equipamento);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Details),
                        new
                        {
                            id = equipamento.EquipamentoId,
                            SuccessMessage = "Equipamento editado com sucesso"
                        });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EquipamentoExists(equipamento.EquipamentoId))
                    {
                        return View("EquipamentoDeleted", equipamento);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(equipamento);
        }

        // GET: Equipamento/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // 1. Carregar o equipamento INCLUINDO a lista de exercícios
            // O nome "ExercicioEquipamentos" bate certo com o seu Model
            var equipamento = await _context.Equipamento
                .Include(e => e.ExercicioEquipamentos)
                .FirstOrDefaultAsync(m => m.EquipamentoId == id);

            if (equipamento == null)
            {
                TempData["SuccessMessage"] = "Este equipamento já foi eliminado.";
                return RedirectToAction(nameof(Index));
            }

            // 2. Verificar quantos exercícios usam este equipamento
            // CORREÇÃO: Uso de '?' para segurança caso a lista seja nula
            int numExercicios = equipamento.ExercicioEquipamentos?.Count ?? 0;

            // 3. Passar essa informação para a View
            ViewBag.NumExercicios = numExercicios;
            ViewBag.PodeEliminar = numExercicios == 0;

            return View(equipamento);
        }

        // POST: Equipamento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var equipamento = await _context.Equipamento
                .Include(e => e.ExercicioEquipamentos)
                .FirstOrDefaultAsync(m => m.EquipamentoId == id);

            if (equipamento != null)
            {
                // Verificação de Segurança Final
                // CORREÇÃO: Uso de '?' para segurança
                if (equipamento.ExercicioEquipamentos != null && equipamento.ExercicioEquipamentos.Any())
                {
                    TempData["ErrorMessage"] = "Não é possível eliminar este equipamento porque existem exercícios associados.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Equipamento.Remove(equipamento);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Equipamento foi apagado com sucesso.";
            }
            else
            {
                TempData["SuccessMessage"] = "Este equipamento já tinha sido eliminado.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool EquipamentoExists(int id)
        {
            return _context.Equipamento.Any(e => e.EquipamentoId == id);
        }
    }
}