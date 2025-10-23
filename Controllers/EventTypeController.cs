using Microsoft.AspNetCore.Mvc;

namespace HealthWellbeing.Controllers {
    public class EventTypeController : Controller {
        public IActionResult Index() {
            return View();
        }
    }
}
