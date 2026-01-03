using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeingRoom.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeingRoom.Controllers
{
    [Authorize(Roles = "logisticsTechnician,Administrator")]
    public class RoomConsumableController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public RoomConsumableController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: RoomConsumable/Add?roomId=1&consumivelId=5&origin=Consumables
        public async Task<IActionResult> Add(int roomId, int? consumivelId, string? origin)
        {
            var room = await _context.Room.FindAsync(roomId);
            if (room == null) return NotFound();

            ViewBag.RoomId = roomId;
            ViewBag.RoomName = room.Name;
            ViewBag.Origin = origin;

            if (consumivelId.HasValue)
            {
                var cons = await _context.Consumivel.FindAsync(consumivelId.Value);
                if (cons == null) return NotFound();

                ViewBag.ConsumivelName = cons.Nome;
                ViewBag.ConsumivelId = cons.ConsumivelId;

                ViewBag.Consumiveis = new SelectList(
                    new List<Consumivel> { cons },
                    "ConsumivelId",
                    "Nome",
                    cons.ConsumivelId
                );
            }
            else
            {
                ViewBag.Consumiveis = new SelectList(
                    await _context.Consumivel
                        .OrderBy(c => c.Nome)
                        .ToListAsync(),
                    "ConsumivelId",
                    "Nome"
                );
            }

            return View(new RoomConsumable { RoomId = roomId });
        }

        // POST: RoomConsumable/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(
            [Bind("RoomId,ConsumivelId,Quantity,Note")] RoomConsumable rc,
            string? origin)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Consumiveis = new SelectList(
                    await _context.Consumivel.OrderBy(c => c.Nome).ToListAsync(),
                    "ConsumivelId",
                    "Nome",
                    rc.ConsumivelId
                );
                ViewBag.RoomId = rc.RoomId;
                ViewBag.Origin = origin;
                return View(rc);
            }

            // Procura se já existe stock deste consumível na sala
            var existente = await _context.RoomConsumables
                .FirstOrDefaultAsync(x =>
                    x.RoomId == rc.RoomId &&
                    x.ConsumivelId == rc.ConsumivelId &&
                    x.IsCurrent);

            if (existente == null)
            {
                rc.InitialDate = DateTime.Now;
                rc.IsCurrent = true;
                _context.RoomConsumables.Add(rc);
            }
            else
            {
                existente.Quantity += rc.Quantity;
                existente.EndDate = null;
                existente.IsCurrent = true;
                _context.RoomConsumables.Update(existente);
            }

            // Opcional: atualizar stock global do consumível
            var cons = await _context.Consumivel.FindAsync(rc.ConsumivelId);
            if (cons != null)
            {
                cons.QuantidadeAtual -= rc.Quantity;
                if (cons.QuantidadeAtual < 0) cons.QuantidadeAtual = 0;
                _context.Consumivel.Update(cons);
            }

            await _context.SaveChangesAsync();

            var room = await _context.Room.FindAsync(rc.RoomId);
            TempData["SuccessMessage"] =
                $"Consumível adicionado à sala '{room?.Name ?? rc.RoomId.ToString()}' com sucesso.";

            // DECISÃO DE PARA ONDE VOLTAR
            if (!string.IsNullOrWhiteSpace(origin) &&
                origin.Equals("Consumables", StringComparison.OrdinalIgnoreCase))
            {
                // View Consumíveis Associados (RoomsController)
                return RedirectToAction(
                    actionName: "Consumables",
                    controllerName: "Rooms",
                    routeValues: new { id = rc.RoomId });
            }

            if (!string.IsNullOrWhiteSpace(origin) &&
                origin.Equals("RoomMaterials", StringComparison.OrdinalIgnoreCase))
            {
                // View Materiais da Sala (RoomReservationsController)
                return RedirectToAction(
                    actionName: "RoomMaterials",
                    controllerName: "RoomReservations",
                    routeValues: new { id = rc.RoomId });
            }

            // fallback
            return RedirectToAction(
                actionName: "RoomMaterials",
                controllerName: "RoomReservations",
                routeValues: new { id = rc.RoomId });
        }
    }
}
