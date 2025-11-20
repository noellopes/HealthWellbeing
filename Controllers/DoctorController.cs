using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellbeing.Controllers
{
    public class DoctorController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public DoctorController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Doctors
        public async Task<IActionResult> Index(int page = 1)
        {
            // Base query sem Includes (a menos que tenhas navegações para carregar)
            var doctorQuery = _context.Doctor
                .AsNoTracking();

            // total de registos
            int numberDoctors = await doctorQuery.CountAsync();

            // cria o viewmodel de paginação
            var DoctorInfo = new PaginationInfo<Doctor>(page, numberDoctors);

            // define um pageSize 
            var pageSize = DoctorInfo.ItemsPerPage > 0 ? DoctorInfo.ItemsPerPage : 10;
            // aplica ordenação + paginação
            DoctorInfo.Items = await doctorQuery
                .OrderBy(d => d.Nome)                   
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return View(DoctorInfo);
        }

        // GET: Doctors/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctor
                .FirstOrDefaultAsync(m => m.IdMedico == id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // GET: Doctors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Doctors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdMedico,Nome,Telemovel,Email")] Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                
                _context.Add(doctor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(doctor);


           
        }

        // GET: Doctors/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctor.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }
            return View(doctor);
        }

        // POST: Doctors/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdMedico,Nome,Telemovel,Email")] Doctor doctor)
        {
            if (id != doctor.IdMedico)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(doctor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorExists(doctor.IdMedico))
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
            return View(doctor);
        }

        // GET: 
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctor
                .FirstOrDefaultAsync(m => m.IdMedico == id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doctor = await _context.Doctor.FindAsync(id);
            if (doctor != null)
            {
                _context.Doctor.Remove(doctor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DoctorExists(int id)
        {
            return _context.Doctor.Any(e => e.IdMedico == id);
        }
    }
}
