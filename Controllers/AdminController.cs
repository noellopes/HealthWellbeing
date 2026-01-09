using Microsoft.AspNetCore.Mvc;

namespace HealthWellbeing.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
