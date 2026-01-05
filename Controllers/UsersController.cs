using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Administrador")] // Só admins podem ver isto
public class UsersController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UsersController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // LISTAR TODOS OS USERS E ROLES
    public async Task<IActionResult> Index(string searchEmail, int page = 1)
    {
        // 1. Iniciar a query sobre os utilizadores
        var query = _userManager.Users.AsQueryable();

        // 2. Aplicar o filtro de pesquisa
        if (!string.IsNullOrEmpty(searchEmail))
        {
            query = query.Where(u => u.Email.Contains(searchEmail));
        }

        // 3. Contar total de resultados para a paginação
        int totalItems = await query.CountAsync();

        // 4. Criar o objeto de paginação (ViewModel de paginação)
        var pagination = new PaginationInfo<UserRoleViewModel>(page, totalItems);

        // 5. Paginar na base de dados
        var usersPaged = await query
            .OrderBy(u => u.Email)
            .Skip(pagination.ItemsToSkip)
            .Take(pagination.ItemsPerPage)
            .ToListAsync();

        // 6. Converter para a lista de ViewModels com as Roles
        var userRoleList = new List<UserRoleViewModel>();
        foreach (var user in usersPaged)
        {
            userRoleList.Add(new UserRoleViewModel
            {
                UserId = user.Id,
                Email = user.Email,
                Roles = (await _userManager.GetRolesAsync(user)).ToList()
            });
        }

        // 7. Injetar os itens no objeto de paginação
        pagination.Items = userRoleList;

        // Guardar o filtro para a View manter o texto na caixa e nos links
        ViewBag.SearchEmail = searchEmail;

        return View(pagination);
    }

    // ALTERAR ROLE (Exemplo simplificado: Alternar para Profissional)
    [HttpPost]
    public async Task<IActionResult> ChangeToProfissional(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            // Remove roles atuais para evitar conflitos
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            // Adiciona a nova role
            await _userManager.AddToRoleAsync(user, "ProfissionalSaude");
        }
        return RedirectToAction(nameof(Index));
    }

    // ALTERAR ROLE (Exemplo simplificado: Alternar para Admin)
    [HttpPost]
    public async Task<IActionResult> ChangeToAdmin(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            // Remove roles atuais para evitar conflitos
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            // Adiciona a nova role
            await _userManager.AddToRoleAsync(user, "Administrador");
        }
        return RedirectToAction(nameof(Index));
    }

    // ALTERAR ROLE (Exemplo simplificado: Alternar para Client)
    [HttpPost]
    public async Task<IActionResult> ChangeToClient(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            // Remove roles atuais para evitar conflitos
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            // Adiciona a nova role
            await _userManager.AddToRoleAsync(user, "Utente");
        }
        return RedirectToAction(nameof(Index));
    }
}