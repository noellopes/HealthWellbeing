using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;

namespace HealthWellbeing.Controllers
{
    public class ClientController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ClientController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Client
        public async Task<IActionResult> Index(int page = 1, string searchName = "", string searchPhone = "", string searchEmail = "")
        {
            var clientsQuery = _context.Client
                .Include(c => c.Membership)
                .AsQueryable();

            // Filtros de Pesquisa
            if (!string.IsNullOrEmpty(searchName))
            {
                clientsQuery = clientsQuery.Where(c => c.Name.Contains(searchName));
            }

            if (!string.IsNullOrEmpty(searchPhone))
            {
                clientsQuery = clientsQuery.Where(c => c.Phone.Contains(searchPhone));
            }

            if (!string.IsNullOrEmpty(searchEmail))
            {
                clientsQuery = clientsQuery.Where(c => c.Email.Contains(searchEmail));
            }

            // Lógica de Paginação
            int totalClients = await clientsQuery.CountAsync();
            var pagination = new PaginationInfo<Client>(page, totalClients);

            pagination.Items = await clientsQuery
                .OrderBy(c => c.Name)
                .Skip(pagination.ItemsToSkip)
                .Take(pagination.ItemsPerPage)
                .ToListAsync();

            // Manter os termos de pesquisa na View para os links de paginação
            ViewBag.SearchName = searchName;
            ViewBag.SearchPhone = searchPhone;
            ViewBag.SearchEmail = searchEmail;

            return View(pagination);
        }

        // GET: Client/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var client = await _context.Client
                .Include(c => c.Membership)
                .FirstOrDefaultAsync(m => m.ClientId == id);

            if (client == null) return NotFound();

            return View(client);
        }

        // GET: Client/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Client/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Email,Phone,Address,BirthDate,Gender")] Client client)
        {
            if (ModelState.IsValid)
            {
                // Verifica se o email já existe para evitar duplicações
                if (_context.Client.Any(c => c.Email == client.Email))
                {
                    ModelState.AddModelError("Email", "This email is already registered.");
                    return View(client);
                }

                client.RegistrationDate = DateTime.Now;
                _context.Add(client);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Client successfully registered.";
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Client/Edit/5
        public async Task<IActionResult> Edit(int? id) // Alterado para int?
        {
            if (id == null) return NotFound();

            var client = await _context.Client.FindAsync(id);
            if (client == null) return NotFound();

            return View(client);
        }

        // POST: Client/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ClientId,Name,Email,Phone,Address,BirthDate,Gender,RegistrationDate")] Client client)
        {
            if (id != client.ClientId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Client information updated.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.ClientId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Client/Delete/5
        public async Task<IActionResult> Delete(int? id) // Alterado para int?
        {
            if (id == null) return NotFound();

            var client = await _context.Client
                .FirstOrDefaultAsync(m => m.ClientId == id);

            if (client == null)
            {
                TempData["ErrorMessage"] = "The client has already been removed.";
                return RedirectToAction(nameof(Index));
            }

            return View(client);
        }

        // POST: Client/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) // Alterado para int
        {
            var client = await _context.Client.FindAsync(id);
            if (client != null)
            {
                _context.Client.Remove(client);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Client successfully deleted.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
            return _context.Client.Any(e => e.ClientId == id);
        }
    }
}