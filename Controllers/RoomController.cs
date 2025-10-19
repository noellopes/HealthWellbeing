using HealthWellbeing.Models;
using HealthWellbeingRoom.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthWellbeingRoom.Controllers
{
    public class RoomController : Controller
    {
        //Pagina principal de salas, listar salas.
        [HttpGet]
        public IActionResult IndexRoom()
        {
            var rooms = RepositoryRoom.Rooms;
            return View(rooms);
        }
        // GET: Criar nova sala
        [HttpGet]
        public IActionResult CreateRoom()
        {
            return View();
        }

        // POST: Criar nova sala
        [HttpPost]
        public IActionResult CreateRoom(Room room)
        {
            if (!ModelState.IsValid)
            {
                return View(room);
            }

            room.SalaId = RepositoryRoom.Rooms.Count() + 1;
            RepositoryRoom.AddRoom(room);
            return View("CreateRoomComplete", room);
        }

        // GET: Confirmação de criação
        [HttpGet]
        public IActionResult CreateRoomComplete()
        {
            return View();
        }

        // GET: Ver uma Sala pelo Id
        [HttpGet]
        public IActionResult ReadRoom(int id)
        {
            var room = RepositoryRoom.GetRoomById(id);
            if (room == null)
            {
                return NotFound();
            }
            return View(room);
        }

        // GET: Editar sala
        [HttpGet]
        public IActionResult UpdateRoom(int id)
        {
            var room = RepositoryRoom.GetRoomById(id);
            if (room == null)
            {
                return NotFound();
            }
            return View(room);
        }

        // POST: Atualizar sala
        [HttpPost]
        public IActionResult UpdateRoom(Room room)
        {
            if (!ModelState.IsValid)
            {
                return View(room);
            }
            RepositoryRoom.UpdateRoom(room);
            TempData["Mensagem"] = "Sala atualizada com sucesso!";
            return RedirectToAction("ReadRoom", "Room");
        }

        // GET: Eliminar sala
        [HttpGet]
        public IActionResult DeleteRoom(int id)
        {
            var room = RepositoryRoom.GetRoomById(id);
            if (room == null)
            {
                return NotFound();
            }
            return View(room);
        }

        // POST: Confirmar eliminação
        [HttpPost, ActionName("DeleteRoom")]
        public IActionResult ConfirmDeleteRoom(int id)
        {
            RepositoryRoom.RemoveRoom(id);
            return RedirectToAction("ReadRoom");
        }
    }
}