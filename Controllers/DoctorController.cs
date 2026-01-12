using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


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
        public async Task<IActionResult> Index(int page = 1, string searchName = "")
        {
            // Base query sem Includes (a menos que tenhas navegações para carregar)
            IQueryable<Doctor> doctorQuery = _context.Doctor
                .AsNoTracking()
                .Include(d => d.Especialidade);
            if (!string.IsNullOrEmpty(searchName))
            {
                doctorQuery = doctorQuery.Where(d => d.Nome.Contains(searchName));
            }
            ViewBag.SearchName = searchName;
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
        [Authorize(Roles = "DiretorClinico,Medico")]

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var doctor = await _context.Doctor
                .Include(d => d.Especialidade)
                .FirstOrDefaultAsync(m => m.IdMedico == id);
            if (doctor == null)
            {
                return NotFound();
            }
            var medico = await GetDoctorFromLoggedUser();
            if (medico == null)
                return Content("Não existe um médico associado ao teu utilizador.");


            if (medico.IdMedico != doctor.IdMedico)
                return Forbid();
            return View(doctor);

        }

        // GET: Doctors/Create
        [Authorize(Roles = "DiretorClinico")]

        public IActionResult Create()
        {
            ViewData["IdEspecialidade"] = new SelectList(
                _context.Specialities,
                "IdEspecialidade",
                "Nome"
            );

            return View();
        }

        // POST: Doctors/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "DiretorClinico")]
        public async Task<IActionResult> Create([Bind("IdMedico,Nome,Telemovel,Email,IdEspecialidade")] Doctor doctor)
        {
            if (await _context.Doctor.AnyAsync(d => d.Email == doctor.Email))
            {
                ModelState.AddModelError("Email", "Já existe um médico registado com este email.");
            }

            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) })
                .ToList();


            if (ModelState.IsValid)
            {
                _context.Add(doctor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdEspecialidade"] = new SelectList(
                _context.Specialities,
                "IdEspecialidade",
                "Nome",
                doctor.IdEspecialidade
            );

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

            ViewData["IdEspecialidade"] = new SelectList(
                _context.Specialities,
                "IdEspecialidade",
                "Nome",
                doctor.IdEspecialidade
            );

            return View(doctor);
        }

        // POST: Doctors/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdMedico,Nome,Telemovel,Email, IdEspecialidade")] Doctor doctor)
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
        [HttpPost, ActionName("Eliminar")]
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
        private async Task<Doctor?> GetDoctorFromLoggedUser()
        {
            var email = User?.Identity?.Name?.Trim();
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return await _context.Doctor
                .FirstOrDefaultAsync(d => d.Email == email);
        }
    }
}
