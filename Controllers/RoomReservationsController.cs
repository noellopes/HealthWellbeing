using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using HealthWellbeingRoom.Models;
using HealthWellbeingRoom.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellbeingRoom.Controllers
{
    public class RoomReservationsController : Controller
    {
        private readonly HealthWellbeingDbContext _context;         
        public RoomReservationsController(HealthWellbeingDbContext context)
        {
            _context = context;
        }
        //------------------------------------------------------VERIFICAR EXISTÊNCIA DE RESERVA PARA CONSULTA-------------------------------------------------------------------------------------
        private bool RoomReservationForConsultationExists(int consultationId)
        {
            return _context.RoomReservations.Any(r => r.ConsultationId == consultationId);
        }
        //------------------------------------------------------REGISTAR HISTÓRICO DE RESERVAS-------------------------------------------------------------------------------------
        private async Task RegistrarHistorico(RoomReservation reserva, string finalStatus)
        {
            var historico = new RoomReservationHistory
            {
                RoomReservationId = reserva.RoomReservationId,
                RoomId = reserva.RoomId,
                ConsultationId = reserva.ConsultationId,

                // NOVOS CAMPOS
                ConsultationDate = reserva.ConsultationDate,
                StartHour = reserva.StartHour,
                EndHour = reserva.EndHour,

                ResponsibleName = reserva.ResponsibleName,
                FinalStatus = finalStatus,
                Notes = reserva.Notes,
                RecordedAt = DateTime.Now
            };

            _context.RoomReservationHistory.Add(historico);
            await _context.SaveChangesAsync();
        }


        //------------------------------------------------------MARCAR RESERVA COMO REALIZADA-------------------------------------------------------------------------------------
        public async Task<IActionResult> MarcarComoRealizada(int id)
        {
            // Carregar a reserva com sala e consumíveis
            var reserva = await _context.RoomReservations
                .Include(r => r.Room)
                .ThenInclude(r => r.RoomConsumables)
                .ThenInclude(rc => rc.Consumivel)
                .FirstOrDefaultAsync(r => r.RoomReservationId == id);

            if (reserva == null)
                return NotFound();

            // 1. Atualizar estado da reserva
            reserva.Status = "Realizada";

            // 2. Atualizar estado da sala para "Disponível"
            var disponivelStatus = await _context.RoomStatus
                .FirstOrDefaultAsync(s => s.Name == "Disponível");

            if (disponivelStatus != null && reserva.Room != null)
            {
                reserva.Room.RoomStatusId = disponivelStatus.RoomStatusId;
                _context.Update(reserva.Room);
            }
            // 2.1 Atualizar estado da consulta para "Realizada"
            var consulta = await _context.Consultations
                .FirstOrDefaultAsync(c => c.ConsultationId == reserva.ConsultationId);

            if (consulta != null)
            {
                consulta.Status = "Realizada";
                _context.Update(consulta);
            }

            // 3. Registar consumíveis usados
            foreach (var item in reserva.Room.RoomConsumables)
            {
                var gasto = new ConsumablesExpenses
                {
                    ConsumableId = item.ConsumivelId,
                    RoomId = reserva.RoomId,
                    RoomReservationId = reserva.RoomReservationId,
                    QuantityUsed = 1, // ajustar se necessário
                    UsedAt = DateTime.Now
                };

                _context.ConsumablesExpenses.Add(gasto);

                // Atualizar stock
                item.Consumivel.QuantidadeAtual -= gasto.QuantityUsed;
            }

            await _context.SaveChangesAsync();

            // 4. Mover para histórico
            await RegistrarHistorico(reserva, "Realizada");

            // 5. Remover da tabela principal
            _context.RoomReservations.Remove(reserva);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Reserva marcada como realizada, consumíveis registados e sala libertada.";

            return RedirectToAction(nameof(Index));
        }

        //------------------------------------------------------CANCELAR RESERVA-------------------------------------------------------------------------------------

public async Task<IActionResult> CancelarReserva(int id)
{
    // Carregar a reserva com sala
    var reserva = await _context.RoomReservations
        .Include(r => r.Room)
        .FirstOrDefaultAsync(r => r.RoomReservationId == id);

    if (reserva == null)
        return NotFound();

    // 1. Atualizar estado da reserva
    reserva.Status = "Cancelada";

    // 2. Atualizar estado da sala para "Disponível"
    var disponivelStatus = await _context.RoomStatus
        .FirstOrDefaultAsync(s => s.Name == "Disponível");

    if (disponivelStatus != null && reserva.Room != null)
    {
        reserva.Room.RoomStatusId = disponivelStatus.RoomStatusId;
        _context.Update(reserva.Room);
    }

    // 3. Atualizar a consulta associada → voltar a estado "Pendente" e limpar dados
    var consulta = await _context.Consultations
        .FirstOrDefaultAsync(c => c.ConsultationId == reserva.ConsultationId);

    if (consulta != null)
    {
        consulta.Status = "Pendente";
        consulta.RoomId = null;
        consulta.ConsultationDate = null;
        consulta.StartTime = null;
        consulta.EndTime = null;

        _context.Update(consulta);
    }

    await _context.SaveChangesAsync();

    // 4. Mover para histórico
    await RegistrarHistorico(reserva, "Cancelada");

    // 5. Remover da tabela principal
    _context.RoomReservations.Remove(reserva);
    await _context.SaveChangesAsync();

    TempData["SuccessMessage"] = "Reserva cancelada, sala libertada e consulta revertida para estado pendente.";

    return RedirectToAction(nameof(Index));
}

        //------------------------------------------------------PREENCHER DROPDOWNS (FILTRADO)-------------------------------------------------------------------------------------
        private async Task PreencherDropdowns(
            int? selectedRoomId = null,
            int? selectedSpecialityId = null,
            DateTime? consultationDate = null,
            TimeSpan? startHour = null,
            TimeSpan? endHour = null,
            int? excludeReservationId = null
        )
        {
            string? dateRaw = null;
            string? startRaw = null;
            string? endRaw = null;
            string? excludeRaw = null;

            // Só lê o Form em pedidos com content-type de formulário
            if (Request.HasFormContentType)
            {
                dateRaw = Request.Form["ConsultationDate"].FirstOrDefault();
                startRaw = Request.Form["StartHour"].FirstOrDefault();
                endRaw = Request.Form["EndHour"].FirstOrDefault();
                excludeRaw = Request.Form["RoomReservationId"].FirstOrDefault();
            }

            // Fallback para query string
            dateRaw ??= Request.Query["date"].FirstOrDefault();
            startRaw ??= Request.Query["start"].FirstOrDefault();
            endRaw ??= Request.Query["end"].FirstOrDefault();
            excludeRaw ??= Request.Query["excludeReservationId"].FirstOrDefault();

            // Converter valores
            if (!string.IsNullOrWhiteSpace(dateRaw) && DateTime.TryParse(dateRaw, out var d))
                consultationDate = d.Date;

            if (!string.IsNullOrWhiteSpace(startRaw) && TimeSpan.TryParse(startRaw, out var sh))
                startHour = sh;

            if (!string.IsNullOrWhiteSpace(endRaw) && TimeSpan.TryParse(endRaw, out var eh))
                endHour = eh;

            if (!string.IsNullOrWhiteSpace(excludeRaw) && int.TryParse(excludeRaw, out var ex))
                excludeReservationId = ex;

            // --- Query base de salas (Criado ou Disponível) ---
            IQueryable<Room> roomsQuery = _context.Room
                .AsNoTracking()
                .Include(r => r.RoomStatus)
                .Where(r =>
                    r.RoomStatus.Name == "Criado" ||
                    r.RoomStatus.Name == "Disponível"
                );

            // --- Filtrar salas sem conflitos de horário ---
            if (consultationDate.HasValue && startHour.HasValue && endHour.HasValue)
            {
                var dateVal = consultationDate.Value;
                var startVal = startHour.Value;
                var endVal = endHour.Value;

                roomsQuery = roomsQuery.Where(r =>
                    !_context.RoomReservations
                        .AsNoTracking()
                        .Where(rr =>
                            rr.RoomId == r.RoomId &&
                            rr.ConsultationDate == dateVal &&
                            (excludeReservationId == null || rr.RoomReservationId != excludeReservationId.Value)
                        )
                        .Any(rr =>
                            rr.StartHour < endVal &&
                            startVal < rr.EndHour
                        )
                );
            }

            // --- Carregar salas filtradas ---
            var rooms = await roomsQuery
                .OrderBy(r => r.Name)
                .ToListAsync();

            var roomsSelect = new SelectList(rooms, "RoomId", "Name", selectedRoomId);
            ViewData["RoomId"] = roomsSelect;
            ViewBag.Rooms = roomsSelect;

            // --- Carregar Especialidades ---
            var specialities = await _context.Specialty
                .AsNoTracking()
                .OrderBy(s => s.Name)
                .ToListAsync();

            var specialitySelect = new SelectList(specialities, "SpecialityId", "Name", selectedSpecialityId);
            ViewData["SpecialityId"] = specialitySelect;
            ViewData["SpecialtyId"] = specialitySelect;
            ViewBag.RoomSpecialty = specialitySelect;

            // --- Carregar Consultas ---
            var consultations = await _context.Consultations
                .OrderBy(c => c.ConsultationDate)
                .Select(c => new
                {
                    c.ConsultationId,
                    Display = $"{c.ConsultationId} - {c.DoctorName} ({c.ConsultationDate:dd/MM/yyyy})"
                })
                .ToListAsync();

            ViewBag.Consultations = new SelectList(
                consultations,
                "ConsultationId",
                "Display"
            );
        }
        //------------------------------------------------------INDEX-------------------------------------------------------------------------------------
        [Authorize(Roles = "logisticsTechnician,Administrator")]
        public async Task<IActionResult> Index(
            string? searchResponsible,
            DateTime? searchDate,
            int? searchRoom,
            int page = 1,
            int itemsPerPage = 10)
        {
            var query = _context.RoomReservations
                .AsNoTracking()
                .Where(r => r.Status == "Ativa")
                .Include(r => r.Room)
                    .ThenInclude(room => room.RoomConsumables)
                        .ThenInclude(rc => rc.Consumivel)
                .Include(r => r.Room)
                    .ThenInclude(room => room.LocalizacaoDispMedicoMovel)
                        .ThenInclude(ldm => ldm.MedicalDevice)
                .Include(r => r.Specialty)
                .AsQueryable();

            // Filtros
            if (!string.IsNullOrWhiteSpace(searchResponsible))
            {
                var term = searchResponsible.Trim();
                query = query.Where(r => r.ResponsibleName != null &&
                                         r.ResponsibleName.Contains(term));
            }

            if (searchDate.HasValue)
            {
                var date = searchDate.Value.Date;
                query = query.Where(r => r.ConsultationDate.Date == date);
            }

            if (searchRoom.HasValue && searchRoom.Value > 0)
            {
                query = query.Where(r => r.RoomId == searchRoom.Value);
            }

            query = query
                .OrderBy(r => r.ConsultationDate)
                .ThenBy(r => r.StartHour);

            // Paginação
            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToListAsync();

            // Usa o construtor existente
            var pagination = new RPaginationInfo<RoomReservation>(page, totalItems, itemsPerPage)
            {
                Items = items
            };

            // Dropdown salas + ViewBags
            var rooms = await _context.Room
                .AsNoTracking()
                .OrderBy(r => r.Name)
                .ToListAsync();

            ViewBag.Rooms = new SelectList(rooms, "RoomId", "Name", searchRoom);

            ViewBag.SearchResponsible = searchResponsible;
            ViewBag.SearchDate = searchDate?.ToString("yyyy-MM-dd");
            ViewBag.SearchRoom = searchRoom;

            return View(pagination);
        }

        //------------------------------------------------------DETAILS-------------------------------------------------------------------------------------

        // GET: RoomReservations/Details/5
        [Authorize(Roles = "logisticsTechnician,Administrator")]
        public async Task<IActionResult> Details(int? id, int roomId)
        {
            if (id == null) return NotFound();

            var reservation = await _context.RoomReservations
                .Include(r => r.Room)
                .Include(r => r.Specialty)
                .Include(r => r.Consultation)
                    .ThenInclude(c => c.Specialty)
                .FirstOrDefaultAsync(m => m.RoomReservationId == id);

            if (reservation == null) return NotFound();

            ViewData["RoomName"] = reservation.Room?.Name ?? "—";

            return View(reservation);
        }
        //------------------------------------------------------CREATE-------------------------------------------------------------------------------------

        // GET: RoomReservations/Create
        [Authorize(Roles = "logisticsTechnician")]
        public async Task<IActionResult> Create(int? roomId)
        {
            await PreencherDropdowns(
                selectedRoomId: roomId,
                selectedSpecialityId: null,
                consultationDate: null,
                startHour: null,
                endHour: null,
                excludeReservationId: null
            );

            var model = new RoomReservation
            {
                RoomId = roomId ?? 0
            };

            return View(model);
        }
        //------------------------------------------------------CREATE POST-------------------------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "logisticsTechnician")]
        public async Task<IActionResult> Create(RoomReservation roomReservation)
        {
            // 1. Validar se a consulta existe
            var consulta = await _context.Consultations
                .FirstOrDefaultAsync(c => c.ConsultationId == roomReservation.ConsultationId);

            if (consulta == null)
                ModelState.AddModelError(nameof(roomReservation.ConsultationId), "A consulta indicada não existe.");

            // 2. Validar campos separados de data e hora
            if (roomReservation.ConsultationDate == default)
                ModelState.AddModelError(nameof(roomReservation.ConsultationDate), "A data da consulta é obrigatória.");

            if (roomReservation.StartHour == default)
                ModelState.AddModelError(nameof(roomReservation.StartHour), "A hora de início é obrigatória.");

            if (roomReservation.EndHour == default)
                ModelState.AddModelError(nameof(roomReservation.EndHour), "A hora de fim é obrigatória.");

            if (roomReservation.StartHour >= roomReservation.EndHour)
                ModelState.AddModelError(string.Empty, "A hora de início deve ser anterior à hora de fim.");

            // 3. Validar data/hora no passado
            if (roomReservation.ConsultationDate < DateTime.Today)
                ModelState.AddModelError(nameof(roomReservation.ConsultationDate), "Não é possível agendar para uma data passada.");

            if (roomReservation.ConsultationDate == DateTime.Today &&
                roomReservation.StartHour < DateTime.Now.TimeOfDay)
            {
                ModelState.AddModelError(nameof(roomReservation.StartHour), "A hora de início não pode ser anterior ao momento atual.");
            }

            // 4. Sala obrigatória
            if (roomReservation.RoomId <= 0)
                ModelState.AddModelError(nameof(roomReservation.RoomId), "A sala é obrigatória.");

            // 5A. Impedir duplicação de reservas para a mesma consulta
            bool consultaJaReservada = await _context.RoomReservations
                .AnyAsync(r => r.ConsultationId == roomReservation.ConsultationId && r.Status == "Ativa");

            if (consultaJaReservada)
                ModelState.AddModelError("", "Esta consulta já tem uma reserva associada.");

            // 5B. Validar conflito de reservas (apenas reservas ativas)
            if (roomReservation.RoomId > 0)
            {
                var hasConflict = await _context.RoomReservations
                    .AsNoTracking()
                    .AnyAsync(r =>
                        r.Status == "Ativa" &&
                        r.RoomId == roomReservation.RoomId &&
                        r.ConsultationDate == roomReservation.ConsultationDate &&
                        r.StartHour < roomReservation.EndHour &&
                        roomReservation.StartHour < r.EndHour
                    );

                if (hasConflict)
                    ModelState.AddModelError(string.Empty, "Já existe uma reserva para esta sala no período selecionado.");
            }

            // 6. Se houver erros, recarregar dropdowns
            if (!ModelState.IsValid)
            {
                await PreencherDropdowns(
                    selectedRoomId: roomReservation.RoomId,
                    selectedSpecialityId: roomReservation.SpecialtyId,
                    consultationDate: roomReservation.ConsultationDate,
                    startHour: roomReservation.StartHour,
                    endHour: roomReservation.EndHour,
                    excludeReservationId: null
                );

                return View(roomReservation);
            }

            // 7. Salvar reserva
            _context.Add(roomReservation);
            await _context.SaveChangesAsync();

            // 8. Atualizar estado da sala
            var sala = await _context.Room.FindAsync(roomReservation.RoomId);
            if (sala != null)
            {
                var indisponivelStatus = await _context.RoomStatus
                    .FirstOrDefaultAsync(s => s.Name == "Indisponível");

                if (indisponivelStatus != null)
                {
                    sala.RoomStatusId = indisponivelStatus.RoomStatusId;
                    _context.Update(sala);
                    await _context.SaveChangesAsync();
                }
            }

            // 9. Atualizar consulta
            if (consulta != null)
            {
                consulta.RoomId = roomReservation.RoomId;
                consulta.ConsultationDate = roomReservation.ConsultationDate;
                consulta.StartTime = TimeOnly.FromTimeSpan(roomReservation.StartHour);
                consulta.EndTime = TimeOnly.FromTimeSpan(roomReservation.EndHour);
                consulta.Status = "Agendada";

                _context.Update(consulta);
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "Reserva criada com sucesso.";
            return RedirectToAction(nameof(Index));
        }
        //------------------------------------------------------EDIT-------------------------------------------------------------------------------------
        // GET: RoomReservations/Edit/5
        [Authorize(Roles = "logisticsTechnician")]
        public async Task<IActionResult> Edit(int id)
        {
            var reserva = await _context.RoomReservations
                .Include(r => r.Room)
                .Include(r => r.Specialty)
                .FirstOrDefaultAsync(r => r.RoomReservationId == id);

            if (reserva == null)
                return NotFound();

            // Preenche dropdowns (salas, especialidades, etc.)
            await PreencherDropdowns(
                selectedRoomId: reserva.RoomId,
                selectedSpecialityId: reserva.SpecialtyId,
                consultationDate: reserva.ConsultationDate,
                startHour: reserva.StartHour,
                endHour: reserva.EndHour,
                excludeReservationId: reserva.RoomReservationId
            );

            return View(reserva);
        }


        //------------------------------------------------------EDIT POST-------------------------------------------------------------------------------------
        // POST: RoomReservations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "logisticsTechnician")]
        public async Task<IActionResult> Edit(int id, [Bind(
    "RoomReservationId,ResponsibleName,RoomId,ConsultationId,PatientId,SpecialtyId,ConsultationDate,StartHour,EndHour,Status,Notes"
)] RoomReservation roomReservation)
        {
            if (id != roomReservation.RoomReservationId)
                return NotFound();

            // ===================== VALIDAÇÕES =====================

            if (roomReservation.ConsultationDate == default)
                ModelState.AddModelError(nameof(roomReservation.ConsultationDate), "A data da consulta é obrigatória.");

            if (roomReservation.StartHour == default)
                ModelState.AddModelError(nameof(roomReservation.StartHour), "A hora de início é obrigatória.");

            if (roomReservation.EndHour == default)
                ModelState.AddModelError(nameof(roomReservation.EndHour), "A hora de fim é obrigatória.");

            if (roomReservation.StartHour >= roomReservation.EndHour)
                ModelState.AddModelError(string.Empty, "A hora de início deve ser anterior à hora de fim.");

            if (roomReservation.RoomId <= 0)
                ModelState.AddModelError(nameof(roomReservation.RoomId), "A sala é obrigatória.");

            // ===================== VERIFICA CONFLITOS =====================

            if (ModelState.IsValid)
            {
                var hasConflict = await _context.RoomReservations
                    .AsNoTracking()
                    .AnyAsync(r =>
                        r.RoomId == roomReservation.RoomId &&
                        r.RoomReservationId != roomReservation.RoomReservationId &&
                        r.ConsultationDate == roomReservation.ConsultationDate &&
                        r.StartHour < roomReservation.EndHour &&
                        roomReservation.StartHour < r.EndHour
                    );

                if (hasConflict)
                    ModelState.AddModelError(string.Empty, "Já existe uma reserva para esta sala no período selecionado.");
            }

            // ===================== SE FALHOU VALIDAÇÃO =====================

            if (!ModelState.IsValid)
            {
                await PreencherDropdowns(
                    selectedRoomId: roomReservation.RoomId,
                    selectedSpecialityId: roomReservation.SpecialtyId,
                    consultationDate: roomReservation.ConsultationDate,
                    startHour: roomReservation.StartHour,
                    endHour: roomReservation.EndHour,
                    excludeReservationId: roomReservation.RoomReservationId
                );

                return View(roomReservation);
            }

            // ===================== ATUALIZA =====================

            try
            {
                _context.Update(roomReservation);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Reserva atualizada com sucesso!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomReservationExists(roomReservation.RoomReservationId))
                    return NotFound();

                throw;
            }

            return RedirectToAction(nameof(Index));
        }
        //------------------------------------------------------DELETE-------------------------------------------------------------------------------------
        // GET: RoomReservations/Delete/5
        [Authorize(Roles = "logisticsTechnician")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var roomReservation = await _context.RoomReservations
                .Include(r => r.Room)
                .Include(r => r.Specialty)
                .FirstOrDefaultAsync(m => m.RoomReservationId == id);

            if (roomReservation == null) return NotFound();

            return View(roomReservation);
        }
        //------------------------------------------------------DELETE POST-------------------------------------------------------------------------------------
        // POST: RoomReservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "logisticsTechnician")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var roomReservation = await _context.RoomReservations.FindAsync(id);
            if (roomReservation == null) return NotFound();

            try
            {
                _context.RoomReservations.Remove(roomReservation);
                await _context.SaveChangesAsync();

                // Atualizar estado da sala para Disponível
                var sala = await _context.Room.FindAsync(roomReservation.RoomId);
                if (sala != null)
                {
                    var disponivelStatus = await _context.RoomStatus
                        .FirstOrDefaultAsync(s => s.Name == "Disponível");

                    if (disponivelStatus != null)
                    {
                        sala.RoomStatusId = disponivelStatus.RoomStatusId;
                        _context.Update(sala);
                        await _context.SaveChangesAsync();
                    }
                }
                TempData["SuccessMessage"] = "Reserva eliminada com sucesso.";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Não foi possível eliminar a reserva. Verifique dependências.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            return RedirectToAction(nameof(Index));
        }

        //------------------------------------------------------JSON RESULT-------------------------------------------------------------------------------------
        // Filtrar salas disponíveis para uma data e intervalo de horas
        [HttpGet]
        public JsonResult GetAvailableRooms(DateTime date, TimeSpan start, TimeSpan end)
        {
            // 1. Obter reservas apenas da mesma data
            var reservations = _context.RoomReservations
                .Where(res => res.ConsultationDate == date.Date)
                .ToList();

            // 2. Identificar salas ocupadas no intervalo
            var occupiedRoomIds = reservations
                .Where(res =>
                    res.StartHour < end &&   // começa antes do fim pedido
                    start < res.EndHour      // termina depois do início pedido
                )
                .Select(res => res.RoomId)
                .Distinct()
                .ToList();

            // 3. Salas disponíveis
            var availableRooms = _context.Room
                .Where(room => !occupiedRoomIds.Contains(room.RoomId))
                .Select(r => new
                {
                    Value = r.RoomId.ToString(),
                    Text = r.Name
                })
                .ToList();

            return Json(availableRooms);
        }
        //------------------------------------------------------VIEWS CONSULTATION-------------------------------------------------------------------------------------

        public async Task<IActionResult> Consultation(int? id)
        {
            var consultas = _context.Consultations
                .Include(c => c.Specialty)
                .Include(c => c.Room)
                .ToList();

            return View(consultas);

        }
        //------------------------------------------------------VIEWS CONSUMABLES EXPENSES-------------------------------------------------------------------------------------

        public async Task<IActionResult> ConsumablesExpenses(int? id)
        {
            // Carregar todos os consumíveis gastos, incluindo relações
            var lista = await _context.ConsumablesExpenses
                .Include(e => e.Consumable)
                .Include(e => e.Room)
                .Include(e => e.RoomReservation)
                .OrderByDescending(e => e.UsedAt)
                .ToListAsync();

            return View(lista);
        }
        //------------------------------------------------------HISTORY-------------------------------------------------------------------------------------
        public async Task<IActionResult> History(int roomId)
        {
            var historico = await _context.RoomReservationHistory
                .Include(h => h.Room)
                .Include(h => h.Consultation)
                .Where(h => h.RoomId == roomId)
                .OrderByDescending(h => h.RecordedAt)
                .ToListAsync();

            ViewBag.RoomName = historico.FirstOrDefault()?.Room?.Name ?? "Sala";

            ViewBag.RoomId = roomId;

            return View(historico);
        }
        //----------------------------------------------------------ROOMRESERVATIONLIST---------------------------------------------------------------------------------
        public IActionResult RoomReservationList(int id, int roomId)
        {
            // Escolhe o id da sala (roomId tem prioridade, fallback para id)
            var selectedRoomId = roomId != 0 ? roomId : id;
            if (selectedRoomId == 0)
                return BadRequest("Room id inválido.");

            // Busca a sala (para mostrar nome e permitir voltar aos detalhes)
            var room = _context.Room
                .AsNoTracking()
                .FirstOrDefault(r => r.RoomId == selectedRoomId);

            if (room == null)
                return NotFound("Sala não encontrada.");

            // Enviar dados para a View
            ViewBag.RoomId = selectedRoomId;
            ViewBag.RoomName = room.Name;

            // Busca reservas da sala (ordenadas por data + hora)
            var reservations = _context.RoomReservations
                .AsNoTracking()
                .Include(rr => rr.Room)
                .Include(rr => rr.Specialty)
                .Where(rr => rr.RoomId == selectedRoomId)
                .OrderBy(rr => rr.ConsultationDate)
                .ThenBy(rr => rr.StartHour)
                .ToList();

            return View("RoomReservationList", reservations);
        }
        //----------------------------------------------------------ROOMRESERVATION---------------------------------------------------------------------------------

        [Authorize(Roles = "logisticsTechnician,Administrator")]
        public IActionResult Reservations(int id)
        {
            ViewBag.RoomId = id;
            return View();
        }

        //----------------------------------------------------------ROOMMATERIALS---------------------------------------------------------------------------------

        public async Task<IActionResult> RoomMaterials(int id)
        {
            // Carregar a sala com os materiais associados
            var room = await _context.Room
                .Include(r => r.LocalizacaoDispMedicoMovel)
                    .ThenInclude(l => l.MedicalDevice)
                        .ThenInclude(d => d.TypeMaterial)
                .Include(r => r.RoomConsumables)
                    .ThenInclude(c => c.Consumivel)
                        .ThenInclude(cons => cons!.CategoriaConsumivel)
                .FirstOrDefaultAsync(r => r.RoomId == id);

            if (room == null)
                return NotFound();

            // DEBUG — quantos consumíveis foram carregados pelo EF?
            Console.WriteLine("DEBUG - Consumíveis carregados: " + room.RoomConsumables?.Count);

            // PONTO 1: Mostrar TODOS os dispositivos associados à sala
            var dispositivos = room.LocalizacaoDispMedicoMovel?.ToList();

            // Construir ViewModel
            var RoomMaterialviewModel = new RoomMaterial
            {
                RoomId = room.RoomId,
                RoomName = room.Name,
                MedicalDevices = dispositivos ?? new List<LocationMedDevice>(),
                Consumables = room.RoomConsumables?.ToList() ?? new List<RoomConsumable>()
            };

            Console.WriteLine("DEBUG - RoomConsumables count: " + room.RoomConsumables?.Count);

            foreach (var rc in room.RoomConsumables ?? new List<RoomConsumable>())
            {
                Console.WriteLine($"DEBUG - RC ID={rc.RoomConsumableId}, ConsumivelId={rc.ConsumivelId}, Nome={rc.Consumivel?.Nome}");
            }

            return View(RoomMaterialviewModel);
        }

        //----------------------------------------------------------ROOMMEXISTS---------------------------------------------------------------------------------

        private bool RoomReservationExists(int id)
        {
            return _context.RoomReservations.Any(e => e.RoomReservationId == id);
        }
    }
}
