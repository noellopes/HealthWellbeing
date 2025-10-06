using Microsoft.AspNetCore.Mvc;

namespace HealthWellbeing.Controllers
{
    public class MaterialController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }
    }
}
