using System.Diagnostics;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthWellbeing.Controllers
{
    public class ReceitasParaPlanosAlimentaresController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        
        public IActionResult ReceitasParaPlanosAlimentares()
        {
            return View();
        }
    }
}
