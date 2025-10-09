using Microsoft.AspNetCore.Mvc;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class ConsumivelController : Controller
    {
        // GET: /Consumivel/ConsumivelRegister
        public IActionResult ConsumivelRegister()
        {
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
