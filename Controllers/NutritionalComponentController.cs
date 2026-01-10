using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;

namespace HealthWellbeing.Controllers
{
    [Authorize]
    public class NutritionalComponentController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public NutritionalComponentController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // Verifica se o usuário é Administrador
        private bool IsAdmin()
            => User.IsInRole("Administrador") || User.IsInRole("Administrator");

        // Verifica se o usuário é Nutricionista
        private bool IsNutritionist()
            => User.IsInRole("Nutricionista") || User.IsInRole("Nutritionist");

        // Verifica se o usuário é Cliente
        private bool IsClient()
            => User.IsInRole("Cliente") || User.IsInRole("Client");

        // Redireciona para a página de "Sem Permissão de Dados"
        private IActionResult NoDataPermission()
            => View("~/Views/Shared/NoDataPermission.cshtml");

        // Ação Index para listar os componentes nutricionais
        public async Task<IActionResult> Index(string? search, int page = 1, int itemsPerPage = 10)
        {
            if (IsNutritionist() || IsClient()) return NoDataPermission(); // Apenas Administradores podem adicionar, editar ou excluir.

            var query = _context.NutritionalComponent.AsQueryable();

            // Se houver um valor de pesquisa, aplica o filtro
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLowerInvariant();
                query = query.Where(nc =>
                    nc.Name.ToLower().Contains(s) ||
                    nc.Unit.ToLower().Contains(s) ||
                    nc.Basis.ToLower().Contains(s)
                );
            }

            // Conta o número total de componentes nutricionais
            int totalItems = await query.CountAsync();

            // Obtém os itens paginados
            var items = await query
                .OrderBy(nc => nc.Name)  // Ordena pelo nome
                .Skip((page - 1) * itemsPerPage)  // Paginando os resultados
                .Take(itemsPerPage)  // Limita a quantidade de itens por página
                .ToListAsync();

            // Cria o modelo de paginação para a view
            var model = new PaginationInfoFoodHabits<NutritionalComponent>(items, totalItems, page, itemsPerPage);

            ViewBag.Search = search ?? "";
            return View(model);
        }

        // Ação para visualizar os detalhes de um componente nutricional
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var nutritionalComponent = await _context.NutritionalComponent
                .FirstOrDefaultAsync(m => m.NutritionalComponentId == id);

            if (nutritionalComponent == null) return NotFound();

            return View(nutritionalComponent);
        }

        // Ação para criar um novo componente nutricional (somente Administradores)
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            if (IsNutritionist() || IsClient()) return NoDataPermission();
            return View();
        }

        // Ação para salvar um novo componente nutricional (somente Administradores)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create([Bind("NutritionalComponentId,Name,Unit,Basis")] NutritionalComponent nutritionalComponent)
        {
            if (IsNutritionist() || IsClient()) return NoDataPermission();

            if (ModelState.IsValid)
            {
                _context.Add(nutritionalComponent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));  // Redireciona para a lista de componentes nutricionais
            }

            return View(nutritionalComponent); // Se a validação falhar, retorna para a tela de criação
        }

        // Ação para editar um componente nutricional (somente Administradores)
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (IsNutritionist() || IsClient()) return NoDataPermission();
            if (id == null) return NotFound();

            var nutritionalComponent = await _context.NutritionalComponent.FindAsync(id);
            if (nutritionalComponent == null) return NotFound();

            return View(nutritionalComponent);
        }

        // Ação para salvar as edições (somente Administradores)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id, [Bind("NutritionalComponentId,Name,Unit,Basis")] NutritionalComponent nutritionalComponent)
        {
            if (IsNutritionist() || IsClient()) return NoDataPermission();
            if (id != nutritionalComponent.NutritionalComponentId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nutritionalComponent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NutritionalComponentExists(nutritionalComponent.NutritionalComponentId))
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

            return View(nutritionalComponent);
        }

        // Ação para excluir um componente nutricional (somente Administradores)
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (IsNutritionist() || IsClient()) return NoDataPermission();
            if (id == null) return NotFound();

            var nutritionalComponent = await _context.NutritionalComponent
                .FirstOrDefaultAsync(m => m.NutritionalComponentId == id);
            if (nutritionalComponent == null) return NotFound();

            return View(nutritionalComponent);
        }

        // Ação para confirmar a exclusão (somente Administradores)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (IsNutritionist() || IsClient()) return NoDataPermission();

            var nutritionalComponent = await _context.NutritionalComponent.FindAsync(id);
            if (nutritionalComponent != null)
            {
                _context.NutritionalComponent.Remove(nutritionalComponent);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index)); // Redireciona para a lista após a exclusão
        }

        private bool NutritionalComponentExists(int id)
        {
            return _context.NutritionalComponent.Any(e => e.NutritionalComponentId == id);
        }
    }
}
