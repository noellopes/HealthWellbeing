using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthWellbeing.Controllers
{
    public class AccountController : Controller
    {
        // Lista simulada (substitui a base de dados)
        private static List<User> users = new List<User>();

        // GET: Página de login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Processa login
        [HttpPost]
        public IActionResult Login(User user)
        {
            var existingUser = users.Find(u => u.UserId == user.UserId && u.Password == user.Password);

            if (existingUser != null)
            {
                // Redireciona conforme o tipo (role)
                switch (existingUser.Role.ToLower())
                {
                    case "médico":
                    case "medico":
                        return RedirectToAction("Index", "Doctors");

                    case "rececionista":
                        return RedirectToAction("Dashboard", "Reception");

                    case "utente":
                        return RedirectToAction("Dashboard", "Patients");

                    case "diretor":
                        return RedirectToAction("Dashboard", "Director");

                    case "admin":
                        return RedirectToAction("Dashboard", "Admin");

                    default:
                        return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.ErrorMessage = "ID ou palavra-passe incorretos.";
            return View();
        }

        // GET: Registo
        public IActionResult Register()
        {
            return View();
        }

        // POST: Registo
        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                users.Add(user);
                return RedirectToAction("Login");
            }

            return View(user);
        }
    }
}
