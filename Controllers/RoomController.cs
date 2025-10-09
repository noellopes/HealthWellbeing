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
                return View(room);//se o modelo nao for valido, retorna a view com os erros
            }

            RepositoryRoom.AddRoom(room);//guarda a sala na "base de dados"

            return View("CreateRoomComplete", room);
        }


        //public IActionResult VerSala()
        //{

        //    var rooms =;

        //    if (rooms.)
        //    {
        //        return View(rooms);
        //    }
        //    else
        //    {
        //        return View("Naohasalas");
        //    }
        //}
    }
}
