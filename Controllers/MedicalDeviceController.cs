using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using HealthWellbeingRoom.Models;
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
    [Authorize(Roles = "logisticsTechnician,Administrator")]
    public class MedicalDeviceController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public MedicalDeviceController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // --- LISTA (Index) ---
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


        // --- 2. DETALHES (Read/Details) ---
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var dispositivo = await _context.MedicalDevices
                .Include(m => m.TypeMaterial)

                // Incluir a coleção da Localização, filtrando SÓ o registo ATIVO (EndDate == null)
                .Include(md => md.LocalizacaoDispMedicoMovel
                     .Where(loc => loc.EndDate == null) // 🎯 CORREÇÃO: Usar a lógica temporal
                )
                .ThenInclude(loc => loc.Room) // para ter acesso ao nome da sala
                .FirstOrDefaultAsync(m => m.MedicalDeviceID == id);

            if (dispositivo == null)
            {
                return View("NotFound");
            }

            return View(dispositivo);
        }

        // --- 3. CRIAR (Create) - GET ---
        public IActionResult Create()
        {
            ViewBag.TypeMaterialID = new SelectList(_context.Set<TypeMaterial>(), "TypeMaterialID", "Name");
            return View(new MedicalDevice());
        }

        // POST: MedicalDevice/Create 
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        // POST: MedicalDevice/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MedicalDeviceID,Name,SerialNumber,RegistrationDate,Observation,TypeMaterialID,IsUnderMaintenance")] MedicalDevice medicalDevices)
        {
            if (id != medicalDevices.MedicalDeviceID)
            {
                return View("NotFound");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //busca a "Ficha Completa" original do dispositivo tal como está gravada na base de dados
                    var dispositivoOriginal = await _context.MedicalDevices 
                        .AsNoTracking() //para não bloquear o Entity Framework ao atualizar o objeto 'medicalDevices' depois.
                        .FirstOrDefaultAsync(m => m.MedicalDeviceID == id);

                    if (dispositivoOriginal == null) return View("NotFound");

                    // Detetar Mudanças de Estado
                    bool entrouEmManutencao = !dispositivoOriginal.IsUnderMaintenance && medicalDevices.IsUnderMaintenance;
                    bool saiuDeManutencao = dispositivoOriginal.IsUnderMaintenance && !medicalDevices.IsUnderMaintenance;

                    _context.Update(medicalDevices);

                    //Entrou em Manutenção (Fecha a localização atual), deixa o dispositivo num limbo(como se não estivesse ali)
                    if (entrouEmManutencao)
                    {
                        var localizacaoAtiva = await _context.LocationMedDevice
                            .FirstOrDefaultAsync(l => l.MedicalDeviceID == id && l.EndDate == null);

                        if (localizacaoAtiva != null)
                        {
                            localizacaoAtiva.EndDate = DateTime.Now;
                            _context.Update(localizacaoAtiva);
                        }
                        TempData["SuccessMessage"] = $"Dispositivo '{medicalDevices.Name}' atualizado e colocado em Modo de Manutenção (Localização anterior encerrada).";
                    }
                    //Saiu de Manutenção (Procura o Depósito)
                    else if (saiuDeManutencao)
                    {
                        // Procura Depósito Disponível (com base no RoomStatus)
                        var salaDeposito = await _context.Room
                            .Include(r => r.RoomStatus)
                            .Where(r => r.Name.Contains("Depósito") &&
                                        r.RoomStatus != null &&
                                        r.RoomStatus.Name == "Disponível")
                            .FirstOrDefaultAsync();

                        if (salaDeposito != null)
                        {
                            // Garante que fecha registos antigos
                            var locsAntigas = await _context.LocationMedDevice
                                .Where(l => l.MedicalDeviceID == id && l.EndDate == null)
                                .ToListAsync();

                            locsAntigas.ForEach(l => l.EndDate = DateTime.Now);

                            // Cria nova localização para o dispositivo(vai para o deposito)
                            var novaLocalizacao = new LocationMedDevice
                            {
                                MedicalDeviceID = id,
                                RoomId = salaDeposito.RoomId,
                                InitialDate = DateTime.Now,
                                EndDate = null
                            };
                            _context.Add(novaLocalizacao);

                            TempData["SuccessMessage"] = $"Manutenção concluída!";
                        }
                        else
                        {
                            TempData["WarningMessage"] = $"Manutenção concluída, mas não foi possível alocar para o Depósito.";
                        }
                    }
                    else
                    {
                        TempData["SuccessMessage"] = $"As alterações no dispositivo '{medicalDevices.Name}' foram salvas com sucesso!";
                    }

                    //SALVAR TUDO
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Details), new { id = medicalDevices.MedicalDeviceID });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicalDevicesExists(medicalDevices.MedicalDeviceID))
                    {
                        ViewBag.MedDeviceWasDeleted = true;
                        return View("NotFound");
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // Recarregar ViewBags em caso de falha
            ViewBag.TypeMaterialID = new SelectList(_context.Set<TypeMaterial>(), "TypeMaterialID", "Name", medicalDevices.TypeMaterialID);

            return View(medicalDevices);
        }

        // --- 5. APAGAR (Delete) - GET ---
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
    }
}