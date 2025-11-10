using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using HealthWellbeingRoom.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellBeingRoom.Controllers
{
    public class MedicalDeviceController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public MedicalDeviceController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // --- 1. LISTA (Index) ---
        public async Task<IActionResult> Index(
            int page = 1,
            string searchName = "",
            string searchType = "",
            string searchRoom = "")
        {
            //Inicializa a query base com os Includes necessários (Eager Loading)
            var devicesQuery = _context.MedicalDevices
                .Include(m => m.TypeMaterial)
                // Incluir a coleção da Localização, filtrando só o registo ativo (IsCurrent = true)
                .Include(md => md.LocalizacaoDispMedicoMovel
                    .Where(loc => loc.IsCurrent == true)
                )
                .ThenInclude(loc => loc.Room)
                .AsQueryable(); 

            //Aplicação dos Filtros de Pesquisa
            if (!string.IsNullOrEmpty(searchName))
            {
                //Pesquisa pelo Nome do Dispositivo
                devicesQuery = devicesQuery.Where(d => d.Name.Contains(searchName));
            }

            if (!string.IsNullOrEmpty(searchType))
            {
                //Pesquisa pelo Nome do Tipo de Material
                devicesQuery = devicesQuery.Where(d => d.TypeMaterial.Name.Contains(searchType));
            }

            if (!string.IsNullOrEmpty(searchRoom))
            {
                //Pesquisa pelo Nome da Sala na Localização Atual (IsCurrent == true)
                devicesQuery = devicesQuery.Where(d => d.LocalizacaoDispMedicoMovel
                    .Any(loc => loc.IsCurrent == true && loc.Room.Name.Contains(searchRoom)));
            }

            // Armazena os valores de pesquisa no ViewBag para persistir na View (mantendo o texto na caixa de busca)
            ViewBag.SearchName = searchName;
            ViewBag.SearchType = searchType;
            ViewBag.SearchRoom = searchRoom;

            //Conta o total de itens após o filtro
            int totalItems = await devicesQuery.CountAsync();

            //Cria a instância do ViewModel de Paginação (RPaginationInfo)
            var paginationInfo = new RPaginationInfo<MedicalDevice>(page, totalItems, itemsPerPage: 10); // 10 por página

            // Executa a Consulta com Ordenação e Paginação
            var listaDeDispositivos = await devicesQuery
                .OrderBy(d => d.Name) // Ordenação para garantir consistência entre páginas
                .Skip(paginationInfo.ItemsToSkip) // Lógica de Skip do ViewModel
                .Take(paginationInfo.ItemsPerPage) // Lógica de Take do ViewModel
                .ToListAsync();

            //Atribui os itens ao ViewModel
            paginationInfo.Items = listaDeDispositivos;

            return View(paginationInfo);
        }

        // --- 2. DETALHES (Read/Details) ---
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                // Se não houver ID, tratamos como NotFound
                return View("NotFound");
            }

            var dispositivo = await _context.MedicalDevices
                .Include(m => m.TypeMaterial)
                //Incluir a coleção da Localização, filtrando SÓ o registo ativo (IsCurrent = true)
                .Include(md => md.LocalizacaoDispMedicoMovel //carrega a tabela localização
                    .Where(loc => loc.IsCurrent == true) //garante que só trazemos o registo de localização que é o **atual**
                )
                .ThenInclude(loc => loc.Room) //para ter acesso ao nome da sala
                .FirstOrDefaultAsync(m => m.MedicalDeviceID == id);

            if (dispositivo == null)
            {
                // Se não encontrar, mostra o NotFound
                return View("NotFound");
            }

            // CORRIGIDO: Se encontrar, retorna a View padrão (Details.cshtml)
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

                TempData["SuccessMessage"] = $"Dispositivo '{medicalDevices.Name}' Registado com sucesso!";
                return RedirectToAction(nameof(Index));
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        // BIND: Inclui todos os campos editáveis e FKs.
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
                    _context.Update(medicalDevices);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"As alterações no dispositivo '{medicalDevices.Name}' foram salvas com sucesso!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicalDevicesExists(medicalDevices.MedicalDeviceID))
                    {
                        return View("NotFound");
                    }
                    else
                    {
                        throw;
                    }
                }
                // Volta para Details para ver o resultado da edição
                return RedirectToAction(nameof(Details), new { id = medicalDevices.MedicalDeviceID });
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
                //Incluir a coleção da Localização, filtrando SÓ o registo ativo (IsCurrent = true)
                .Include(md => md.LocalizacaoDispMedicoMovel //carrega a tabela localização
                    .Where(loc => loc.IsCurrent == true) //garante que só trazemos o registo de localização que é o **atual**
                )
                .ThenInclude(loc => loc.Room) //para ter acesso ao nome da sala
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