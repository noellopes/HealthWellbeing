using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using HealthWellBeing.Models;
using Microsoft.AspNetCore.Authorization; // Necessário para [Authorize]
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // Necessário para SelectList
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;          // Necessário para User.FindFirstValue
using System.Threading.Tasks;

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
    [Authorize]
    public async Task<IActionResult> Index(int page = 1, string searchNome = null, string searchFuncao = null)
    {
        int itemsPerPage = 5;

        IQueryable<ProfissionalExecutante> profissionaisQuery =
            _context.ProfissionalExecutante
            .Include(p => p.Funcao)
            .Include(p => p.User) // Inclui o Utilizador (Identity)
            .OrderBy(p => p.Nome);

        if (!string.IsNullOrEmpty(searchNome))
            profissionaisQuery = profissionaisQuery.Where(p => p.Nome.Contains(searchNome));

        if (!string.IsNullOrEmpty(searchFuncao))
            // Filtro pela propriedade NomeFuncao da entidade Funcao
            profissionaisQuery = profissionaisQuery.Where(p => p.Funcao.NomeFuncao.Contains(searchFuncao));

        int totalItems = await profissionaisQuery.CountAsync();
        var paginationInfo = new PaginationInfo<ProfissionalExecutante>(page, totalItems, itemsPerPage);

        paginationInfo.Items = await profissionaisQuery
            .Skip(paginationInfo.ItemsToSkip)
            .Take(paginationInfo.ItemsPerPage)
            .ToListAsync();

        return View(paginationInfo);
    }

    // ==================================================================================
    // AÇÃO DETAILS (Detalhes)
    // ==================================================================================
    [Authorize]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var profissional = await _context.ProfissionalExecutante
            .Include(p => p.Funcao)
            .Include(p => p.User) // Inclui o Utilizador (Identity)
            .FirstOrDefaultAsync(m => m.ProfissionalExecutanteId == id);

        if (profissional == null) return NotFound();

        return View(profissional);
    }

    // ==================================================================================
    // AÇÃO CREATE (Criação)
    // ==================================================================================
    [Authorize]
    public IActionResult Create()
    {
        // Dropdown para FuncaoId
        ViewData["FuncaoId"] = new SelectList(_context.Funcoes, "FuncaoId", "NomeFuncao");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Create([Bind("ProfissionalExecutanteId,Nome,Telefone,Email,FuncaoId")] ProfissionalExecutante profissionalExecutante)
    {
        if (ModelState.IsValid)
        {
            // 🎯 INJETAR O USER ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            profissionalExecutante.UserId = userId;

            _context.Add(profissionalExecutante);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["FuncaoId"] = new SelectList(_context.Funcoes, "FuncaoId", "NomeFuncao", profissionalExecutante.FuncaoId);
        return View(profissionalExecutante);
    }

    // ==================================================================================
    // AÇÃO EDIT (Edição)
    // ==================================================================================
    [Authorize]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var profissional = await _context.ProfissionalExecutante
            .Include(p => p.Funcao)
            .FirstOrDefaultAsync(p => p.ProfissionalExecutanteId == id);

        // 🎯 LÓGICA DE AUTORIZAÇÃO: Verifica se o utilizador logado é o proprietário
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (profissional != null && profissional.UserId != userId)
        {
            return Forbid();
        }

        if (profissional == null) return NotFound();

        ViewData["FuncaoId"] = new SelectList(_context.Funcoes, "FuncaoId", "NomeFuncao", profissional.FuncaoId);
        return View(profissional);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    // Incluir UserId no Bind é necessário para que a chave estrangeira seja preservada
    public async Task<IActionResult> Edit(int id, [Bind("ProfissionalExecutanteId,Nome,Telefone,Email,FuncaoId,UserId")] ProfissionalExecutante profissionalExecutante)
    {
        if (id != profissionalExecutante.ProfissionalExecutanteId) return NotFound();

        // 🎯 LÓGICA DE AUTORIZAÇÃO: Verifica se o utilizador logado é o proprietário
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (profissionalExecutante.UserId != userId)
        {
            return Forbid();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(profissionalExecutante);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                // ... (lógica de concorrência)
            }
        }

        ViewData["FuncaoId"] = new SelectList(_context.Funcoes, "FuncaoId", "NomeFuncao", profissionalExecutante.FuncaoId);
        return View(profissionalExecutante);
    }

    // ==================================================================================
    // AÇÃO DELETE (Confirmação e Execução)
    // ==================================================================================
    [Authorize]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var profissional = await _context.ProfissionalExecutante
            .Include(p => p.Funcao)
            .FirstOrDefaultAsync(p => p.ProfissionalExecutanteId == id);

        // 🎯 LÓGICA DE AUTORIZAÇÃO: Verifica se o utilizador logado é o proprietário
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (profissional != null && profissional.UserId != userId)
        {
            return Forbid();
        }

        if (profissional == null) return NotFound();

        return View(profissional);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var profissional = await _context.ProfissionalExecutante.FindAsync(id);

        // 🎯 LÓGICA DE AUTORIZAÇÃO: Verifica se o utilizador logado é o proprietário
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (profissional != null && profissional.UserId != userId)
        {
            return Forbid();
        }

        if (profissional != null)
        {
            _context.ProfissionalExecutante.Remove(profissional);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool ProfissionalExecutanteExists(int id)
    {
        return _context.ProfissionalExecutante.Any(e => e.ProfissionalExecutanteId == id);
    }
}