using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Administrador")]
public class UsersController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UsersController(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // ==========================
    // LISTAR USERS COM PAGINAÇÃO
    // ==========================
    public async Task<IActionResult> Index(string searchEmail, int page = 1)
    {
        var query = _userManager.Users.AsQueryable();

        if (!string.IsNullOrEmpty(searchEmail))
        {
            query = query.Where(u => u.Email.Contains(searchEmail));
        }

        int totalItems = await query.CountAsync();

        var pagination = new PaginationInfo<UserRoleViewModel>(page, totalItems);

        var usersPaged = await query
            .OrderBy(u => u.Email)
            .Skip(pagination.ItemsToSkip)
            .Take(pagination.ItemsPerPage)
            .ToListAsync();

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

        pagination.Items = userRoleList;

        ViewBag.SearchEmail = searchEmail;

        return View(pagination);
    }

    // ==========================
    // MÉTODO AUXILIAR
    // ==========================
    private async Task<int> CountAdminsAsync()
    {
        var admins = await _userManager.GetUsersInRoleAsync("Administrador");
        return admins.Count;
    }

    // ==========================
    // ALTERAR PARA ADMIN
    // ==========================
    [HttpPost]
    public async Task<IActionResult> ChangeToAdmin(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return RedirectToAction(nameof(Index));

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRoleAsync(user, "Administrador");

        return RedirectToAction(nameof(Index));
    }

    // ==========================
    // ALTERAR PARA PROFISSIONAL
    // ==========================
    [HttpPost]
    public async Task<IActionResult> ChangeToProfissional(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return RedirectToAction(nameof(Index));

        var roles = await _userManager.GetRolesAsync(user);

        if (roles.Contains("Administrador") && await CountAdminsAsync() <= 1)
        {
            TempData["Error"] = "Não é possível remover o último Administrador do sistema.";
            return RedirectToAction(nameof(Index));
        }

        await _userManager.RemoveFromRolesAsync(user, roles);
        await _userManager.AddToRoleAsync(user, "ProfissionalSaude");

        return RedirectToAction(nameof(Index));
    }

    // ==========================
    // ALTERAR PARA UTENTE
    // ==========================
    [HttpPost]
    public async Task<IActionResult> ChangeToClient(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return RedirectToAction(nameof(Index));

        var roles = await _userManager.GetRolesAsync(user);

        if (roles.Contains("Administrador") && await CountAdminsAsync() <= 1)
        {
            TempData["Error"] = "Não é possível remover o último Administrador do sistema.";
            return RedirectToAction(nameof(Index));
        }

        await _userManager.RemoveFromRolesAsync(user, roles);
        await _userManager.AddToRoleAsync(user, "Utente");

        return RedirectToAction(nameof(Index));
    }
}
