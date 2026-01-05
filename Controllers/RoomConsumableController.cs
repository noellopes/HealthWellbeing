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
        [HttpGet]
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
                if (rc.Quantity < 0) rc.Quantity = 0;

                rc.InitialDate = DateTime.Now;
                rc.IsCurrent = rc.Quantity > 0;
                _context.RoomConsumables.Add(rc);
            }
            else
            {
                existente.Quantity += rc.Quantity;

                if (existente.Quantity <= 0)
                {
                    existente.Quantity = 0;
                    existente.IsCurrent = false;
                    existente.EndDate = DateTime.Now;
                }
                else
                {
                    existente.IsCurrent = true;
                    existente.EndDate = null;
                }

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

            return RedirectToOrigin(origin, rc.RoomId);
        }

        // POST: RoomConsumable/Consumir
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Consumir(int roomConsumableId, int usedQuantity, string? origin)
        {
            if (usedQuantity <= 0)
            {
                TempData["ErrorMessage"] = "A quantidade a consumir deve ser maior que zero.";
                return RedirectToOrigin(origin, null);
            }

            var rc = await _context.RoomConsumables
                .Include(x => x.Consumivel)
                .Include(x => x.Room)
                .FirstOrDefaultAsync(x => x.RoomConsumableId == roomConsumableId && x.IsCurrent);

            if (rc == null)
            {
                TempData["ErrorMessage"] = "Registo de consumível na sala não encontrado.";
                return RedirectToOrigin(origin, null);
            }

            if (usedQuantity > rc.Quantity)
            {
                TempData["ErrorMessage"] =
                    $"Não pode consumir {usedQuantity} unidade(s). Stock disponível: {rc.Quantity}.";
                return RedirectToOrigin(origin, rc.RoomId);
            }

            // Atualizar stock na sala
            rc.Quantity -= usedQuantity;

            if (rc.Quantity <= 0)
                _context.RoomConsumables.Remove(rc);
            else
                _context.RoomConsumables.Update(rc);

            // Atualizar stock global
            var cons = rc.Consumivel;
            if (cons != null)
            {
                cons.QuantidadeAtual -= usedQuantity;
                if (cons.QuantidadeAtual < 0) cons.QuantidadeAtual = 0;
                _context.Consumivel.Update(cons);
            }

            // REGISTAR O GASTO
            var expense = new ConsumablesExpenses
            {
                ConsumableId = rc.ConsumivelId,
                RoomId = rc.RoomId,
                UsedAt = DateTime.Now,
                QuantityUsed = usedQuantity
            };

            _context.ConsumablesExpenses.Add(expense);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                $"Foram consumidas {usedQuantity} unidade(s) de '{rc.Consumivel?.Nome}' na sala '{rc.Room?.Name}'.";

            return RedirectToOrigin(origin, rc.RoomId);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AtualizarUsado([FromBody] RoomUpdateUsedConsumables dto)
        {
            if (dto == null)
                return BadRequest();

            // Procurar o consumível da sala pelo ConsumivelId
            var rc = await _context.RoomConsumables
                .FirstOrDefaultAsync(x => x.ConsumivelId == dto.Id && x.IsCurrent);

            if (rc == null)
                return NotFound();

            // Atualizar o valor real utilizado
            rc.RealUsedQuantity = dto.Quantity;

            await _context.SaveChangesAsync();

            return Json(new { sucesso = true });
        }

        // Helper para decidir para onde voltar
        private IActionResult RedirectToOrigin(string? origin, int? roomId)
        {
            if (roomId == null)
                return RedirectToAction("Index", "RoomReservations");

            if (!string.IsNullOrWhiteSpace(origin) &&
                origin.Equals("Consumables", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction(
                    actionName: "Consumables",
                    controllerName: "Rooms",
                    routeValues: new { id = roomId.Value });
            }

            if (!string.IsNullOrWhiteSpace(origin) &&
                origin.Equals("RoomMaterials", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction(
                    actionName: "RoomMaterials",
                    controllerName: "RoomReservations",
                    routeValues: new { id = roomId.Value });
            }

            // fallback
            return RedirectToAction(
                actionName: "RoomMaterials",
                controllerName: "RoomReservations",
                routeValues: new { id = roomId.Value });
        }
    }
}
