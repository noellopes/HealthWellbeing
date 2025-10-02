using Microsoft.AspNetCore.Mvc;

namespace HealthWellbeing.Controllers
{
    public class ExerciciosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
