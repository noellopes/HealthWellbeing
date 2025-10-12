using HealthWellbeing.Models;
using HealthWellbeingRoom.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using System.Diagnostics;

namespace HealthWellbeingRoom.Controllers
{
    public class RoomController : Controller
    {
        [HttpGet]
        public IActionResult CreateRoom()
        {
            return View();
        }
        [HttpGet]
        public IActionResult ReadRoom()
        {
            var rooms = RepositoryRoom.Rooms;
            return View(rooms);
        }
        [HttpGet]
        public IActionResult UpdateRoom()
        {
            return View();
        }
        [HttpGet]
        public IActionResult DeleteRoom()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateRoom(Room room)
        {

            if (!ModelState.IsValid)
            {
                return View(room); //se o modelo nao for valido, retorna a view com os erros
            }
            room.SalaId = RepositoryRoom.Rooms.Count() + 1;
            RepositoryRoom.AddRoom(room);
            return View("CreateRoomComplete", room);
        }
        [HttpGet]
        public IActionResult CreateRoomComplete()
        {
            return View();
        }
    }
}
