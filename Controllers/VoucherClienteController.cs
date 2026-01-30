using Microsoft.AspNetCore.Mvc;

namespace HealthWellbeing.Controllers
{
    public class VoucherClienteController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
