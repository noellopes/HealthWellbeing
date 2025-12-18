using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using HealthWellbeingRoom.Models;
using HealthWellbeingRoom.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellBeingRoom.Controllers
{
    [Authorize]
    public class MedicalDeviceController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public MedicalDeviceController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // --- LISTA (Index) ---
        [Authorize(Roles = "logisticsTechnician,Administrator")]
        public async Task<IActionResult> Index(
        int page = 1,
        string searchName = "",
        string searchSerial = "", 
        string searchType = "",
        string searchRoom = "",
        string searchStatus = "")
        {
            // Consulta base com Eager Loading
            var devicesQuery = _context.MedicalDevices
                .Include(m => m.TypeMaterial)
                .Include(m => m.LocalizacaoDispMedicoMovel)
                    .ThenInclude(loc => loc.Room)
                .AsQueryable();

            // --- Filtro por NOME ---
            if (!string.IsNullOrEmpty(searchName))
            {
                devicesQuery = devicesQuery.Where(d => d.Name.Contains(searchName));
            }

            // --- Filtro por NÚMERO DE SÉRIE ---
            if (!string.IsNullOrEmpty(searchSerial))
            {
                devicesQuery = devicesQuery.Where(d => d.SerialNumber.Contains(searchSerial));
            }

            // --- Filtro por TIPO ---
            if (!string.IsNullOrEmpty(searchType))
            {
                devicesQuery = devicesQuery.Where(d => d.TypeMaterial.Name.Contains(searchType));
            }

            // --- Filtro por SALA ---
            if (!string.IsNullOrEmpty(searchRoom))
            {
                devicesQuery = devicesQuery.Where(d =>
                    d.LocalizacaoDispMedicoMovel.Any(l => l.EndDate == null && l.Room.Name.Contains(searchRoom)));
            }

            // --- Filtro por ESTADO  ---
            if (!string.IsNullOrEmpty(searchStatus))
            {
                if (searchStatus == "Em Manutenção")
                {
                    // Filtra pela flag
                    devicesQuery = devicesQuery.Where(d => d.IsUnderMaintenance == true);
                }
                else if (searchStatus == "Em Armazenamento")
                {
                    // Não está em manutenção E (está num Depósito OU não tem sala ativa)
                    devicesQuery = devicesQuery.Where(d =>
                        d.IsUnderMaintenance == false &&
                        d.LocalizacaoDispMedicoMovel.Any(l => l.EndDate == null && l.Room.Name.Contains("Depósito")));
                }
                else if (searchStatus == "Alocado")
                {
                    // Não está em manutenção E tem sala ativa que NÃO é depósito
                    devicesQuery = devicesQuery.Where(d =>
                        d.IsUnderMaintenance == false &&
                        d.LocalizacaoDispMedicoMovel.Any(l => l.EndDate == null && !l.Room.Name.Contains("Depósito")));
                }
            }

            // Armazenar valores de pesquisa no ViewBag
            ViewBag.SearchName = searchName;
            ViewBag.SearchSerial = searchSerial;
            ViewBag.SearchType = searchType;
            ViewBag.SearchRoom = searchRoom;
            ViewBag.SearchStatus = searchStatus; 

            // Paginação e Ordenação
            int totalItems = await devicesQuery.CountAsync();
            var paginationInfo = new RPaginationInfo<MedicalDevice>(page, totalItems, itemsPerPage: 10);

            var listaDeDispositivos = await devicesQuery
                .OrderBy(d => d.Name)
                .Skip(paginationInfo.ItemsToSkip)
                .Take(paginationInfo.ItemsPerPage)
                .ToListAsync();

            paginationInfo.Items = listaDeDispositivos;

            // Carregar Listas para Dropdowns
            ViewBag.TypeMaterialList = new SelectList(await _context.TypeMaterial.OrderBy(t => t.Name).ToListAsync(), "Name", "Name", searchType);
            ViewBag.RoomList = new SelectList(await _context.Room.OrderBy(s => s.Name).ToListAsync(), "Name", "Name", searchRoom);

            // Lista Estática para o Estado (Hardcoded porque são regras de negócio fixas)
            var listaEstados = new List<string> { "Em Armazenamento", "Alocado", "Em Manutenção" };
            ViewBag.StatusList = new SelectList(listaEstados, searchStatus);

            return View(paginationInfo);
        }

        [Authorize(Roles = "logisticsTechnician,Administrator")]
        public async Task<IActionResult> Details(int? id, int? roomId, string origem)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalDevice = await _context.MedicalDevices
                .Where(m => m.MedicalDeviceID == id)
                .Include(m => m.TypeMaterial)
                .Include(m => m.LocalizacaoDispMedicoMovel
                    .Where(loc => loc.EndDate == null)) // apenas localização ativa
                    .ThenInclude(loc => loc.Room)
                .FirstOrDefaultAsync();

            if (medicalDevice == null)
            {
                return NotFound();
            }

            // Contexto adicional para navegação
            ViewBag.Origem = origem;
            ViewBag.RoomId = roomId ?? medicalDevice.LocalizacaoDispMedicoMovel
                .FirstOrDefault(l => l.EndDate == null)?.RoomId;

            return View(medicalDevice);
        }

        // --- 3. CRIAR (Create) - GET ---
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            ViewBag.TypeMaterialID = new SelectList(_context.Set<TypeMaterial>().OrderBy(t => t.Name), "TypeMaterialID", "Name");
            return View(new MedicalDevice());
        }

        // POST: MedicalDevice/Create 
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        // BIND: Inclui SerialNumber e ManufacturerId
        public async Task<IActionResult> Create([Bind("MedicalDeviceID,Name,SerialNumber,Observation,TypeMaterialID,IsUnderMaintenance")] MedicalDevice medicalDevices)
        {
            // [A. VERIFICAÇÃO DE UNICIDADE DO SerialNumber]
            bool serialExists = await _context.MedicalDevices
                .AnyAsync(m => m.SerialNumber == medicalDevices.SerialNumber);

            if (serialExists)
            {
                ModelState.AddModelError("SerialNumber",
                                         $"O Número de Série '{medicalDevices.SerialNumber}' já está registado. Por favor, utilize um número único.");
            }

            if (ModelState.IsValid)
            {
                medicalDevices.RegistrationDate = DateTime.Now;

                _context.Add(medicalDevices);
                await _context.SaveChangesAsync();

                // Procura uma sala com o nome "Depósito" E que o estado da sala esteja "Disponível"
                var salaDeposito = await _context.Room
                .Include(r => r.RoomStatus) // Necessário para acessar as propriedades do Status
                .Where(r => r.Name.Contains("Depósito") &&
                            r.RoomStatus != null && // Segurança contra nulos
                            r.RoomStatus.Name == "Disponível") // Verifica o NOME dentro do objeto RoomStatus
                .FirstOrDefaultAsync();

                if (salaDeposito != null)
                {
                    // Código de criação da localização ao criar novo dispositivo
                    var novaLocalizacao = new LocationMedDevice
                    {
                        MedicalDeviceID = medicalDevices.MedicalDeviceID,
                        RoomId = salaDeposito.RoomId,
                        InitialDate = DateTime.Now,
                        EndDate = null
                    };
                    _context.Add(novaLocalizacao);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"Dispositivo '{medicalDevices.Name}' registado e alocado na sala {salaDeposito.Name}!";
                }
                else
                {
                    TempData["WarningMessage"] = $"Dispositivo '{medicalDevices.Name}' registado, mas não foi possível encontrar um Depósito disponível.";
                }

                return RedirectToAction(nameof(Details), new { id = medicalDevices.MedicalDeviceID });
            }

            // Recarregar ViewBags em caso de falha (seja por SerialNumber ou outra validação)
            ViewBag.TypeMaterialID = new SelectList(_context.Set<TypeMaterial>(), "TypeMaterialID", "Name", medicalDevices.TypeMaterialID);

            return View(medicalDevices);
        }

        // --- 4. EDITAR (Edit) - GET ---
        [Authorize(Roles = "logisticsTechnician,Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var medicalDevices = await _context.MedicalDevices
                .Include(m => m.TypeMaterial) 
                .FirstOrDefaultAsync(m => m.MedicalDeviceID == id);

            if (medicalDevices == null)
            {
                return View("NotFound");
            }

            ViewBag.TypeMaterialID = new SelectList(_context.Set<TypeMaterial>(), "TypeMaterialID", "Name", medicalDevices.TypeMaterialID);

            return View(medicalDevices);
        }

        // POST: MedicalDevice/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "logisticsTechnician,Administrator")]
        public async Task<IActionResult> Edit(int id, [Bind("MedicalDeviceID,Name,SerialNumber,RegistrationDate,Observation,TypeMaterialID,IsUnderMaintenance")] MedicalDevice medicalDevices)
        {
            if (id != medicalDevices.MedicalDeviceID) return View("NotFound");

            // 1. Buscar o objeto REAl da base de dados
            var dispositivoDB = await _context.MedicalDevices
                .Include(m => m.LocalizacaoDispMedicoMovel)
                .FirstOrDefaultAsync(m => m.MedicalDeviceID == id);

            if (dispositivoDB == null) return View("NotFound");

            // 2. Definição de Permissões (Quem é o utilizador?)
            bool isAdmin = User.IsInRole("Administrator");
            bool isLogistica = User.IsInRole("logisticsTechnician");

            // 3. Validação de Unicidade do Serial (Apenas se quem está a mexer for Logística, pois Admin não muda Serial)
            if (isLogistica)
            {
                // Se o serial mudou, verifica se já existe noutro
                if (dispositivoDB.SerialNumber != medicalDevices.SerialNumber)
                {
                    bool serialExiste = await _context.MedicalDevices
                        .AnyAsync(d => d.SerialNumber == medicalDevices.SerialNumber && d.MedicalDeviceID != id);

                    if (serialExiste) ModelState.AddModelError("SerialNumber", "Este Número de Série já está registado.");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // --- CENÁRIO ADMINISTRADOR ---
                    // Edita: Nome, Tipo, Observação. 
                    // NÃO Edita: Serial, Manutenção.
                    if (isAdmin)
                    {
                        dispositivoDB.Name = medicalDevices.Name;
                        dispositivoDB.Observation = medicalDevices.Observation;
                        dispositivoDB.TypeMaterialID = medicalDevices.TypeMaterialID;
                        // Ignoramos alterações ao SerialNumber e IsUnderMaintenance vindas do formulário
                    }

                    // --- CENÁRIO LOGÍSTICA ---
                    // Edita: Serial, Manutenção.
                    // NÃO Edita: Nome, Tipo, Observação.
                    else if (isLogistica)
                    {
                        dispositivoDB.SerialNumber = medicalDevices.SerialNumber;

                        // Lógica complexa de Manutenção (Só Logística pode fazer isto)
                        bool entrouEmManutencao = !dispositivoDB.IsUnderMaintenance && medicalDevices.IsUnderMaintenance;
                        bool saiuDeManutencao = dispositivoDB.IsUnderMaintenance && !medicalDevices.IsUnderMaintenance;

                        // Atualiza o estado na BD
                        dispositivoDB.IsUnderMaintenance = medicalDevices.IsUnderMaintenance;

                        if (entrouEmManutencao)
                        {
                            var localizacaoAtiva = dispositivoDB.LocalizacaoDispMedicoMovel.FirstOrDefault(l => l.EndDate == null);
                            if (localizacaoAtiva != null)
                            {
                                localizacaoAtiva.EndDate = DateTime.Now;
                            }
                            TempData["SuccessMessage"] = $"Nº Série atualizado e dispositivo colocado em Manutenção.";
                        }
                        else if (saiuDeManutencao)
                        {
                            // Lógica de mover para depósito
                            var salaDeposito = await _context.Room
                                .Include(r => r.RoomStatus)
                                .Where(r => r.Name.Contains("Depósito") && r.RoomStatus.Name == "Disponível")
                                .FirstOrDefaultAsync();

                            if (salaDeposito != null)
                            {
                                // Fecha localizações antigas (segurança)
                                var locsAntigas = _context.LocationMedDevice.Where(l => l.MedicalDeviceID == id && l.EndDate == null);
                                foreach (var l in locsAntigas) l.EndDate = DateTime.Now;

                                var novaLocalizacao = new LocationMedDevice
                                {
                                    MedicalDeviceID = id,
                                    RoomId = salaDeposito.RoomId,
                                    InitialDate = DateTime.Now,
                                    EndDate = null,
                                    IsCurrent = true
                                };
                                _context.Add(novaLocalizacao);
                                TempData["SuccessMessage"] = $"Manutenção concluída! Movido para {salaDeposito.Name}.";
                            }
                            else
                            {
                                TempData["WarningMessage"] = $"Manutenção concluída, mas Depósito não encontrado.";
                            }
                        }
                        else
                        {
                            TempData["SuccessMessage"] = $"Número de série atualizado com sucesso.";
                        }
                    }

                    // Gravar alterações
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new { id = dispositivoDB.MedicalDeviceID });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicalDevicesExists(medicalDevices.MedicalDeviceID)) return View("NotFound");
                    else throw;
                }
            }

            ViewBag.TypeMaterialID = new SelectList(_context.Set<TypeMaterial>(), "TypeMaterialID", "Name", medicalDevices.TypeMaterialID);
            return View(medicalDevices);
        }

        // --- 5. APAGAR (Delete) - GET ---
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var medicalDevices = await _context.MedicalDevices
                .Include(m => m.TypeMaterial)

                //Usar a lógica temporal EndDate == null para carregar SÓ o registo ATIVO
                .Include(md => md.LocalizacaoDispMedicoMovel // carrega a tabela localização
                     .Where(loc => loc.EndDate == null) // A localização ativa é aquela que não foi encerrada
                )

                .ThenInclude(loc => loc.Room) // para ter acesso ao nome da sala
                .FirstOrDefaultAsync(m => m.MedicalDeviceID == id);

            if (medicalDevices == null)
            {
                return View("NotFound");
            }

            return View(medicalDevices);
        }

        // POST: MedicalDevice/Delete/5 (Substitui o seu DeleteConfirmed POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medicalDevices = await _context.MedicalDevices.FindAsync(id);
            string deviceName = "";

            if (medicalDevices != null)
            {
                deviceName = medicalDevices.Name;
                _context.MedicalDevices.Remove(medicalDevices);
            }
            else
            {
                TempData["ErrorMessage"] = $"Dispositivo com ID {id} não encontrado para eliminação.";
                return RedirectToAction(nameof(Index));
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"O dispositivo '{deviceName}' foi eliminado permanentemente.";

            return RedirectToAction(nameof(Index));
        }

        private bool MedicalDevicesExists(int id)
        {
            return _context.MedicalDevices.Any(e => e.MedicalDeviceID == id);
        }


        // GET: MedicalDevice/HistoryLoc/5
        [Authorize(Roles = "logisticsTechnician,Administrator")]
        public async Task<IActionResult> HistoryLoc(
            int id,
            int page = 1,
            string searchRoom = "",
            string searchDate = "")
        {
            // 1. Verificar se o dispositivo existe (para exibir o nome no título)
            var device = await _context.MedicalDevices.FindAsync(id);
            if (device == null) return NotFound();

            // 2. Query Base (Igual à do Marcel, mas filtrada pelo ID do dispositivo)
            var query = _context.LocationMedDevice
                .Include(l => l.Room)           // Precisamos do nome da sala
                .Include(l => l.MedicalDevice)  // Opcional, mas boa prática
                .Where(l => l.MedicalDeviceID == id) // <--- O FILTRO CRUCIAL (Só este dispositivo)
                .AsQueryable();


            // Filtro por Sala
            if (!string.IsNullOrEmpty(searchRoom))
            {
                query = query.Where(l => l.Room.Name.Contains(searchRoom));
            }

            // Filtro por Data
            if (!string.IsNullOrEmpty(searchDate))
            {
                if (DateTime.TryParse(searchDate, out DateTime parsedDate))
                {
                    query = query.Where(l => l.InitialDate.Date == parsedDate.Date);
                }
            }

            int itemsPerPage = 10;
            int totalItems = await query.CountAsync();

            var paginationInfo = new RPaginationInfo<LocationMedDevice>(page, totalItems, itemsPerPage);

            // Buscar dados ordenados (Do mais recente para o mais antigo fica melhor em históricos)
            var historyItems = await query
                .OrderByDescending(l => l.InitialDate)
                .Skip(paginationInfo.ItemsToSkip)
                .Take(paginationInfo.ItemsPerPage)
                .ToListAsync();

            paginationInfo.Items = historyItems;

            // Dados para o Cabeçalho
            ViewBag.DeviceName = device.Name;
            ViewBag.DeviceSerial = device.SerialNumber;
            ViewBag.DeviceId = id;

            // Manter os filtros na caixa de pesquisa
            ViewBag.SearchRoom = searchRoom;
            ViewBag.SearchDate = searchDate;

            // Carregar lista para dropdowns
            ViewBag.RoomList = new SelectList(await _context.Room.OrderBy(s => s.Name).ToListAsync(), "Name", "Name", searchRoom);

            return View(paginationInfo);
        }

        // GET: MedicalDevice/DeviceGroupIndex
        [Authorize(Roles = "logisticsTechnician, Administrator")]
        public async Task<IActionResult> DeviceGroupIndex(string searchName, int page = 1)
        {
            int pageSize = 10;

            // Guardar a pesquisa para devolver à View (para a caixa de texto e paginação)
            ViewData["CurrentFilter"] = searchName;

            // 1. Começar a Query
            var query = _context.MedicalDevices.AsQueryable();

            // 2. Aplicar Filtro (SE houver pesquisa)
            if (!string.IsNullOrEmpty(searchName))
            {
                query = query.Where(m => m.Name.Contains(searchName));
            }

            // 3. Agrupar (Agora já filtra antes de agrupar!)
            var groupedQuery = query.GroupBy(m => m.Name);

            // 4. Contar grupos
            var totalItems = await groupedQuery.CountAsync();

            // 5. Ordenar, Paginar e Transformar na ViewModel
            var items = await groupedQuery
                .OrderBy(g => g.Key)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(g => new DeviceGroupMedicalDevice
                {
                    DeviceName = g.Key,
                    TotalQuantity = g.Count()
                })
                .ToListAsync();

            // 6. Criar Paginação
            var paginationInfo = new RPaginationInfo<DeviceGroupMedicalDevice>(page, totalItems, pageSize)
            {
                Items = items
            };

            return View(paginationInfo);
        }
    }
}