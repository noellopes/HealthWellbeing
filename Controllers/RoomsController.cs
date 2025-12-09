using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using HealthWellbeingRoom.Models;
using HealthWellbeingRoom.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using HealthWellbeingRoom.Models;

namespace HealthWellbeingRoom.Controllers
{

    public class RoomsController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public RoomsController(HealthWellbeingDbContext context)
        {
            _context = context;
        }
        //------------------------------------------------------PREENCHER DROPDOWNS-------------------------------------------------------------------------------------

        private async Task PreencherDropdowns()
        {
            // Tipos de Sala
            ViewBag.RoomTypes = new SelectList(await _context.RoomType.ToListAsync(), "RoomTypeId", "Name");
            // Localizações
            ViewBag.RoomLocations = new SelectList(await _context.RoomLocation.ToListAsync(), "RoomLocationId", "Name");
            // Status
            ViewBag.RoomStatuses = new SelectList(await _context.RoomStatus.ToListAsync(), "RoomStatusId", "Name");
            // Especialidades
            ViewBag.RoomSpecialty = new SelectList(await _context.Specialty.ToListAsync(), "SpecialtyId", "Name");
        }
        //------------------------------------------------------INDEX / LIST-------------------------------------------------------------------------------------
        [Authorize(Roles = "logisticsTechnician,Administrator")]
        public async Task<IActionResult> Index(
            string? searchName,
            string? searchStatus,
            string? searchSpecialty,
            string? searchLocation,
            string? searchRoomType,
            string? searchOpeningTime,
            string? searchClosingTime,
            int page = 1)
        {
            // Carregamento inicial com Includes
            var allRooms = await _context.Room
                .Include(r => r.Specialty)
                .Include(r => r.RoomStatus)
                .Include(r => r.RoomLocation)
                .Include(r => r.RoomType)
                .ToListAsync();

            // Filtro por nome
            if (!string.IsNullOrWhiteSpace(searchName))
            {
                var normalizedSearch = RemoveDiacritics.Normalize(searchName);
                allRooms = allRooms
                    .Where(r => RemoveDiacritics.Normalize(r.Name ?? "").Contains(normalizedSearch))
                    .ToList();
            }
            // Filtro por status
            if (!string.IsNullOrWhiteSpace(searchStatus) && int.TryParse(searchStatus, out var statusId))
                allRooms = allRooms.Where(r => r.RoomStatusId == statusId).ToList();

            // Filtro por especialidade
            if (!string.IsNullOrWhiteSpace(searchSpecialty) && int.TryParse(searchSpecialty, out var specialtyId))
                allRooms = allRooms.Where(r => r.SpecialtyId == specialtyId).ToList();

            // Filtro por localização
            if (!string.IsNullOrWhiteSpace(searchLocation))
            {
                var normalizedLocation = RemoveDiacritics.Normalize(searchLocation.Trim().ToLower());
                allRooms = allRooms
                    .Where(r => RemoveDiacritics.Normalize(r.RoomLocation?.Name ?? "").ToLower().Contains(normalizedLocation))
                    .ToList();
            }

            // Filtro por tipo de sala
            // Filtro por tipo de sala (comparação por Id)
            if (!string.IsNullOrWhiteSpace(searchRoomType) && int.TryParse(searchRoomType, out int roomTypeId))
            {
                allRooms = allRooms
                    .Where(r => r.RoomTypeId == roomTypeId)
                    .ToList();
            }

            // Filtro por horário
            TimeSpan? openingTime = null;
            TimeSpan? closingTime = null;

            if (!string.IsNullOrWhiteSpace(searchOpeningTime) && TimeSpan.TryParse(searchOpeningTime, out var parsedOpening))
                openingTime = parsedOpening;

            if (!string.IsNullOrWhiteSpace(searchClosingTime) && TimeSpan.TryParse(searchClosingTime, out var parsedClosing))
                closingTime = parsedClosing;

            if (openingTime.HasValue && closingTime.HasValue)
            {
                if (openingTime.Value >= closingTime.Value)
                {
                    ViewBag.ErrorMessage = "A hora de abertura deve ser menor que a hora de fecho.";
                    allRooms = new List<Room>();
                }
                else
                {
                    allRooms = allRooms
                        .Where(r => r.OpeningTime == openingTime.Value && r.ClosingTime == closingTime.Value)
                        .ToList();

                    if (!allRooms.Any())
                        ViewBag.ErrorMessage = $"Nenhuma sala encontrada com horário de {openingTime.Value:hh\\:mm} até {closingTime.Value:hh\\:mm}.";
                }
            }
            else
            {
                if (openingTime.HasValue)
                {
                    allRooms = allRooms.Where(r => r.OpeningTime == openingTime.Value).ToList();
                    if (!allRooms.Any())
                        ViewBag.ErrorMessage = $"Nenhuma sala encontrada com hora de abertura às {openingTime.Value:hh\\:mm}.";
                }

                if (closingTime.HasValue)
                {
                    allRooms = allRooms.Where(r => r.ClosingTime == closingTime.Value).ToList();
                    if (!allRooms.Any())
                        ViewBag.ErrorMessage = $"Nenhuma sala encontrada com hora de fecho às {closingTime.Value:hh\\:mm}.";
                }
            }

            // Mensagem se nenhum resultado
            if (!allRooms.Any() && ViewBag.ErrorMessage == null)
            {
                var mensagens = new List<string>();
                if (!string.IsNullOrWhiteSpace(searchName)) mensagens.Add($"nome \"{searchName}\"");
                if (!string.IsNullOrWhiteSpace(searchSpecialty)) mensagens.Add($"especialidade \"{searchSpecialty}\"");
                if (!string.IsNullOrWhiteSpace(searchLocation)) mensagens.Add($"localização \"{searchLocation}\"");
                if (!string.IsNullOrWhiteSpace(searchRoomType)) mensagens.Add($"tipo de sala \"{searchRoomType}\"");

                ViewBag.ErrorMessage = $"Nenhuma sala encontrada com {string.Join(", ", mensagens)}.";
            }

            // Paginação
            const int itemsPerPage = 10;
            var totalItems = allRooms.Count;
            var pagination = new RPaginationInfo<Room>(page, totalItems, itemsPerPage)
            {
                Items = allRooms
                    .OrderBy(r => r.Name ?? "")
                    .Skip((page - 1) * itemsPerPage)
                    .Take(itemsPerPage)
                    .ToList()
            };

            // Manter filtros na View
            ViewBag.SearchName = searchName;
            ViewBag.SearchStatus = searchStatus;
            ViewBag.SearchSpecialty = searchSpecialty;
            ViewBag.SearchLocation = searchLocation;
            ViewBag.SearchRoomType = searchRoomType;
            ViewBag.SearchOpeningTime = searchOpeningTime;
            ViewBag.SearchClosingTime = searchClosingTime;

            // Dropdowns
            ViewBag.Status = new SelectList(
                _context.RoomStatus.Select(s => new { Value = s.RoomStatusId.ToString(), Text = s.Name }),
                "Value", "Text", searchStatus);

            ViewBag.RoomSpecialty = new SelectList(
                _context.Specialty.Select(s => new { Value = s.SpecialtyId.ToString(), Text = s.Name }),
                "Value", "Text", searchSpecialty);

            ViewBag.RoomTypes = new SelectList(
                _context.RoomType.Select(t => new { Value = t.RoomTypeId.ToString(), Text = t.Name }),
                "Value", "Text", searchRoomType);

            ViewBag.RoomLocations = new SelectList(
                _context.RoomLocation.Select(l => new { Value = l.RoomLocationId.ToString(), Text = l.Name }),
                "Value", "Text", searchLocation);

            return View(pagination);
        }
        //------------------------------------------------------DETAILS-------------------------------------------------------------------------------------
        [Authorize(Roles = "logisticsTechnician,Administrator")]
        public async Task<IActionResult> Details(int? id, bool fromCreation = false)
        {
            if (id == null) return NotFound();

            var room = await _context.Room
                .Include(r => r.Specialty)
                .Include(r => r.RoomStatus)
                .Include(r => r.RoomLocation)
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(m => m.RoomId == id);

            if (room == null) return NotFound();

            ViewBag.FromCreation = fromCreation;
            return View(room);
        }

        //------------------------------------------------------CREATE-------------------------------------------------------------------------------------
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create()
        {
            await PreencherDropdowns();
            return View();
        }

        //--------------------------------------------------------CREATE POST-----------------------------------------------------------------------------------
        [HttpPost] // Indica que este método responde a requisições HTTP POST
        [ValidateAntiForgeryToken] // Protege contra ataques CSRF, validando o token antifalsificação
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create(Room room) // Método assíncrono que recebe um objeto Room como parâmetro
        {
            if (room == null) return NotFound(); // Se o objeto recebido for nulo, retorna erro 404

            // Validação: Abertura < Fecho
            if (room.OpeningTime >= room.ClosingTime)
            {
                ModelState.AddModelError(string.Empty, "A hora de abertura deve ser menor que a hora de encerramento.");
            }

            // Validação: Nome duplicado
            var nomeNormalizado = room.Name?.Trim().ToLower() ?? string.Empty;
            var salaExistente = await _context.Room.AnyAsync(r => (r.Name ?? "").Trim().ToLower() == nomeNormalizado);
            if (salaExistente)
            {
                ModelState.AddModelError(nameof(room.Name), "Já existe uma sala com este nome, por favor insira outro nome.");
            }

            // Validação: Status "Criado" existe
            var statusCriado = await _context.RoomStatus.FirstOrDefaultAsync(s => s.Name == "Criado");
            if (statusCriado == null)
            {
                ModelState.AddModelError(nameof(room.RoomStatusId), "Não foi possível atribuir o status inicial. Verifique se existe o status \"Criado\".");
            }

            // Validação: Especialidade obrigatória para certos tipos de sala
            var tipoSala = await _context.RoomType.FindAsync(room.RoomTypeId);
            if (tipoSala != null)
            {
                var tiposComEspecialidadeObrigatoria = new[]
                {
            "Consultas",
            "Unidade de Terapia Intensiva (UTI)",
            "Centro Cirúrgico",
            "Exames",
            "Recuperação Pós-Operatória",
            "Emergência"
        };

                if (tiposComEspecialidadeObrigatoria.Contains(tipoSala.Name) && room.SpecialtyId == null)
                {
                    ModelState.AddModelError(nameof(room.SpecialtyId), "Este tipo de sala exige uma especialidade.");
                }
            }

            // Se houver qualquer erro de validação, retorna à View com os dropdowns preenchidos
            if (!ModelState.IsValid)
            {
                await PreencherDropdowns();
                return View(room);
            }

            // Atribui o status "Criado"
            room.RoomStatusId = statusCriado!.RoomStatusId;

            // Salva no banco
            _context.Add(room);
            await _context.SaveChangesAsync();

            // Mensagem de sucesso na criação
            TempData["SuccessMessage"] = "Sala criada com sucesso!";

            // Redireciona para a ação Details após a criação bem-sucedida
            return RedirectToAction("Details", new { id = room.RoomId, fromCreation = true });
        }

        //----------------------------------------------------------EDIT---------------------------------------------------------------------------------
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var room = await _context.Room.FindAsync(id);
            if (room == null) return NotFound();

            await PreencherDropdowns();
            return View(room);
        }

        //----------------------------------------------------------EDIT POST---------------------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id, Room room)
        {
            if (id != room.RoomId) return NotFound();

            // Validação: Especialidade obrigatória para certos tipos de sala
            var tipoSala = await _context.RoomType.FindAsync(room.RoomTypeId);
            if (tipoSala != null)
            {
                var tiposComEspecialidadeObrigatoria = new[]
                {
            "Consultas",
            "Unidade de Terapia Intensiva (UTI)",
            "Centro Cirúrgico",
            "Exames",
            "Recuperação Pós-Operatória",
            "Emergência"
        };

                if (tiposComEspecialidadeObrigatoria.Contains(tipoSala.Name) && room.SpecialtyId == null)
                {
                    ModelState.AddModelError(nameof(room.SpecialtyId), "Este tipo de sala exige uma especialidade.");
                }
            }

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    Console.WriteLine(error.ErrorMessage);

                await PreencherDropdowns();
                return View(room);
            }

            try
            {
                _context.Update(room);
                await _context.SaveChangesAsync();
                TempData["SuccessMessageEdit"] = $"Sala \"{room.Name ?? "(sem nome)"}\" atualizada com sucesso!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomExists(room.RoomId)) return NotFound();
                else throw;
            }

            // Mensagem de sucesso na edição
            TempData["SuccessMessage"] = "Sala editada com sucesso!";

            // Redireciona para a ação Details após a edição bem-sucedida
            return RedirectToAction("Details", new { id = room.RoomId, fromCreation = true });
        }
        //----------------------------------------------------------DELETE---------------------------------------------------------------------------------
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var room = await _context.Room
                .Include(r => r.Specialty)
                .Include(r => r.RoomStatus)
                .Include(r => r.RoomLocation)
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(m => m.RoomId == id);

            if (room == null) return NotFound();

            return View(room);
        }
        //----------------------------------------------------------DELETE---------------------------------------------------------------------------------

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Room
                .Include(r => r.Specialty)
                .Include(r => r.Equipments)
                .Include(r => r.LocalizacaoDispMedicoMovel)
                .FirstOrDefaultAsync(r => r.RoomId == id);

            if (room == null)
            {
                TempData["ErrorMessageDelete"] = "Sala não encontrada.";
                return View("Delete");
            }

            if (room.Equipments?.Any() == true)
            {
                TempData["ErrorMessageDelete"] = $"A sala \"{room.Name ?? "(sem nome)"}\" não pode ser eliminada porque possui equipamentos.";
                return View("Delete", room);
            }

            if (room.LocalizacaoDispMedicoMovel?.Any() == true)
            {
                TempData["errorMessageDeleteDispositivo"] = $"A sala \"{room.Name ?? "(sem nome)"}\" não pode ser eliminada porque possui dispositivos médicos.";
                return View("Delete", room);
            }

            _context.Room.Remove(room);
            await _context.SaveChangesAsync();
            TempData["SuccessMessageDelete"] = $"Sala \"{room.Name ?? "(sem nome)"}\" eliminada com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        //----------------------------------------------------------ROOM EXISTS---------------------------------------------------------------------------------

        private bool RoomExists(int id)
        {
            return _context.Room.Any(e => e.RoomId == id);
        }

        //----------------------------------------------------------HISTORY---------------------------------------------------------------------------------
        [Authorize(Roles = "logisticsTechnician,Administrator")]
        public async Task<IActionResult> History(int id)
        {
            ViewBag.RoomId = id;

            List<RoomHistory> histories = await _context.RoomHistories
                .Where(h => h.RoomId == id)
                .ToListAsync();

            return View(histories);
        }

        //----------------------------------------------------------EQUIPMENTS---------------------------------------------------------------------------------

        [Authorize(Roles = "logisticsTechnician,Administrator")]
        public IActionResult Equipments(int id)
        {
            ViewBag.RoomId = id;
            return View();
        }

        //----------------------------------------------------------MEDICAL DEVICES---------------------------------------------------------------------------------


        [Authorize(Roles = "logisticsTechnician,Administrator")]
        public IActionResult MedicalDevices(int id)
        {
            ViewBag.RoomId = id;
            return View();
        }

        //----------------------------------------------------------RESERVATIONS---------------------------------------------------------------------------------


        [Authorize(Roles = "logisticsTechnician,Administrator")]
        public IActionResult Reservations(int id)
        {
            ViewBag.RoomId = id;
            return View();
        }


    }
}