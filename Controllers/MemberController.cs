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
    [Authorize] // Segurança Base: Exige login para tudo
    public class MemberController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public MemberController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // =============================================================
        // INDEX: Lista de Membros (APENAS STAFF)
        // =============================================================
        [Authorize(Roles = "Administrator,Trainer")]
        public async Task<IActionResult> Index(int page = 1, string searchName = "", string searchPhone = "", string searchEmail = "")
        {
            var membersQuery = _context.Member
                .Include(m => m.Client)
                .Include(m => m.MemberPlans).ThenInclude(mp => mp.Plan)
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

        // =============================================================
        // DETAILS: Ver Detalhes (STAFF OU O PRÓPRIO)
        // =============================================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var member = await _context.Member
                .Include(m => m.Client)
                .Include(m => m.MemberPlans)
                    .ThenInclude(mp => mp.Plan)
                .Include(m => m.PhysicalAssessments.OrderByDescending(a => a.AssessmentDate))
                .FirstOrDefaultAsync(m => m.MemberId == id);

            if (member == null) return NotFound();

            // SEGURANÇA: Se não for Staff, tem de ser o dono da conta
            if (!IsStaff() && member.Client.Email != User.Identity.Name)
            {
                return Forbid();
            }

            return View(member);
        }

        // =============================================================
        // CREATE: Inscrever Novo Membro (APENAS STAFF)
        // =============================================================
        [Authorize(Roles = "Administrator,Trainer")]
        public IActionResult Create(int? clientId)
        {
            var clients = _context.Client.Where(c => c.Member == null).OrderBy(c => c.Name).ToList();
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
                // 1. Criar o Member
                var member = new Member { ClientId = ClientId };
                _context.Add(member);
                await _context.SaveChangesAsync();

                // 2. Criar a Subscrição
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

            ViewData["ClientId"] = new SelectList(_context.Client.Where(c => c.Member == null), "ClientId", "Name", ClientId);
            ViewData["PlanId"] = new SelectList(_context.Plan, "PlanId", "Name", PlanId);
            return View();
        }

        // =============================================================
        // EDIT: Mudar Plano (STAFF OU O PRÓPRIO)
        // =============================================================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var member = await _context.Member
                .Include(m => m.Client)
                .Include(m => m.MemberPlans).ThenInclude(mp => mp.Plan)
                .FirstOrDefaultAsync(m => m.MemberId == id);

            if (member == null) return NotFound();

            // SEGURANÇA
            if (!IsStaff() && member.Client.Email != User.Identity.Name)
            {
                return Forbid();
            }

            // Descobrir plano ativo para pré-selecionar no dropdown
            var activePlan = member.MemberPlans.FirstOrDefault(mp => mp.Status == "Active");
            int currentPlanId = activePlan?.PlanId ?? 0;

            ViewData["PlanId"] = new SelectList(_context.Plan, "PlanId", "Name", currentPlanId);
            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int NewPlanId)
        {
            var member = await _context.Member
                .Include(m => m.Client)
                .Include(m => m.MemberPlans)
                .FirstOrDefaultAsync(m => m.MemberId == id);

            if (member == null) return NotFound();

            // SEGURANÇA
            if (!IsStaff() && member.Client.Email != User.Identity.Name)
            {
                return Forbid();
            }

            // Lógica de Atualização do Plano (Reinício de Ciclo)
            var activeMemberPlan = member.MemberPlans.FirstOrDefault(mp => mp.Status == "Active");

            if (activeMemberPlan != null && NewPlanId != 0 && activeMemberPlan.PlanId != NewPlanId)
            {
                var newPlanInfo = await _context.Plan.FindAsync(NewPlanId);

                if (newPlanInfo != null)
                {
                    // 1. Atualizar o ID do Plano
                    activeMemberPlan.PlanId = NewPlanId;

                    // 2. REINICIAR O CICLO DE FATURAÇÃO
                    // A data de início passa a ser hoje
                    activeMemberPlan.StartDate = DateTime.Now;
                    // A data de fim é recalculada (Hoje + Duração do Novo Plano)
                    activeMemberPlan.EndDate = DateTime.Now.AddDays(newPlanInfo.DurationDays);

                    _context.Update(activeMemberPlan);
                    await _context.SaveChangesAsync();

                    // Mensagem de Feedback explicativa
                    TempData["SuccessMessage"] = $"Plan updated to {newPlanInfo.Name}. A new billing cycle has started today.";
                }
            }

            // Redirecionamento Inteligente
            if (!IsStaff())
            {
                return RedirectToAction(nameof(Details), new { id = member.MemberId });
            }
            return RedirectToAction(nameof(Index));
        }

        // =============================================================
        // DELETE: Cancelar Membro (APENAS STAFF)
        // =============================================================
        [Authorize(Roles = "Administrator,Trainer")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var member = await _context.Member
                .Include(m => m.Client)
                .Include(m => m.MemberPlans).ThenInclude(mp => mp.Plan)
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

        // Helper para verificar permissões de Staff
        private bool IsStaff()
        {
            return User.IsInRole("Administrator") || User.IsInRole("Trainer");
        }
    }
}