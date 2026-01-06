using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using static HealthWellbeing.Data.SeedData;

namespace HealthWellbeing.Controllers
{
    [Authorize(Roles = SeedData.Roles.Administrador + "," + SeedData.Roles.Profissional)]

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
        [Authorize(Roles = SeedData.Roles.Administrador + "," + SeedData.Roles.Profissional)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var equipamento = await _context.Equipamento
                .Include(e => e.ExercicioEquipamentos) // Necessário para contar os usos
                .FirstOrDefaultAsync(m => m.EquipamentoId == id);

            if (equipamento == null)
            {
                TempData["ErrorMessage"] = "Este equipamento já não existe.";
                return RedirectToAction(nameof(Index));
            }

            // Lógica visual: Verificar se está em uso para avisar o utilizador antes de ele clicar
            int numExercicios = equipamento.ExercicioEquipamentos?.Count ?? 0;

            ViewBag.NumExercicios = numExercicios;
            ViewBag.PodeEliminar = numExercicios == 0;

            return View(equipamento);
        }

        // POST: Equipamento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SeedData.Roles.Administrador + "," + SeedData.Roles.Profissional)]
        public async Task<IActionResult> DeleteConfirmed(int EquipamentoId) // Mudei para EquipamentoId para ser explícito
        {
            var equipamento = await _context.Equipamento.FindAsync(EquipamentoId);

            if (equipamento == null) return RedirectToAction(nameof(Index));

            try
            {
                _context.Equipamento.Remove(equipamento);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Equipamento eliminado com sucesso.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                // Este bloco apanha o erro se a Base de Dados recusar apagar (regra Restrict)
                TempData["ErrorMessage"] = "Não é possível eliminar este equipamento porque ele está associado a um ou mais exercícios.";

                // Redireciona de volta para a página de Delete para mostrar o erro
                return RedirectToAction(nameof(Delete), new { id = EquipamentoId });
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Ocorreu um erro inesperado ao tentar eliminar o equipamento.";
                return RedirectToAction(nameof(Delete), new { id = EquipamentoId });
            }
        }

        private bool EquipamentoExists(int id)
        {
            return _context.Equipamento.Any(e => e.EquipamentoId == id);
        }
    }
}