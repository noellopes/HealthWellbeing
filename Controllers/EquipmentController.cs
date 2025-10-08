using Microsoft.AspNetCore.Mvc;

namespace HealthWellbeingRoom.Controllers
{
    public class EquipmentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
