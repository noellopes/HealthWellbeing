using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using HealthWellbeingRoom.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeingRoom.Controllers
{
    public class RoomsController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public RoomsController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        private async Task PreencherDropdowns()
        {
            ViewBag.RoomTypes = new SelectList(await _context.RoomType.ToListAsync(), "RoomTypeId", "Name");
            ViewBag.RoomLocations = new SelectList(await _context.RoomLocation.ToListAsync(), "RoomLocationId", "Name");
            ViewBag.RoomStatuses = new SelectList(await _context.RoomStatus.ToListAsync(), "RoomStatusId", "Name");
        }
        //------------------------------------------------------INDEX / LIST-------------------------------------------------------------------------------------
        public async Task<IActionResult> Index(string? searchName, string? searchStatus, string? searchSpecialty, string? searchLocation, string? searchRoomType, string searchOpeningTime, string? searchClosingTime, int page = 1)
        {
            var allRooms = await _context.Room
                .Include(r => r.RoomStatus)
                .Include(r => r.RoomLocation)
                .Include(r => r.RoomType)
                .ToListAsync();
            // Aplicar filtros de pesquisa
            // Nome
            if (!string.IsNullOrWhiteSpace(searchName))
            {
                var normalizedSearch = RemoveDiacritics.Normalize(searchName ?? string.Empty);
                allRooms = allRooms.Where(r => RemoveDiacritics.Normalize(r.Name ?? "").Contains(normalizedSearch))
                    .ToList();
            }
            // Status
            if (!string.IsNullOrWhiteSpace(searchStatus) && int.TryParse(searchStatus, out var statusId))
                allRooms = allRooms.Where(r => r.RoomStatusId == statusId)
                    .ToList();
            if (!string.IsNullOrWhiteSpace(searchSpecialty))
            {
                var normalizedSpecialty = RemoveDiacritics.Normalize(searchSpecialty ?? string.Empty);
                allRooms = allRooms
                    .Where(r => RemoveDiacritics.Normalize(r.Specialty ?? "").Contains(normalizedSpecialty))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(searchLocation))
            {
                var normalizedLocation = RemoveDiacritics.Normalize(searchLocation ?? string.Empty);
                allRooms = allRooms
                    .Where(r => r.RoomLocation != null && RemoveDiacritics.Normalize(r.RoomLocation.Name ?? "").Contains(normalizedLocation))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(searchRoomType))
            {
                var normalizedRoomType = RemoveDiacritics.Normalize(searchRoomType.Trim().ToLower());
                allRooms = allRooms
                    .Where(r => RemoveDiacritics.Normalize(r.RoomType?.Name ?? "").ToLower().Contains(normalizedRoomType))
                    .ToList();
            }

            // Verificar se a lista final está vazia
            if (!allRooms.Any())
            {
                var mensagens = new List<string>();
                if (!string.IsNullOrWhiteSpace(searchName))
                    mensagens.Add($"nome \"{searchName}\"");

                if (!string.IsNullOrWhiteSpace(searchSpecialty))
                    mensagens.Add($"especialidade \"{searchSpecialty}\"");

                if (!string.IsNullOrWhiteSpace(searchLocation))
                    mensagens.Add($"localização \"{searchLocation}\"");

                if (!string.IsNullOrWhiteSpace(searchRoomType))
                    mensagens.Add($"tipo de sala \"{searchRoomType}\"");

                ViewBag.ErrorMessage = $"Nenhuma sala encontrada com {string.Join(", ", mensagens)}.";
            }

            // Hora de Abertura
            TimeSpan? openingTime = null;
            if (!string.IsNullOrWhiteSpace(searchOpeningTime) && TimeSpan.TryParse(searchOpeningTime, out var parsedOpening))
            {
                openingTime = parsedOpening; //converte a string para TimeSpan se for válida
            }

            // Hora de Fecho
            TimeSpan? closingTime = null;
            if (!string.IsNullOrWhiteSpace(searchClosingTime) && TimeSpan.TryParse(searchClosingTime, out var parsedClosing))
            {
                closingTime = parsedClosing; //idem para a hora de fecho
            }

            //Validação: abertura < fecho
            if (openingTime.HasValue && closingTime.HasValue)
            {
                if (openingTime.Value >= closingTime.Value)
                {
                    //Caso inválido: abertura maior ou igual ao fecho
                    ViewBag.ErrorMessage = "A hora de abertura deve ser menor que a hora de fecho.";
                    allRooms = new List<Room>(); // devolve lista vazia
                }
                else
                {
                    //Caso válido: aplica filtro combinado
                    allRooms = allRooms
                        .Where(r => r.OpeningTime == openingTime.Value && r.ClosingTime == closingTime.Value)
                        .ToList();

                    if (!allRooms.Any())
                    {
                        //Nenhuma sala encontrada com esse intervalo
                        ViewBag.ErrorMessage = $"Nenhuma sala encontrada com horário de {openingTime.Value:hh\\:mm} até {closingTime.Value:hh\\:mm}.";
                    }
                }
            }
            else
            {
                //se só abertura foi fornecida
                if (openingTime.HasValue)
                {
                    allRooms = allRooms.Where(r => r.OpeningTime == openingTime.Value).ToList();

                    if (!allRooms.Any())
                    {
                        ViewBag.ErrorMessage = $"Nenhuma sala encontrada com hora de abertura às {openingTime.Value:hh\\:mm}.";
                    }
                }

                //Se só fecho foi fornecido
                if (closingTime.HasValue)
                {
                    allRooms = allRooms.Where(r => r.ClosingTime == closingTime.Value).ToList();

                    if (!allRooms.Any())
                    {
                        ViewBag.ErrorMessage = $"Nenhuma sala encontrada com hora de fecho às {closingTime.Value:hh\\:mm}.";
                    }
                }
            }

            // Paginação
            var totalItems = allRooms.Count;
            var pagination = new RPaginationInfo<Room>(page, totalItems);
            pagination.Items = allRooms.OrderBy(r => r.Name ?? "").Skip(pagination.ItemsToSkip).Take(pagination.ItemsPerPage).ToList();
            // Manter os parâmetros de pesquisa na View
            ViewBag.SearchName = searchName;
            ViewBag.SearchStatus = searchStatus;
            ViewBag.SearchSpecialty = searchSpecialty;
            ViewBag.SearchLocation = searchLocation;
            ViewBag.SearchOpeningTime = searchOpeningTime;
            ViewBag.SearchClosingTime = searchClosingTime;
            // Preencher o dropdown de Status
            ViewBag.Status = new SelectList(
                _context.RoomStatus.Select(s => new { Value = s.RoomStatusId.ToString(), Text = s.Name }),
                "Value", "Text", searchStatus);
            // Retornar a View com os dados paginados
            return View(pagination);
        }
        //------------------------------------------------------DETAILS-------------------------------------------------------------------------------------
        public async Task<IActionResult> Details(int? id, bool fromCreation = false)
        {
            if (id == null) return NotFound();

            var room = await _context.Room
                .Include(r => r.RoomStatus)
                .Include(r => r.RoomLocation)
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(m => m.RoomId == id);

            if (room == null) return NotFound();

            ViewBag.FromCreation = fromCreation;
            return View(room);
        }
        //------------------------------------------------------CREATE-------------------------------------------------------------------------------------
        public async Task<IActionResult> Create()
        {
            await PreencherDropdowns();
            return View();
        }
        //--------------------------------------------------------CREATE POST-----------------------------------------------------------------------------------
        [HttpPost] // Indica que este método responde a requisições HTTP POST
        [ValidateAntiForgeryToken] // Protege contra ataques CSRF, validando o token antifalsificação
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

            TempData["SuccessMessage"] = "Sala criada com sucesso!";
            return RedirectToAction("Details", new { id = room.RoomId, fromCreation = true });
        }

        //----------------------------------------------------------EDIT---------------------------------------------------------------------------------
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
        public async Task<IActionResult> Edit(int id, Room room)
        {
            if (id != room.RoomId) return NotFound();

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
            // Redireciona para a ação Details após a edição bem-sucedida
            return RedirectToAction("Details", new { id = room.RoomId, fromCreation = true }); //
        }
        //----------------------------------------------------------DELETE---------------------------------------------------------------------------------

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var room = await _context.Room
                .Include(r => r.RoomStatus)
                .Include(r => r.RoomLocation)
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(m => m.RoomId == id);

            if (room == null) return NotFound();

            return View(room);
        }
        //----------------------------------------------------------EDIT CONFIRMED---------------------------------------------------------------------------------

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Room
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
    }
}