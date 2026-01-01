using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using HealthWellbeingRoom.Models;
using HealthWellbeingRoom.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

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

        private async Task PreencherDropdowns(Room? room = null)
        {
            // Tipos de Sala
            ViewBag.RoomTypes = new SelectList(await _context.RoomType.ToListAsync(), "RoomTypeId", "Name");

            // Localizações
            ViewBag.RoomLocations = new SelectList(await _context.RoomLocation.ToListAsync(), "RoomLocationId", "Name");

            // Status (filtra "Criado" se estiver editando uma sala existente)
            var statusList = await _context.RoomStatus.ToListAsync();
            if (room != null && room.RoomId > 0)
            {
                statusList = statusList.Where(s => s.Name != "Criado").ToList();
            }
            ViewBag.RoomStatuses = new SelectList(statusList, "RoomStatusId", "Name", room?.RoomStatusId);

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
                .Include(r => r.RoomReservations)
                .FirstOrDefaultAsync(m => m.RoomId == id);

            if (room == null) return NotFound();

            ViewBag.FromCreation = fromCreation;

            // Navegação entre salas
            var orderedIds = await _context.Room
                .OrderBy(r => r.RoomId)
                .Select(r => r.RoomId)
                .ToListAsync();

            int currentIndex = orderedIds.IndexOf(id.Value);
            int? previousId = currentIndex > 0 ? orderedIds[currentIndex - 1] : (int?)null;
            int? nextId = currentIndex < orderedIds.Count - 1 ? orderedIds[currentIndex + 1] : (int?)null;

            ViewBag.PreviousRoomId = previousId;
            ViewBag.NextRoomId = nextId;

            // Verificar reserva ativa se a sala estiver indisponível
            if (room.RoomStatus?.Name == "Indisponível")
            {
                var now = DateTime.Now;

                var reservaAtiva = room.RoomReservations
                    .Where(r =>
                        r.ConsultationDate > now.Date ||
                        (r.ConsultationDate == now.Date && r.EndHour > now.TimeOfDay)
                    )
                    .OrderBy(r => r.ConsultationDate)
                    .ThenBy(r => r.StartHour)
                    .FirstOrDefault();

                ViewBag.ReservaAtivaId = reservaAtiva?.RoomReservationId;
            }

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

            await PreencherDropdowns(room);//sala ja criada entao ignora estado "criado"
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
        //----------------------------------------------------------DELETE POST---------------------------------------------------------------------------------

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

        //----------------------------------------------------------EQUIPMENTS---------------------------------------------------------------------------------
        [Authorize(Roles = "logisticsTechnician,Administrator")]
        public IActionResult Equipments(
            int id,
            string? searchName,
            string? searchType,
            string? searchStatus,
            string? searchSerial,
            int page = 1)
        {
            // Verifica se a sala existe
            var room = _context.Room.FirstOrDefault(r => r.RoomId == id);
            if (room == null)
                return NotFound();

            ViewData["RoomName"] = room.Name;
            ViewBag.RoomId = id;

            // Query base: equipamentos associados à sala
            var equipamentosQuery = _context.Equipment
                .Include(e => e.EquipmentType)
                .Include(e => e.EquipmentStatus)
                .Where(e => e.RoomId == id);

            // Aplicar filtros
            if (!string.IsNullOrWhiteSpace(searchName))
                equipamentosQuery = equipamentosQuery.Where(e =>
                    e.Name.ToLower().Contains(searchName.ToLower()));

            if (!string.IsNullOrWhiteSpace(searchType))
                equipamentosQuery = equipamentosQuery.Where(e =>
                    e.EquipmentType.Name.ToLower().Contains(searchType.ToLower()));

            if (!string.IsNullOrWhiteSpace(searchStatus))
                equipamentosQuery = equipamentosQuery.Where(e =>
                    e.EquipmentStatus.Name.ToLower().Contains(searchStatus.ToLower()));

            if (!string.IsNullOrWhiteSpace(searchSerial))
                equipamentosQuery = equipamentosQuery.Where(e =>
                    e.SerialNumber.ToLower().Contains(searchSerial.ToLower()));

            // Executa a query em memória
            var equipamentos = equipamentosQuery.ToList();

            // Paginação segura
            int itemsPerPage = 10;
            int totalItems = equipamentos.Count;
            var pagination = new RPaginationInfo<Equipment>(page, totalItems, itemsPerPage);

            pagination.Items = equipamentos
                .OrderBy(e => e.Name)
                .Skip(pagination.ItemsToSkip)
                .Take(pagination.ItemsPerPage)
                .ToList();

            // Guardar filtros para preencher o formulário
            ViewBag.SearchName = searchName;
            ViewBag.SearchType = searchType;
            ViewBag.SearchStatus = searchStatus;
            ViewBag.SearchSerial = searchSerial;

            return View(pagination);
        }

        //----------------------------------------------------------MEDICAL DEVICES---------------------------------------------------------------------------------

        [Authorize(Roles = "logisticsTechnician,Administrator")]
        public IActionResult MedicalDevices(
            int id,
            string? searchName,
            string? searchType,
            string? searchStatus,
            int page = 1)
        {
            // Verifica se a sala existe
            var room = _context.Room.FirstOrDefault(r => r.RoomId == id);
            if (room == null)
                return NotFound();

            ViewData["RoomName"] = room.Name;
            ViewBag.RoomId = id;

            // Carrega dispositivos médicos associados à sala via LocationMedDevice (apenas localização ativa)
            var dispositivosQuery = _context.MedicalDevices
                .Include(d => d.TypeMaterial)
                .Include(d => d.LocalizacaoDispMedicoMovel)
                    .ThenInclude(l => l.Room)
                .Where(d => d.LocalizacaoDispMedicoMovel
                    .Any(l => l.RoomId == id && l.EndDate == null));

            // Aplicar filtros
            if (!string.IsNullOrWhiteSpace(searchName))
                dispositivosQuery = dispositivosQuery.Where(d =>
                    d.Name.ToLower().Contains(searchName.ToLower()));

            if (!string.IsNullOrWhiteSpace(searchType))
                dispositivosQuery = dispositivosQuery.Where(d =>
                    d.TypeMaterial.Name.ToLower().Contains(searchType.ToLower()));

            if (!string.IsNullOrWhiteSpace(searchStatus))
                dispositivosQuery = dispositivosQuery.Where(d =>
                    d.CurrentStatus.ToLower().Contains(searchStatus.ToLower()));

            // Executa a query e transforma em List<T>
            var dispositivos = dispositivosQuery.ToList();

            // Paginação segura (igual ao Consumables)
            int itemsPerPage = 10;
            itemsPerPage = itemsPerPage > 0 ? itemsPerPage : 10;

            int totalItems = dispositivos.Count;
            var pagination = new RPaginationInfo<MedicalDevice>(page, totalItems, itemsPerPage);

            pagination.Items = dispositivos
                .OrderBy(d => d.Name)
                .Skip(pagination.ItemsToSkip)
                .Take(pagination.ItemsPerPage)
                .ToList();

            // Guardar filtros para manter no formulário
            ViewBag.SearchName = searchName;
            ViewBag.SearchType = searchType;
            ViewBag.SearchStatus = searchStatus;

            return View(pagination);
        }

        //----------------------------------------------------------CONSUMABLES---------------------------------------------------------------------------------
        [Authorize(Roles = "logisticsTechnician,Administrator")]
        public async Task<IActionResult> Consumables(
            int id,
            string? searchName,
            string? searchCategory,
            int? searchMinQuantity,
            int? searchMaxQuantity,
            int page = 1)
        {
            ViewBag.RoomId = id;

            var room = await _context.Room.FirstOrDefaultAsync(r => r.RoomId == id);
            if (room == null)
                return NotFound();

            ViewData["RoomName"] = room.Name;
            ViewData["Title"] = "Consumíveis associados à sala";

            // Consumíveis associados à sala via ZonaArmazenamento
            var query = _context.ZonaArmazenamento
                .Include(z => z.Consumivel)
                    .ThenInclude(c => c.CategoriaConsumivel)
                .Where(z => z.RoomId == id)
                .AsQueryable();

            // Aplicar filtros
            if (!string.IsNullOrEmpty(searchName))
                query = query.Where(z => z.Consumivel.Nome.Contains(searchName));

            if (!string.IsNullOrEmpty(searchCategory))
                query = query.Where(z => z.Consumivel.CategoriaConsumivel.Nome.Contains(searchCategory));

            if (searchMinQuantity.HasValue)
                query = query.Where(z => z.QuantidadeAtual >= searchMinQuantity.Value);

            if (searchMaxQuantity.HasValue)
                query = query.Where(z => z.QuantidadeAtual <= searchMaxQuantity.Value);

            // Paginação
            int itemsPerPage = 10;
            int totalItems = await query.CountAsync();
            var pagination = new RPaginationInfo<ZonaArmazenamento>(page, totalItems, itemsPerPage);

            pagination.Items = await query
                .OrderBy(z => z.Consumivel.Nome)
                .Skip(pagination.ItemsToSkip)
                .Take(pagination.ItemsPerPage)
                .ToListAsync();

            // Manter filtros na ViewBag
            ViewBag.SearchName = searchName;
            ViewBag.SearchCategory = searchCategory;
            ViewBag.SearchMinQuantity = searchMinQuantity;
            ViewBag.SearchMaxQuantity = searchMaxQuantity;

            return View(pagination);
        }
    }
}