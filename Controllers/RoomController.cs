using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HealthWellbeingRoom.Controllers
{
    public class RoomController : Controller
    {
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult Read()
        {
            return View();
        }


        public IActionResult Update()
        {
            return View();
        }

        public IActionResult Delete()
        {
            return View();
        }

    }
}
