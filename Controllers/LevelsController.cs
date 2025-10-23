using Microsoft.AspNetCore.Mvc;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class LevelsController : Controller
    {
        public IActionResult Index(int level = 1)
        {
            if (level < 1) level = 1;
            if (level > 100) level = 100;

            var model = new Levels(level);
            return View(model);
        }
    }
}
