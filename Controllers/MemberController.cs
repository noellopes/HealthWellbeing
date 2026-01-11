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
using Microsoft.AspNetCore.Authorization;

namespace HealthWellbeing.Controllers
{
    [Authorize] // Exige que o utilizador esteja logado para qualquer ação
    public class MemberController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public MemberController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Administrator,Trainer")]
        public async Task<IActionResult> Index(int page = 1, string searchName = "", string searchPhone = "", string searchEmail = "")
        {
            var membersQuery = _context.Member
                .Include(m => m.Client)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchName))
                membersQuery = membersQuery.Where(m => m.Client != null && m.Client.Name.Contains(searchName));

            if (!string.IsNullOrEmpty(searchPhone))
                membersQuery = membersQuery.Where(m => m.Client != null && m.Client.Phone.Contains(searchPhone));

            if (!string.IsNullOrEmpty(searchEmail))
                membersQuery = membersQuery.Where(m => m.Client != null && m.Client.Email.Contains(searchEmail));

            int totalMembers = await membersQuery.CountAsync();
            var pagination = new PaginationInfo<Member>(page, totalMembers);

            pagination.Items = await membersQuery
                .OrderBy(m => m.Client != null ? m.Client.Name : "")
                .Skip(pagination.ItemsToSkip)
                .Take(pagination.ItemsPerPage)
                .ToListAsync();

            ViewBag.SearchName = searchName;
            ViewBag.SearchPhone = searchPhone;
            ViewBag.SearchEmail = searchEmail;

            return View(pagination);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var member = await _context.Member
                .Include(m => m.Client)
                .Include(m => m.MemberPlans)
                    .ThenInclude(mp => mp.Plan)
                .FirstOrDefaultAsync(m => m.MemberId == id);

            if (member == null) return NotFound();

            // SEGURANÇA: Se não for Staff, tem de ser o dono da conta (Email igual ao Login)
            if (!IsStaff() && member.Client.Email != User.Identity.Name)
            {
                return Forbid(); // Acesso Negado
            }

            return View(member);
        }

        [Authorize(Roles = "Administrator,Trainer")]
        public IActionResult Create(int? clientId)
        {
            var clients = _context.Client.Where(c => c.Membership == null).OrderBy(c => c.Name).ToList();
            var plans = _context.Plan.OrderBy(p => p.Price).ToList();

            ViewData["ClientId"] = new SelectList(clients, "ClientId", "Name", clientId);
            ViewData["PlanId"] = new SelectList(plans, "PlanId", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Trainer")]
        public async Task<IActionResult> Create(int ClientId, int PlanId)
        {
            if (ClientId == 0 || PlanId == 0)
            {
                ModelState.AddModelError("", "You must select both a Client and a Plan.");
            }

            if (ModelState.IsValid)
            {
                var member = new Member { ClientId = ClientId };
                _context.Add(member);
                await _context.SaveChangesAsync();

                var plan = await _context.Plan.FindAsync(PlanId);
                if (plan != null)
                {
                    var memberPlan = new MemberPlan
                    {
                        MemberId = member.MemberId,
                        PlanId = PlanId,
                        StartDate = DateTime.Now,
                        EndDate = DateTime.Now.AddDays(plan.DurationDays),
                        Status = "Active"
                    };
                    _context.Add(memberPlan);
                    await _context.SaveChangesAsync();
                }

                TempData["Message"] = "Membership created successfully.";
                TempData["MessageType"] = "success";
                return RedirectToAction(nameof(Index));
            }

            ViewData["ClientId"] = new SelectList(_context.Client.Where(c => c.Membership == null), "ClientId", "Name", ClientId);
            ViewData["PlanId"] = new SelectList(_context.Plan, "PlanId", "Name", PlanId);
            return View();
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var member = await _context.Member
                .Include(m => m.Client) // Necessário para validar o Email
                .FirstOrDefaultAsync(m => m.MemberId == id);

            if (member == null) return NotFound();

            // SEGURANÇA: Só Staff ou o próprio dono podem editar
            if (!IsStaff() && member.Client.Email != User.Identity.Name)
            {
                return Forbid();
            }

            // Nota: Se for o cliente a editar, o dropdown de ClientId devia estar desativado na View, 
            // mas aqui carregamos a lista na mesma para o Admin poder mudar se quiser.
            ViewData["ClientId"] = new SelectList(_context.Client, "ClientId", "Name", member.ClientId);
            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MemberId,ClientId")] Member member)
        {
            if (id != member.MemberId) return NotFound();

            // SEGURANÇA NO POST: Verificar permissões novamente antes de gravar
            // Buscamos o membro original (sem tracking) para conferir quem é o dono
            var originalMember = await _context.Member
                .Include(m => m.Client)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.MemberId == id);

            if (originalMember == null) return NotFound();

            if (!IsStaff() && originalMember.Client.Email != User.Identity.Name)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(member);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Membership updated successfully.";

                    if (!IsStaff())
                    {
                        return RedirectToAction(nameof(Details), new { id = member.MemberId });
                    }
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

        [Authorize(Roles = "Administrator,Trainer")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var member = await _context.Member
                .Include(m => m.Client)
                .Include(m => m.MemberPlans)
                    .ThenInclude(mp => mp.Plan)
                .FirstOrDefaultAsync(m => m.MemberId == id);

            if (member == null) return RedirectToAction(nameof(Index));

            return View(member);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Trainer")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var member = await _context.Member.FindAsync(id);
            if (member != null)
            {
                _context.Member.Remove(member);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Membership terminated successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool MemberExists(int id)
        {
            return _context.Member.Any(e => e.MemberId == id);
        }

        // Função auxiliar para verificar se é Admin ou Trainer
        private bool IsStaff()
        {
            return User.IsInRole("Administrator") || User.IsInRole("Trainer");
        }
    }
}