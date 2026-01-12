using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Controllers
{
    [Authorize]
    public class ClientController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ClientController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Client (Index)
        [Authorize(Roles = "Administrator,Trainer")]
        public async Task<IActionResult> Index(int page = 1, string searchName = "")
        {
            // CORREÇÃO: Usar .Member
            var clientsQuery = _context.Client
                .Include(c => c.Member)
                    .ThenInclude(m => m.MemberPlans.Where(mp => mp.Status == "Active"))
                        .ThenInclude(mp => mp.Plan)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchName))
                clientsQuery = clientsQuery.Where(c => c.Name.Contains(searchName));

            int total = await clientsQuery.CountAsync();
            var pagination = new PaginationInfo<Client>(page, total, 10);

            pagination.Items = await clientsQuery.OrderBy(c => c.Name)
                .Skip(pagination.ItemsToSkip).Take(pagination.ItemsPerPage).ToListAsync();

            ViewBag.SearchName = searchName;
            return View(pagination);
        }

        // GET: Client/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var client = await _context.Client
                .Include(c => c.Member) // CORREÇÃO: Usar .Member
                    .ThenInclude(m => m.MemberPlans)
                        .ThenInclude(mp => mp.Plan)
                .FirstOrDefaultAsync(m => m.ClientId == id);

            if (client == null) return NotFound();

            if (!IsStaff() && client.Email != User.Identity.Name)
            {
                return Forbid();
            }

            return View(client);
        }

        // ... Resto dos métodos (Create, Edit, Delete) mantêm-se iguais ...
        // ... Apenas certifique-se que não usa "Membership" em lado nenhum ...

        // GET: Client/Create
        [Authorize(Roles = "Administrator,Trainer")]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Trainer")]
        public async Task<IActionResult> Create([Bind("ClientId,Name,Email,Phone,Address,BirthDate,Gender,RegistrationDate")] Client client)
        {
            if (ModelState.IsValid) { _context.Add(client); await _context.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
            return View(client);
        }

        public async Task<IActionResult> Edit(int? id) { /* Código igual ao seu... */ if (id == null) return NotFound(); var client = await _context.Client.FindAsync(id); if (client == null) return NotFound(); if (!IsStaff() && client.Email != User.Identity.Name) return Forbid(); return View(client); }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ClientId,Name,Email,Phone,Address,BirthDate,Gender,RegistrationDate")] Client client) { /* Código igual ao seu... */ if (id != client.ClientId) return NotFound(); if (!IsStaff() && client.Email != User.Identity.Name) return Forbid(); if (ModelState.IsValid) { try { _context.Update(client); await _context.SaveChangesAsync(); if (!IsStaff()) return RedirectToAction(nameof(Details), new { id = client.ClientId }); } catch (DbUpdateConcurrencyException) { if (!ClientExists(client.ClientId)) return NotFound(); else throw; } return RedirectToAction(nameof(Index)); } return View(client); }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id) { /* Código igual ao seu... */ if (id == null) return NotFound(); var client = await _context.Client.FirstOrDefaultAsync(m => m.ClientId == id); if (client == null) return NotFound(); return View(client); }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id) { var client = await _context.Client.FindAsync(id); if (client != null) _context.Client.Remove(client); await _context.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }

        private bool ClientExists(int id) => _context.Client.Any(e => e.ClientId == id);
        private bool IsStaff() => User.IsInRole("Administrator") || User.IsInRole("Trainer");
    }
}