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
    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.ToListAsync();
        var userRoleList = new List<UserRoleViewModel>();

        foreach (var user in users)
        {
            var viewModel = new UserRoleViewModel
            {
                UserId = user.Id,
                Email = user.Email,
                Roles = (await _userManager.GetRolesAsync(user)).ToList()
            };
            userRoleList.Add(viewModel);
        }

        return View(userRoleList);
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
            await _userManager.AddToRoleAsync(user, "Cliente");
        }
        return RedirectToAction(nameof(Index));
    }
}