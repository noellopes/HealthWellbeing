using Microsoft.AspNetCore.Mvc;

namespace HealthWellbeing.Controllers
{
    public class RegistoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
