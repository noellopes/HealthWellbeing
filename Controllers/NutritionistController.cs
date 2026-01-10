using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;

namespace HealthWellbeing.Controllers
{
    [Authorize]
    public class NutritionistController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public NutritionistController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
            => User.IsInRole("Administrador") || User.IsInRole("Administrator");

        private IActionResult NoDataPermission()
            => View("~/Views/Shared/NoDataPermission.cshtml");

        private async Task<List<Tuple<int, string, string>>> LoadClientsAsync()
        {
            return await _context.Client
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => Tuple.Create(c.ClientId, c.Name, c.Email ?? ""))
                .ToListAsync();
        }

        [Authorize(Roles = "Administrador,Administrator,Nutricionista,Nutritionist,Cliente,Client")]
        public async Task<IActionResult> Index(string? search, int page = 1, int itemsPerPage = 10)
        {
            var query = _context.Nutritionist.AsNoTracking().AsQueryable();

            var s = (search ?? "").Trim();
            if (!string.IsNullOrWhiteSpace(s))
            {
                var sl = s.ToLowerInvariant();

                query = query.Where(n =>
                    (n.Name != null && n.Name.ToLower().Contains(sl)) ||
                    (!string.IsNullOrEmpty(n.Email) && n.Email.ToLower().Contains(sl)) ||
                    (!string.IsNullOrEmpty(n.Gender) && n.Gender.ToLower().Contains(sl)));
            }

            int totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(n => n.Name)
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToListAsync();

            ViewBag.Search = s;
            ViewBag.IsAdmin = IsAdmin();

            return View(new PaginationInfoFoodHabits<Nutritionist>(items, totalItems, page, itemsPerPage));
        }

        [Authorize(Roles = "Administrador,Administrator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var nutritionist = await _context.Nutritionist
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.NutritionistId == id);

            if (nutritionist == null) return NotFound();
            return View(nutritionist);
        }

        [Authorize(Roles = "Administrador,Administrator")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Clients = await LoadClientsAsync();
            return View(new Nutritionist());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Administrator")]
        public async Task<IActionResult> Create(int clientId, string gender)
        {
            ViewBag.Clients = await LoadClientsAsync();

            if (clientId <= 0)
                ModelState.AddModelError("ClientId", "Seleciona um cliente.");

            if (string.IsNullOrWhiteSpace(gender))
                ModelState.AddModelError("Gender", "Seleciona um género.");

            var client = await _context.Client
                .AsNoTracking()
                .Where(c => c.ClientId == clientId)
                .Select(c => new { c.Name, c.Email })
                .FirstOrDefaultAsync();

            if (client == null)
                ModelState.AddModelError("ClientId", "Cliente inválido.");

            if (client != null)
            {
                var email = (client.Email ?? "").Trim();

                var exists = await _context.Nutritionist
                    .AsNoTracking()
                    .AnyAsync(n => n.Email == email);

                if (exists)
                    ModelState.AddModelError("ClientId", "Este cliente já está registado como nutricionista.");
            }

            if (!ModelState.IsValid)
                return View(new Nutritionist());

            var n = new Nutritionist
            {
                Name = client!.Name,
                Email = client.Email,
                Gender = gender
            };

            _context.Nutritionist.Add(n);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrador,Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var nutritionist = await _context.Nutritionist.FindAsync(id);
            if (nutritionist == null) return NotFound();

            ViewBag.Clients = await LoadClientsAsync();

            int selectedClientId = 0;
            if (!string.IsNullOrWhiteSpace(nutritionist.Email))
            {
                selectedClientId = await _context.Client
                    .AsNoTracking()
                    .Where(c => c.Email == nutritionist.Email)
                    .Select(c => c.ClientId)
                    .FirstOrDefaultAsync();
            }

            ViewBag.SelectedClientId = selectedClientId;

            return View(nutritionist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Administrator")]
        public async Task<IActionResult> Edit(int id, int clientId, string gender)
        {
            var nutritionist = await _context.Nutritionist.FindAsync(id);
            if (nutritionist == null) return NotFound();

            ViewBag.Clients = await LoadClientsAsync();
            ViewBag.SelectedClientId = clientId;

            if (clientId <= 0)
                ModelState.AddModelError("ClientId", "Seleciona um cliente.");

            if (string.IsNullOrWhiteSpace(gender))
                ModelState.AddModelError("Gender", "Seleciona um género.");

            var client = await _context.Client
                .AsNoTracking()
                .Where(c => c.ClientId == clientId)
                .Select(c => new { c.Name, c.Email })
                .FirstOrDefaultAsync();

            if (client == null)
                ModelState.AddModelError("ClientId", "Cliente inválido.");

            if (client != null)
            {
                var email = (client.Email ?? "").Trim();

                var conflict = await _context.Nutritionist
                    .AsNoTracking()
                    .AnyAsync(n => n.Email == email && n.NutritionistId != id);

                if (conflict)
                    ModelState.AddModelError("ClientId", "Já existe outro nutricionista com este email.");
            }

            if (!ModelState.IsValid)
                return View(nutritionist);

            nutritionist.Name = client!.Name;
            nutritionist.Email = client.Email;
            nutritionist.Gender = gender;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrador,Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var nutritionist = await _context.Nutritionist
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.NutritionistId == id);

            if (nutritionist == null) return NotFound();
            return View(nutritionist);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nutritionist = await _context.Nutritionist.FindAsync(id);
            if (nutritionist != null)
            {
                _context.Nutritionist.Remove(nutritionist);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
