using Microsoft.AspNetCore.Mvc;

namespace HealthWellbeing.Controllers
{
    public class GestaoDeTratamentosModules : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/Shared/Group1/Index.cshtml");
        }
    }
}
