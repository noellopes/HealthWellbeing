using Microsoft.AspNetCore.Mvc;

namespace HealthWellbeing.Controllers
{
    public class Exame : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
