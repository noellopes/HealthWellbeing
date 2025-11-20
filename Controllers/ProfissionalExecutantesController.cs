using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // Adicionado para SelectList
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Controllers // Namespace CORRETO
{
    public class ProfissionalExecutantesController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ProfissionalExecutantesController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // ==================================================================================
        // AÇÃO INDEX (Listagem e Pesquisa)
        // ==================================================================================
        public async Task<IActionResult> Index(int page = 1, string searchNome = null, string searchFuncao = null)
        {
            int itemsPerPage = 5;

            // 1. Usa .Include() para carregar a entidade Funcao
            IQueryable<ProfissionalExecutante> profissionaisQuery = _context.ProfissionalExecutante
                                                                            .Include(p => p.Funcao)
                                                                            .OrderBy(p => p.Nome)
                                                                            .AsQueryable();

            if (!string.IsNullOrEmpty(searchNome))
            {
                profissionaisQuery = profissionaisQuery.Where(p => p.Nome.Contains(searchNome));
                ViewBag.SearchNome = searchNome;
            }

            if (!string.IsNullOrEmpty(searchFuncao))
            {
                // 2. CORREÇÃO CRÍTICA: Pesquisar pelo NomeFuncao da entidade relacionada
                profissionaisQuery = profissionaisQuery.Where(p => p.Funcao.NomeFuncao.Contains(searchFuncao));
                ViewBag.SearchFuncao = searchFuncao;
            }

            int totalItems = await profissionaisQuery.CountAsync();
            var paginationInfo = new PaginationInfo<ProfissionalExecutante>(page, totalItems, itemsPerPage);

            paginationInfo.Items = await profissionaisQuery
                                            .Skip(paginationInfo.ItemsToSkip)
                                            .Take(paginationInfo.ItemsPerPage)
                                            .ToListAsync();

            if (!string.IsNullOrEmpty(searchNome) || !string.IsNullOrEmpty(searchFuncao))
            {
                ViewBag.Collapse = "show";
            }

            return View(paginationInfo);
        }

        // ==================================================================================
        // AÇÃO DETAILS (Detalhes)
        // ==================================================================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Usa .Include() para carregar a função
            var profissionalExecutante = await _context.ProfissionalExecutante
                .Include(p => p.Funcao)
                .FirstOrDefaultAsync(m => m.ProfissionalExecutanteId == id);

            if (profissionalExecutante == null)
            {
                return NotFound();
            }

            return View(profissionalExecutante);
        }

        // ==================================================================================
        // AÇÃO CREATE (Criação de Novo Registo)
        // ==================================================================================
        public IActionResult Create()
        {
            // Carrega a lista de Funções para o Dropdown
            ViewData["FuncaoId"] = new SelectList(_context.Funcoes, "FuncaoId", "NomeFuncao");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // ATUALIZADO: Usar FuncaoId no Bind e remover Funcao string
        public async Task<IActionResult> Create([Bind("ProfissionalExecutanteId,Nome,Telefone,Email,FuncaoId")] ProfissionalExecutante profissionalExecutante)
        {
            if (ModelState.IsValid)
            {
                TempData["SuccessMessage"] = $"Profissional '{profissionalExecutante.Nome}' criado com sucesso!";
                _context.Add(profissionalExecutante);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Recarrega Funções se a validação falhar
            ViewData["FuncaoId"] = new SelectList(_context.Funcoes, "FuncaoId", "NomeFuncao", profissionalExecutante.FuncaoId);
            return View(profissionalExecutante);
        }

        // ==================================================================================
        // AÇÃO EDIT (Edição de Registo Existente)
        // ==================================================================================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profissionalExecutante = await _context.ProfissionalExecutante.FindAsync(id);

            if (profissionalExecutante == null)
            {
                return NotFound();
            }

            // Carrega Funções e marca a FuncaoId atual como selecionada
            ViewData["FuncaoId"] = new SelectList(_context.Funcoes, "FuncaoId", "NomeFuncao", profissionalExecutante.FuncaoId);
            return View(profissionalExecutante);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // ATUALIZADO: Usar FuncaoId no Bind e remover Funcao string
        public async Task<IActionResult> Edit(int id, [Bind("ProfissionalExecutanteId,Nome,Telefone,Email,FuncaoId")] ProfissionalExecutante profissionalExecutante)
        {
            if (id != profissionalExecutante.ProfissionalExecutanteId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(profissionalExecutante);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Profissional '{profissionalExecutante.Nome}' editado com sucesso!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProfissionalExecutanteExists(profissionalExecutante.ProfissionalExecutanteId))
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

            // Recarrega Funções se a validação falhar
            ViewData["FuncaoId"] = new SelectList(_context.Funcoes, "FuncaoId", "NomeFuncao", profissionalExecutante.FuncaoId);
            return View(profissionalExecutante);
        }

        // ==================================================================================
        // AÇÃO DELETE (Confirmação e Execução)
        // ==================================================================================

        // Ação Delete GET precisa de .Include() (já atualizado em Details/Delete)

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var profissionalExecutante = await _context.ProfissionalExecutante.FindAsync(id);
            if (profissionalExecutante != null)
            {
                var nome = profissionalExecutante.Nome;
                _context.ProfissionalExecutante.Remove(profissionalExecutante);

                TempData["SuccessMessage"] = $"Profissional '{nome}' eliminado com sucesso!";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProfissionalExecutanteExists(int id)
        {
            return _context.ProfissionalExecutante.Any(e => e.ProfissionalExecutanteId == id);
        }
    }
}