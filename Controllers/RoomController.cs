using HealthWellbeing.Models;
using HealthWellbeingRoom.Models;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using System.Diagnostics;

namespace HealthWellbeingRoom.Controllers
{
    public class RoomController : Controller
    {
        public IActionResult CreateRoom()
        {
            return View();
        }
        public IActionResult ReadRoom()
        {
            return View();
        }


        public IActionResult UpdateRoom()
        {
            return View();
        }

        public IActionResult DeleteRoom()
        {
            return View();
        }

        //Metodo POST para criar uma sala
        [HttpPost]
        public IActionResult CreateRoom(Room room)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            RepositoryRoom.AddRoom(room);

            return View("Sala Criada com Sucesso!!!", room);
        }

    }
}
