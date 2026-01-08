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
    public class MemberController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public MemberController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Member
        // Implementada a paginação e pesquisa seguindo o padrão do BooksController
        public async Task<IActionResult> Index(int page = 1, string searchName = "", string searchPhone = "", string searchEmail = "")
        {
            var membersQuery = _context.Member
                .Include(m => m.Client)
                .AsQueryable();

            // Filtros de Pesquisa através da entidade Client (Relacionamento 1:1)
            if (!string.IsNullOrEmpty(searchName))
            {
                membersQuery = membersQuery.Where(m => m.Client != null && m.Client.Name.Contains(searchName));
            }

            if (!string.IsNullOrEmpty(searchPhone))
            {
                membersQuery = membersQuery.Where(m => m.Client != null && m.Client.Phone.Contains(searchPhone));
            }

            if (!string.IsNullOrEmpty(searchEmail))
            {
                membersQuery = membersQuery.Where(m => m.Client != null && m.Client.Email.Contains(searchEmail));
            }

            // Lógica de Paginação usando o ViewModel Genérico
            int totalMembers = await membersQuery.CountAsync();
            var pagination = new PaginationInfo<Member>(page, totalMembers);

            pagination.Items = await membersQuery
                .OrderBy(m => m.Client != null ? m.Client.Name : "")
                .Skip(pagination.ItemsToSkip)
                .Take(pagination.ItemsPerPage)
                .ToListAsync();

            // Guardar termos de pesquisa para a View (Links de paginação)
            ViewBag.SearchName = searchName;
            ViewBag.SearchPhone = searchPhone;
            ViewBag.SearchEmail = searchEmail;

            return View(pagination);
        }

        // GET: Member/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var member = await _context.Member
                .Include(m => m.Client)
                .FirstOrDefaultAsync(m => m.MemberId == id);

            if (member == null) return NotFound();

            return View(member);
        }

        // GET: Member/Create
        public IActionResult Create()
        {
            // Lista apenas clientes que ainda não são membros (opcional, para integridade)
            var clientsWithoutMembership = _context.Client
                .Where(c => c.Membership == null)
                .ToList();

            ViewData["ClientId"] = new SelectList(clientsWithoutMembership, "ClientId", "Name");
            return View();
        }

        // POST: Member/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MemberId,ClientId")] Member member)
        {
            if (ModelState.IsValid)
            {
                _context.Add(member);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Membership successfully created.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Client, "ClientId", "Name", member.ClientId);
            return View(member);
        }

        // GET: Member/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var member = await _context.Member.Include(m => m.Client).FirstOrDefaultAsync(m => m.MemberId == id);
            if (member == null) return NotFound();

            ViewData["ClientId"] = new SelectList(_context.Client, "ClientId", "Name", member.ClientId);
            return View(member);
        }

        // POST: Member/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MemberId,ClientId")] Member member)
        {
            if (id != member.MemberId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(member);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Membership information updated.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MemberExists(member.MemberId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Client, "ClientId", "Name", member.ClientId);
            return View(member);
        }

        // GET: Member/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var member = await _context.Member
                .Include(m => m.Client)
                .FirstOrDefaultAsync(m => m.MemberId == id);

            if (member == null)
            {
                TempData["SuccessMessage"] = "This membership has already been removed.";
                return RedirectToAction(nameof(Index));
            }

            return View(member);
        }

        // POST: Member/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var member = await _context.Member.FindAsync(id);
            if (member != null)
            {
                _context.Member.Remove(member);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Membership successfully deleted.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool MemberExists(int id)
        {
            return _context.Member.Any(e => e.MemberId == id);
        }
    }
}