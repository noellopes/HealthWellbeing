using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class ProfissionalExecutantesController : Controller
{
    private readonly HealthWellbeingDbContext _context;

    public ProfissionalExecutantesController(HealthWellbeingDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int page = 1, string searchNome = null, string searchFuncao = null)
    {
        int itemsPerPage = 5;

        IQueryable<ProfissionalExecutante> profissionaisQuery =
            _context.ProfissionalExecutante
            .Include(p => p.Funcao)
            .OrderBy(p => p.Nome);

        if (!string.IsNullOrEmpty(searchNome))
            profissionaisQuery = profissionaisQuery.Where(p => p.Nome.Contains(searchNome));

        if (!string.IsNullOrEmpty(searchFuncao))
            profissionaisQuery = profissionaisQuery.Where(p => p.Funcao.NomeFuncao.Contains(searchFuncao));

        int totalItems = await profissionaisQuery.CountAsync();
        var paginationInfo = new PaginationInfo<ProfissionalExecutante>(page, totalItems, itemsPerPage);

        paginationInfo.Items = await profissionaisQuery
            .Skip(paginationInfo.ItemsToSkip)
            .Take(paginationInfo.ItemsPerPage)
            .ToListAsync();

        return View(paginationInfo);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var profissional = await _context.ProfissionalExecutante
            .Include(p => p.Funcao)
            .FirstOrDefaultAsync(m => m.ProfissionalExecutanteId == id);

        if (profissional == null) return NotFound();

        return View(profissional);
    }

    public IActionResult Create()
    {
        ViewData["FuncaoId"] = new SelectList(_context.Funcoes, "FuncaoId", "NomeFuncao");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ProfissionalExecutanteId,Nome,Telefone,Email,FuncaoId")] ProfissionalExecutante profissionalExecutante)
    {
        if (ModelState.IsValid)
        {
            _context.Add(profissionalExecutante);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["FuncaoId"] = new SelectList(_context.Funcoes, "FuncaoId", "NomeFuncao", profissionalExecutante.FuncaoId);
        return View(profissionalExecutante);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var profissional = await _context.ProfissionalExecutante
            .Include(p => p.Funcao)
            .FirstOrDefaultAsync(p => p.ProfissionalExecutanteId == id);

        if (profissional == null) return NotFound();

        ViewData["FuncaoId"] = new SelectList(_context.Funcoes, "FuncaoId", "NomeFuncao", profissional.FuncaoId);
        return View(profissional);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("ProfissionalExecutanteId,Nome,Telefone,Email,FuncaoId")] ProfissionalExecutante profissionalExecutante)
    {
        if (id != profissionalExecutante.ProfissionalExecutanteId) return NotFound();

        if (ModelState.IsValid)
        {
            _context.Update(profissionalExecutante);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["FuncaoId"] = new SelectList(_context.Funcoes, "FuncaoId", "NomeFuncao", profissionalExecutante.FuncaoId);
        return View(profissionalExecutante);
    }

    // GET opcional apenas para confirmação, pode remover se não precisar
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var profissional = await _context.ProfissionalExecutante
            .Include(p => p.Funcao)
            .FirstOrDefaultAsync(p => p.ProfissionalExecutanteId == id);

        if (profissional == null) return NotFound();

        return View(profissional); // se tiver view de confirmação
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var profissional = await _context.ProfissionalExecutante.FindAsync(id);
        if (profissional != null)
        {
            _context.ProfissionalExecutante.Remove(profissional);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}