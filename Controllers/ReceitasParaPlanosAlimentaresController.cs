using System.Diagnostics;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthWellbeing.Controllers
{
    [Authorize] // Restringe o acesso a usuários autenticados
    public class ReceitasParaPlanosAlimentaresController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        
        public IActionResult ReceitasParaPlanosAlimentares()
        {
            if (!User.Identity.IsAuthenticated)
            {
                ViewData["AccessDenied"] = true;
                return View();
            }

            return View();
        }
    }
}
