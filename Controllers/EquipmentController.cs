using HealthWellbeingRoom.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthWellbeingRoom.Controllers
{
    public class EquipmentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            System.Console.WriteLine("Test");
            return View("Create");
        }

        [HttpPost]
        public IActionResult Save()
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            return View("Index");
        }
    }
}
