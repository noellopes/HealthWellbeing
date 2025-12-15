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
                //.Include(r => r.Equipments)
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
        public IActionResult History(
            int id,
            int? searchHistoryId,
            int? searchResponsibleId,
            DateTime? searchStartDate,
            DateTime? searchEndDate,
            int page = 1)
        {
            ViewBag.RoomId = id;

            var room = _context.Room.FirstOrDefault(r => r.RoomId == id);
            if (room == null)
                return NotFound();

            ViewData["RoomName"] = room.Name;
            ViewData["Title"] = "Histórico de utilização de Sala";


            // Dados fictícios
            var fakeData = new List<RoomHistory>
            {
                new RoomHistory { RoomHistoryId = 10, StartDate = DateTime.Now.AddHours(-2), EndDate = DateTime.Now, Responsible = "João Silva", ResponsibleId = 1, Note = "Limpeza completa" },
                new RoomHistory { RoomHistoryId = 20, StartDate = DateTime.Now.AddDays(-1), EndDate = DateTime.Now.AddDays(-1).AddHours(1), Responsible = "Maria Costa", ResponsibleId = 2, Note = "Inspeção técnica" },
                new RoomHistory { RoomHistoryId = 30, StartDate = DateTime.Now.AddDays(-3), EndDate = DateTime.Now.AddDays(-3).AddHours(2), Responsible = "Carlos Mendes", ResponsibleId = 3, Note = "Revisão elétrica" },
                new RoomHistory { RoomHistoryId = 40, StartDate = DateTime.Now.AddDays(-5), EndDate = DateTime.Now.AddDays(-5).AddHours(1), Responsible = "Ana Ferreira", ResponsibleId = 4, Note = "Troca de lâmpadas" },
                new RoomHistory { RoomHistoryId = 50, StartDate = DateTime.Now.AddDays(-7), EndDate = DateTime.Now.AddDays(-7).AddHours(3), Responsible = "Pedro Santos", ResponsibleId = 5, Note = "Limpeza de ar condicionado" },
                new RoomHistory { RoomHistoryId = 60, StartDate = DateTime.Now.AddDays(-10), EndDate = DateTime.Now.AddDays(-10).AddHours(2), Responsible = "Sofia Almeida", ResponsibleId = 6, Note = "Inspeção de segurança" },
                new RoomHistory { RoomHistoryId = 70, StartDate = DateTime.Now.AddDays(-12), EndDate = DateTime.Now.AddDays(-12).AddHours(1), Responsible = "Ricardo Lopes", ResponsibleId = 7, Note = "Revisão de portas" },
                new RoomHistory { RoomHistoryId = 80, StartDate = DateTime.Now.AddDays(-15), EndDate = DateTime.Now.AddDays(-15).AddHours(2), Responsible = "Helena Costa", ResponsibleId = 8, Note = "Verificação de extintores" },
                new RoomHistory { RoomHistoryId = 90, StartDate = DateTime.Now.AddDays(-18), EndDate = DateTime.Now.AddDays(-18).AddHours(1), Responsible = "Miguel Rocha", ResponsibleId = 9, Note = "Revisão de cablagem" },
                new RoomHistory { RoomHistoryId = 100, StartDate = DateTime.Now.AddDays(-20), EndDate = DateTime.Now.AddDays(-20).AddHours(2), Responsible = "Patrícia Gomes", ResponsibleId = 10, Note = "Limpeza geral" },
                new RoomHistory { RoomHistoryId = 110, StartDate = DateTime.Now.AddDays(-22), EndDate = DateTime.Now.AddDays(-22).AddHours(1), Responsible = "Tiago Martins", ResponsibleId = 11, Note = "Inspeção técnica" },
                new RoomHistory { RoomHistoryId = 120, StartDate = DateTime.Now.AddDays(-25), EndDate = DateTime.Now.AddDays(-25).AddHours(2), Responsible = "Beatriz Silva", ResponsibleId = 12, Note = "Troca de filtros" },
                new RoomHistory { RoomHistoryId = 130, StartDate = DateTime.Now.AddDays(-27), EndDate = DateTime.Now.AddDays(-27).AddHours(1), Responsible = "André Carvalho", ResponsibleId = 13, Note = "Revisão de iluminação" },
                new RoomHistory { RoomHistoryId = 140, StartDate = DateTime.Now.AddDays(-30), EndDate = DateTime.Now.AddDays(-30).AddHours(2), Responsible = "Mariana Ribeiro", ResponsibleId = 14, Note = "Limpeza técnica" },
                new RoomHistory { RoomHistoryId = 150, StartDate = DateTime.Now.AddDays(-32), EndDate = DateTime.Now.AddDays(-32).AddHours(1), Responsible = "João Costa", ResponsibleId = 15, Note = "Inspeção final" }
            };

            // Aplicar filtros
            if (searchHistoryId.HasValue)
                fakeData = fakeData.Where(h => h.RoomHistoryId == searchHistoryId.Value).ToList();

            if (searchResponsibleId.HasValue)
                fakeData = fakeData.Where(h => h.ResponsibleId == searchResponsibleId.Value).ToList();

            if (searchStartDate.HasValue)
                fakeData = fakeData.Where(h => h.StartDate >= searchStartDate.Value).ToList();

            if (searchEndDate.HasValue)
                fakeData = fakeData.Where(h => h.EndDate <= searchEndDate.Value).ToList();

            // Paginação segura
            int itemsPerPage = 10;
            var pagination = new RPaginationInfo<RoomHistory>(page, fakeData.Count, itemsPerPage);

            pagination.Items = fakeData
                .OrderByDescending(h => h.StartDate)
                .Skip(pagination.ItemsToSkip)
                .Take(pagination.ItemsPerPage)
                .ToList();

            // Guardar filtros para preencher o formulário
            ViewBag.SearchHistoryId = searchHistoryId;
            ViewBag.SearchResponsibleId = searchResponsibleId;
            ViewBag.SearchStartDate = searchStartDate?.ToString("yyyy-MM-dd");
            ViewBag.SearchEndDate = searchEndDate?.ToString("yyyy-MM-dd");

            return View(pagination);
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


        //----------------------------------------------------------RESERVATIONS---------------------------------------------------------------------------------


        [Authorize(Roles = "logisticsTechnician,Administrator")]
        public IActionResult Reservations(int id)
        {
            ViewBag.RoomId = id;
            return View();
        }

        //----------------------------------------------------------CONSUMABLES---------------------------------------------------------------------------------


        [Authorize(Roles = "logisticsTechnician,Administrator")]
        public IActionResult Consumables(
            int id,
            string? searchName,
            string? searchCategory,
            int? searchMinQuantity,
            int? searchMaxQuantity,
            int page = 1){
            ViewBag.RoomId = id;

            var room = _context.Room.FirstOrDefault(r => r.RoomId == id);
            if (room == null)
                return NotFound();

            ViewData["RoomName"] = room.Name;
            ViewData["Title"] = "Consumíveis associados à sala";

            // Dados fictícios
            var consumables = new List<RoomConsumable>
            {
                new RoomConsumable { RoomConsumableId = 1, Name = "Álcool gel", Quantity = 40, Note = "Uso diário nas entradas", Category = "Higiene" },
                new RoomConsumable { RoomConsumableId = 2, Name = "Máscaras descartáveis", Quantity = 200, Note = "Caixa com 50 unidades", Category = "Proteção" },
                new RoomConsumable { RoomConsumableId = 3, Name = "Luvas de látex", Quantity = 120, Note = "Tamanhos variados", Category = "Proteção" },
                new RoomConsumable { RoomConsumableId = 4, Name = "Lenços de papel", Quantity = 75, Note = "Pacotes individuais", Category = "Higiene" },
                new RoomConsumable { RoomConsumableId = 5, Name = "Sabonete líquido", Quantity = 30, Note = "Frascos de 500ml", Category = "Higiene" },
                new RoomConsumable { RoomConsumableId = 6, Name = "Toalhas de papel", Quantity = 90, Note = "Rolos grandes", Category = "Higiene" },
                new RoomConsumable { RoomConsumableId = 7, Name = "Desinfetante multiuso", Quantity = 25, Note = "Para limpeza geral", Category = "Limpeza" },
                new RoomConsumable { RoomConsumableId = 8, Name = "Água oxigenada", Quantity = 15, Note = "Frascos de 1L", Category = "Medicinal" },
                new RoomConsumable { RoomConsumableId = 9, Name = "Gaze estéril", Quantity = 60, Note = "Pacotes com 10 unidades", Category = "Medicinal" },
                new RoomConsumable { RoomConsumableId = 10, Name = "Curativos adesivos", Quantity = 150, Note = "Diversos tamanhos", Category = "Medicinal" },
                new RoomConsumable { RoomConsumableId = 11, Name = "Termômetro digital", Quantity = 10, Note = "Uso em triagem", Category = "Equipamento" },
                new RoomConsumable { RoomConsumableId = 12, Name = "Spray antisséptico", Quantity = 35, Note = "Frascos de bolso", Category = "Higiene" },
                new RoomConsumable { RoomConsumableId = 13, Name = "Soro fisiológico", Quantity = 50, Note = "Ampolas de 10ml", Category = "Medicinal" },
                new RoomConsumable { RoomConsumableId = 14, Name = "Fita adesiva médica", Quantity = 20, Note = "Rolos de 5m", Category = "Medicinal" },
                new RoomConsumable { RoomConsumableId = 15, Name = "Protetor facial", Quantity = 12, Note = "Visores reutilizáveis", Category = "Proteção" }
            };

            // Aplicar filtros
            if (!string.IsNullOrEmpty(searchName))
                consumables = consumables
                    .Where(c => c.Name.Contains(searchName, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            if (!string.IsNullOrEmpty(searchCategory))
                consumables = consumables
                    .Where(c => c.Category.Contains(searchCategory, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            if (searchMinQuantity.HasValue)
                consumables = consumables
                    .Where(c => c.Quantity >= searchMinQuantity.Value)
                    .ToList();

            if (searchMaxQuantity.HasValue)
                consumables = consumables
                    .Where(c => c.Quantity <= searchMaxQuantity.Value)
                    .ToList();

            // Paginação
            int itemsPerPage = 10;
            var pagination = new RPaginationInfo<RoomConsumable>(page, consumables.Count, itemsPerPage);

            pagination.Items = consumables
                .Skip(pagination.ItemsToSkip)
                .Take(pagination.ItemsPerPage);

            // Manter filtros na ViewBag para preencher o formulário
            ViewBag.SearchName = searchName;
            ViewBag.SearchCategory = searchCategory;
            ViewBag.SearchMinQuantity = searchMinQuantity;
            ViewBag.SearchMaxQuantity = searchMaxQuantity;

            return View(pagination);
        }

        //----------------------------------------------------------HISTORYDETAILS---------------------------------------------------------------------------------
        [Authorize(Roles = "logisticsTechnician,Administrator")]
        public IActionResult HistoryDetails(int id, int roomId)
        {
            // Buscar a sala
            var room = _context.Room.FirstOrDefault(r => r.RoomId == roomId);
            if (room == null)
                return NotFound();

            ViewBag.RoomId = roomId;
            ViewData["RoomName"] = room.Name;

            // Dados fictícios (mesmos do método History, mas agora com RoomId preenchido)
            var fakeData = new List<RoomHistory>
            {
                new RoomHistory { RoomHistoryId = 10, RoomId = roomId, StartDate = DateTime.Now.AddHours(-2), EndDate = DateTime.Now, Responsible = "João Silva", ResponsibleId = 1, Note = "Limpeza completa" },
                new RoomHistory { RoomHistoryId = 20, RoomId = roomId, StartDate = DateTime.Now.AddDays(-1), EndDate = DateTime.Now.AddDays(-1).AddHours(1), Responsible = "Maria Costa", ResponsibleId = 2, Note = "Inspeção técnica" },
                new RoomHistory { RoomHistoryId = 30, RoomId = roomId, StartDate = DateTime.Now.AddDays(-3), EndDate = DateTime.Now.AddDays(-3).AddHours(2), Responsible = "Carlos Mendes", ResponsibleId = 3, Note = "Revisão elétrica" },
                new RoomHistory { RoomHistoryId = 40, RoomId = roomId, StartDate = DateTime.Now.AddDays(-5), EndDate = DateTime.Now.AddDays(-5).AddHours(1), Responsible = "Ana Ferreira", ResponsibleId = 4, Note = "Troca de lâmpadas" },
                new RoomHistory { RoomHistoryId = 50, RoomId = roomId, StartDate = DateTime.Now.AddDays(-7), EndDate = DateTime.Now.AddDays(-7).AddHours(3), Responsible = "Pedro Santos", ResponsibleId = 5, Note = "Limpeza de ar condicionado" },
                new RoomHistory { RoomHistoryId = 60, RoomId = roomId, StartDate = DateTime.Now.AddDays(-10), EndDate = DateTime.Now.AddDays(-10).AddHours(2), Responsible = "Sofia Almeida", ResponsibleId = 6, Note = "Inspeção de segurança" },
                new RoomHistory { RoomHistoryId = 70, RoomId = roomId, StartDate = DateTime.Now.AddDays(-12), EndDate = DateTime.Now.AddDays(-12).AddHours(1), Responsible = "Ricardo Lopes", ResponsibleId = 7, Note = "Revisão de portas" },
                new RoomHistory { RoomHistoryId = 80, RoomId = roomId, StartDate = DateTime.Now.AddDays(-15), EndDate = DateTime.Now.AddDays(-15).AddHours(2), Responsible = "Helena Costa", ResponsibleId = 8, Note = "Verificação de extintores" },
                new RoomHistory { RoomHistoryId = 90, RoomId = roomId, StartDate = DateTime.Now.AddDays(-18), EndDate = DateTime.Now.AddDays(-18).AddHours(1), Responsible = "Miguel Rocha", ResponsibleId = 9, Note = "Revisão de cablagem" },
                new RoomHistory { RoomHistoryId = 100, RoomId = roomId, StartDate = DateTime.Now.AddDays(-20), EndDate = DateTime.Now.AddDays(-20).AddHours(2), Responsible = "Patrícia Gomes", ResponsibleId = 10, Note = "Limpeza geral" },
                new RoomHistory { RoomHistoryId = 110, RoomId = roomId, StartDate = DateTime.Now.AddDays(-22), EndDate = DateTime.Now.AddDays(-22).AddHours(1), Responsible = "Tiago Martins", ResponsibleId = 11, Note = "Inspeção técnica" },
                new RoomHistory { RoomHistoryId = 120, RoomId = roomId, StartDate = DateTime.Now.AddDays(-25), EndDate = DateTime.Now.AddDays(-25).AddHours(2), Responsible = "Beatriz Silva", ResponsibleId = 12, Note = "Troca de filtros" },
                new RoomHistory { RoomHistoryId = 130, RoomId = roomId, StartDate = DateTime.Now.AddDays(-27), EndDate = DateTime.Now.AddDays(-27).AddHours(1), Responsible = "André Carvalho", ResponsibleId = 13, Note = "Revisão de iluminação" },
                new RoomHistory { RoomHistoryId = 140, RoomId = roomId, StartDate = DateTime.Now.AddDays(-30), EndDate = DateTime.Now.AddDays(-30).AddHours(2), Responsible = "Mariana Ribeiro", ResponsibleId = 14, Note = "Limpeza técnica" },
                new RoomHistory { RoomHistoryId = 150, RoomId = roomId, StartDate = DateTime.Now.AddDays(-32), EndDate = DateTime.Now.AddDays(-32).AddHours(1), Responsible = "João Costa", ResponsibleId = 15, Note = "Inspeção final" }
            };

            // Procurar o histórico pelo ID e RoomId
            var history = fakeData.FirstOrDefault(h => h.RoomHistoryId == id && h.RoomId == roomId);

            if (history == null)
                return NotFound();

            ViewBag.RoomId = roomId;
            ViewData["Title"] = "Detalhes do Histórico da Sala";

            return View(history); // envia para HistoryDetails.cshtml
        }
    }
}