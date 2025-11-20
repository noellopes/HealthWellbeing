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
        public async Task<IActionResult> Index(int page = 1, string searchName = "", string searchPhone = "", string searchEmail = "", string searchMember = "")
        {
            var clientsQuery = _context.Client
                .Include(c => c.Membership)
                .AsQueryable();

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

            if (!string.IsNullOrEmpty(searchMember))
            {
                if (searchMember == "Yes")
                {
                    clientsQuery = clientsQuery.Where(c => c.Membership != null);
                }
                else if (searchMember == "No")
                {
                    clientsQuery = clientsQuery.Where(c => c.Membership == null);
                }
            }

            ViewBag.SearchName = searchName;
            ViewBag.SearchPhone = searchPhone;
            ViewBag.SearchEmail = searchEmail;
            ViewBag.SearchMember = searchMember;

            int numberClients = await clientsQuery.CountAsync();

            var clientsInfo = new ViewModels.PaginationInfo<Client>(page, numberClients, 5);

            clientsInfo.Items = await clientsQuery
                .OrderBy(c => c.Name)
                .Skip(clientsInfo.ItemsToSkip)
                .Take(clientsInfo.ItemsPerPage)
                .ToListAsync();

            return View(clientsInfo);
        }

        // GET: Client/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client
                .FirstOrDefaultAsync(m => m.ClientId == id);

            if (client == null)
            {
                return View("InvalidClient");
            }

            return View(client);
        }

        // GET: Client/Create
        public IActionResult Create()
        {
            return View();
        }

		// POST: Client/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Name,Email,Phone,Address,BirthDate,Gender")] Client client)
		{
			if (ModelState.IsValid)
			{
				// Geração do ClientID aqui, ANTES de adicionar ao DBContext
				client.ClientId = Guid.NewGuid().ToString("N");
				client.RegistrationDate = DateTime.Now; // Defina a data de registo aqui também

				_context.Add(client);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(ClientCreatedConfirmation), new { clientId = client.ClientId, clientName = client.Name });
			}
			return View(client);
		}
		public IActionResult ClientCreatedConfirmation(string clientId, string clientName)
		{
			// Passa o ID e Nome para a View
			ViewBag.ClientId = clientId;
			ViewBag.ClientName = clientName;
			return View();
		}

		// GET: Client/Edit/5
		public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        // POST: Client/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Name,Email,Phone,Address,BirthDate,Gender,Membership")] Client client)
        {
            if (id != client.ClientId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.ClientId))
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
            return View(client);
        }

        // GET: Client/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client
                .FirstOrDefaultAsync(m => m.ClientId == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Client/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var client = await _context.Client.FindAsync(id);
            if (client != null)
            {
                _context.Client.Remove(client);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(string id)
        {
            return _context.Client.Any(e => e.ClientId == id);
        }
    }
}
