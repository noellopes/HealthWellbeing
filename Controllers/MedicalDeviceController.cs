using HealthWellbeing.Data;
using HealthWellbeingRoom.Models.FileMedicalDevices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellbeingRoom.Controllers
{
    public class MedicalDeviceController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        // Construtor: Injeta o DbContext, mantendo a abordagem do Scaffolding (EF Core direto)
        public MedicalDeviceController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // 1. LISTA (Index)
        public async Task<IActionResult> Index()
        {
            // Usa o ToListAsync() do EF Core para obter a lista
            var listaDeDispositivos = await _context.MedicalDevices.ToListAsync();
            return View(listaDeDispositivos);
        }

        // 2. DETALHES (Read/Details)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Usa o FirstOrDefaultAsync do EF Core 
            var dispositivo = await _context.MedicalDevices
                .FirstOrDefaultAsync(m => m.DevicesID == id);

            if (dispositivo == null)
            {
                TempData["Mensagem de erro"] = $"Dispositivo com ID {id} não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            return View(dispositivo);
        }

        // 3. CRIAR (Create)
        public IActionResult Create()
        {
            return View(new MedicalDevices());
        }

        // POST: MedicalDevice/Create 
        [HttpPost]
        [ValidateAntiForgeryToken]
        // **[Bind] ATUALIZADO:** Quantity e SalaID removidos
        public async Task<IActionResult> Create([Bind("DevicesID,Name,Type,Specification,RegistrationDate,Status,Observation")] MedicalDevices medicalDevices)
        {
            if (ModelState.IsValid)
            {
                //Define a data de registo para a data e hora local de agora
                medicalDevices.RegistrationDate = DateTime.Now;

                _context.Add(medicalDevices); // Chama o método Add do EF Core
                await _context.SaveChangesAsync();

                TempData["Mensagem de sucesso"] = $"Dispositivo '{medicalDevices.Name}' Registado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            return View(medicalDevices);
        }

        // 4. EDITAR (Edit) 
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalDevices = await _context.MedicalDevices.FindAsync(id);
            if (medicalDevices == null)
            {
                return NotFound();
            }
            return View(medicalDevices);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // **[Bind] ATUALIZADO:** Quantity e SalaID removidos
        public async Task<IActionResult> Edit(int id, [Bind("DevicesID,Name,Type,Specification,RegistrationDate,Status,Observation")] MedicalDevices medicalDevices)
        {
            if (id != medicalDevices.DevicesID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medicalDevices); // Chama o método Update do EF Core
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicalDevicesExists(medicalDevices.DevicesID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(medicalDevices);
        }

        // 5. APAGAR (Delete)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalDevices = await _context.MedicalDevices
                .FirstOrDefaultAsync(m => m.DevicesID == id);

            if (medicalDevices == null)
            {
                return NotFound();
            }

            return View(medicalDevices);
        }

        // POST: MedicalDevice/Delete/5 (Substitui o seu DeleteConfirmed POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medicalDevices = await _context.MedicalDevices.FindAsync(id);

            if (medicalDevices != null)
            {
                _context.MedicalDevices.Remove(medicalDevices); // Chama o método Remove do EF Core
            }
            else
            {
                // Trata o caso de não encontrar o dispositivo
                TempData["Mensagem de erro"] = $"Dispositivo com ID {id} não encontrado para eliminação.";
                return RedirectToAction(nameof(Index));
            }

            await _context.SaveChangesAsync();
            TempData["Mensagem de sucesso"] = $"Dispositivo eliminado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        private bool MedicalDevicesExists(int id)
        {
            return _context.MedicalDevices.Any(e => e.DevicesID == id);
        }
    }
}