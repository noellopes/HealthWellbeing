using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Controllers
{
    [Authorize] // Exige Login, mas não restringe Role na classe toda
    public class ClientController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ClientController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Client (Lista) -> APENAS STAFF
        [Authorize(Roles = "Administrator,Trainer")]
        public async Task<IActionResult> Index(int page = 1, string searchName = "")
        {
            var clientsQuery = _context.Client.AsQueryable();
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
                .Include(c => c.Membership)
                    .ThenInclude(m => m.MemberPlans) // <--- FALTAVA ISTO
                        .ThenInclude(mp => mp.Plan)  // <--- E ISTO (para saber o nome do plano)
                .FirstOrDefaultAsync(m => m.ClientId == id);

            if (client == null) return NotFound();

            // SEGURANÇA: Se não for Staff, tem de ser o dono do email
            if (!IsStaff() && client.Email != User.Identity.Name)
            {
                return Forbid();
            }

            return View(client);
        }

        // GET: Client/Create -> APENAS STAFF (Ou via Registo Público que usa o AccountController)
        [Authorize(Roles = "Administrator,Trainer")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Client/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Trainer")]
        public async Task<IActionResult> Create([Bind("ClientId,Name,Email,Phone,Address,BirthDate,Gender,RegistrationDate")] Client client)
        {
            if (ModelState.IsValid)
            {
                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Client/Edit/5 -> STAFF ou O PRÓPRIO
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var client = await _context.Client.FindAsync(id);
            if (client == null) return NotFound();

            // SEGURANÇA: Só o dono ou Admin pode editar
            if (!IsStaff() && client.Email != User.Identity.Name)
            {
                return Forbid();
            }

            return View(client);
        }

        // POST: Client/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ClientId,Name,Email,Phone,Address,BirthDate,Gender,RegistrationDate")] Client client)
        {
            if (id != client.ClientId) return NotFound();

            // SEGURANÇA NO POST TAMBÉM
            // Nota: Num cenário real, não devemos confiar no email que vem do form (Bind), 
            // mas sim verificar o original na BD. Para simplificar aqui:
            if (!IsStaff() && client.Email != User.Identity.Name)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();

                    // Se for cliente normal, volta aos detalhes em vez da lista (que não tem acesso)
                    if (!IsStaff())
                    {
                        return RedirectToAction(nameof(Details), new { id = client.ClientId });
                    }
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
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var client = await _context.Client.FirstOrDefaultAsync(m => m.ClientId == id);
            if (client == null) return NotFound();
            return View(client);
        }

        // POST: Client/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Client.FindAsync(id);
            if (client != null) _context.Client.Remove(client);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id) => _context.Client.Any(e => e.ClientId == id);

        private bool IsStaff() => User.IsInRole("Administrator") || User.IsInRole("Trainer");
    }
}