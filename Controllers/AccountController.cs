using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;

namespace HealthWellbeing.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly HealthWellbeingDbContext _context;

        public AccountController(UserManager<IdentityUser> userManager,
                                 SignInManager<IdentityUser> signInManager,
                                 HealthWellbeingDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // GET: Account/Register
        public IActionResult Register()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Account/Register
        // Usa o ClientInputModel que criou para registar o utilizador e o cliente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(ClientInputModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. Criar o Utilizador do Identity (Login e Password)
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // 2. Atribuir o Role "Client"
                    await _userManager.AddToRoleAsync(user, "Client");

                    // 3. Criar o Registo na tabela Client (Ligação ao Negócio)
                    var client = new Client
                    {
                        Name = model.Name,
                        Email = model.Email,
                        Phone = model.PhoneNumber,
                        BirthDate = model.BirthDate,
                        RegistrationDate = DateTime.Now,
                        IdentityUserId = user.Id // A LIGAÇÃO IMPORTANTE (Passo 1 que já fez)
                    };

                    _context.Client.Add(client);
                    await _context.SaveChangesAsync();

                    // 4. Login automático e redirecionar
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // Redireciona para a página pública de planos (início do fluxo)
                    return RedirectToAction("PublicIndex", "Plan");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        // GET: Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    // Redirecionar com base no Role (Opcional, mas útil)
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (await _userManager.IsInRoleAsync(user, "Administrator"))
                    {
                        return RedirectToAction("Index", "Home"); // Admin vai para a Home/Dashboard
                    }
                    // Cliente vai para a sua área de Planos ou Treinos
                    return RedirectToAction("PublicIndex", "Plan");
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }

        // POST: Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}