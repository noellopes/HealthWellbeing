using Microsoft.AspNetCore.Mvc;

namespace HealthWellbeing.Controllers
{
    public class ClientController : Controller
    {
		// GET: /Client/Index
		public IActionResult Index()
        {
            return View();
        }
    }
}
