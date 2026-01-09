using HealthWellbeing.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        // Roles permitidas (SeedData)
        private static readonly string[] AllowedRoles =
        {
            "Administrador",
            "DiretorClinico",
            "Medico",
            "Rececionista",
            "Utente"
        };

        public AdminController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Users(string search = "")
        {
            var q = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                q = q.Where(u =>
                    (u.Email != null && u.Email.Contains(s)) ||
                    (u.UserName != null && u.UserName.Contains(s))
                );
            }

            var users = await q.OrderBy(u => u.Email).ToListAsync();

            var vm = new AdminUsersListVM { Search = search };

            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);

                vm.Users.Add(new AdminUserListItemVM
                {
                    UserId = u.Id,
                    Email = u.Email ?? "",
                    UserName = u.UserName ?? "",
                    Roles = string.Join(", ", roles.OrderBy(r => r)),
                    EmailConfirmed = u.EmailConfirmed
                });
            }

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> EditRoles(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // Só mostrar as roles do SeedData que existam na BD
            var existingRoles = await _roleManager.Roles
                .Select(r => r.Name!)
                .ToListAsync();

            var rolesToShow = AllowedRoles
                .Where(r => existingRoles.Contains(r))
                .ToList();

            var userRoles = await _userManager.GetRolesAsync(user);

            // se tiver mais do que uma role, escolhe a primeira (ordem do SeedData)
            string? selected = rolesToShow.FirstOrDefault(r => userRoles.Contains(r));
            if (selected == null && userRoles.Any())
                selected = userRoles.FirstOrDefault();

            var vm = new EditUserRolesVM
            {
                UserId = user.Id,
                Email = user.Email ?? user.UserName ?? "",
                AvailableRoles = rolesToShow,
                SelectedRole = selected
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRoles(EditUserRolesVM vm)
        {
            var user = await _userManager.FindByIdAsync(vm.UserId);
            if (user == null) return NotFound();

            if (string.IsNullOrWhiteSpace(vm.SelectedRole))
            {
                TempData["Msg"] = "Seleciona uma permissão.";
                return RedirectToAction(nameof(EditRoles), new { id = vm.UserId });
            }

            // garantir que é uma role permitida (SeedData)
            if (!AllowedRoles.Contains(vm.SelectedRole))
            {
                TempData["Msg"] = "Permissão inválida.";
                return RedirectToAction(nameof(EditRoles), new { id = vm.UserId });
            }

            // garantir que a role existe mesmo
            if (!await _roleManager.RoleExistsAsync(vm.SelectedRole))
            {
                TempData["Msg"] = "A role selecionada não existe na base de dados.";
                return RedirectToAction(nameof(EditRoles), new { id = vm.UserId });
            }

            // impedir que o admin se auto-remova de Administrador
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == user.Id && vm.SelectedRole != "Administrador")
            {
                TempData["Msg"] = "Não podes remover a tua própria role Administrador.";
                return RedirectToAction(nameof(EditRoles), new { id = vm.UserId });
            }

            // remover todas as roles atuais e atribuir só 1
            var currentRoles = await _userManager.GetRolesAsync(user);

            if (currentRoles.Any())
            {
                var removeRes = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeRes.Succeeded)
                {
                    TempData["Msg"] = "Erro ao remover roles atuais.";
                    return RedirectToAction(nameof(EditRoles), new { id = vm.UserId });
                }
            }

            var addRes = await _userManager.AddToRoleAsync(user, vm.SelectedRole);
            if (!addRes.Succeeded)
            {
                TempData["Msg"] = "Erro ao atribuir a role selecionada.";
                return RedirectToAction(nameof(EditRoles), new { id = vm.UserId });
            }

            // força atualização de permissões em sessões ativas
            await _userManager.UpdateSecurityStampAsync(user);

            // se o utilizador editado for o próprio admin logado, refresca cookie já
            if (currentUserId == user.Id)
                await _signInManager.RefreshSignInAsync(user);

            TempData["Msg"] = "Permissão atualizada com sucesso.";
            return RedirectToAction(nameof(Users));
        }
    }
}
