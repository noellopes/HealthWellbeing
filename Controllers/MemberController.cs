using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class MemberController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public MemberController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // --- EXISTING ACTIONS (Index, Details, Edit, Delete, etc. are retained) ---

        // GET: Member
        public async Task<IActionResult> Index()
        {
            // Eager load Client data for better display in the Member Index, if needed
            var members = _context.Member.Include(m => m.Client);
            return View(await members.ToListAsync());
        }

        // GET: Member/Details/5 (Assuming you want to include Client data here too)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Member
                .Include(m => m.Client) // Include the Client data
                .FirstOrDefaultAsync(m => m.MemberId == id);

            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // --- NEW/MODIFIED CREATE ACTIONS FOR "MAKE MEMBER" ---

        // GET: Member/Create?clientId={clientId}
        // This action receives the ClientId from the Client Index page
        public IActionResult Create(int clientId)  // Accepting clientId as an int
        {
            if (clientId == 0)
            {
                // If the link was manually entered without a valid ClientId
                TempData["Message"] = "Client ID is required to create a membership.";
                return RedirectToAction("Index", "Client");
            }

            // 1. Find the Client
            var client = _context.Client.Find(clientId);
            if (client == null)
            {
                TempData["Message"] = $"Client ID '{clientId}' not found.";
                return RedirectToAction("Index", "Client");
            }

            // 2. Check if the client is ALREADY a member
            if (_context.Member.Any(m => m.ClientId == clientId))
            {
                // Redirect back to client list or details with a warning
                TempData["Message"] = $"Client **{client.Name}** is already an active member.";
                TempData["MessageType"] = "warning"; // Use TempData to signal a warning type
                return RedirectToAction("Index", "Client");
            }

            // Initialize the Member object with the foreign key (ClientId)
            var member = new Member { ClientId = clientId };

            // Pass the Client name/details to the view using ViewBag for display
            ViewBag.ClientName = client.Name;

            return View(member);
        }

        // POST: Member/Create
        // To protect from overposting attacks, we only bind the ClientId
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClientId")] Member member)
        {
            // We rely on the ClientId being present from the hidden field in the form
            if (member.ClientId == 0) // Checking if ClientId is 0 (invalid)
            {
                ModelState.AddModelError("ClientId", "Client ID is missing.");
            }

            // Ensure we retrieve the client object before saving, in case we need the name for the message
            var client = await _context.Client.FindAsync(member.ClientId);

            if (ModelState.IsValid && client != null)
            {
                // Since MemberId is the primary key and identity, we only set the foreign key ClientId
                member.Client = client; // Attach the client object for navigation property to be correctly tracked (optional but helpful)

                _context.Add(member);
                await _context.SaveChangesAsync();

                // Success message
                TempData["Message"] = $"Membership successfully created for Client: **{client.Name}**";
                TempData["MessageType"] = "success";

                // Redirect back to the Client Index page to see the new status
                return RedirectToAction("Index", "Client");
            }

            // If validation fails, re-populate ViewBag for view display
            ViewBag.ClientName = client?.Name ?? "Unknown Client";

            return View(member);
        }

        // --- EXISTING ACTIONS (Edit, Delete, Exists) ---

        // GET: Member/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // ... (keep original logic) ...
            if (id == null) return NotFound();
            var member = await _context.Member.FindAsync(id);
            if (member == null) return NotFound();
            return View(member);
        }

        // POST: Member/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MemberId,ClientId")] Member member)
        {
            // ... (keep original logic) ...
            if (id != member.MemberId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(member);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MemberExists(member.MemberId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        // GET: Member/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            // ... (keep original logic) ...
            if (id == null) return NotFound();
            var member = await _context.Member.FirstOrDefaultAsync(m => m.MemberId == id);
            if (member == null) return NotFound();
            return View(member);
        }

        // POST: Member/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // ... (keep original logic) ...
            var member = await _context.Member.FindAsync(id);
            if (member != null)
            {
                _context.Member.Remove(member);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MemberExists(int id)
        {
            return _context.Member.Any(e => e.MemberId == id);
        }
    }
}
