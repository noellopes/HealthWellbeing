using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellbeing.Controllers
{
    // Por segurança, exigimos login por defeito em todo o controlador.
    // As exceções (páginas públicas) terão [AllowAnonymous].
    [Authorize]
    public class PlanController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public PlanController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // =================================================================
        // ÁREA DE GESTÃO (ADMINISTRADOR APENAS)
        // =================================================================

        // GET: Plan (Lista de Gestão)
        // Apenas o Administrador pode ver a tabela de gestão de planos
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Index(
            int page = 1,
            string searchName = "",
            decimal? searchMinPrice = null,
            decimal? searchMaxPrice = null,
            int? searchDuration = null)
        {
            var plansQuery = _context.Plan.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchName))
            {
                plansQuery = plansQuery.Where(p => p.Name.Contains(searchName));
            }

            if (searchMinPrice.HasValue)
            {
                plansQuery = plansQuery.Where(p => p.Price >= searchMinPrice.Value);
            }

            if (searchMaxPrice.HasValue)
            {
                plansQuery = plansQuery.Where(p => p.Price <= searchMaxPrice.Value);
            }

            if (searchDuration.HasValue)
            {
                plansQuery = plansQuery.Where(p => p.DurationDays == searchDuration.Value);
            }

            ViewBag.SearchName = searchName;
            ViewBag.SearchMinPrice = searchMinPrice;
            ViewBag.SearchMaxPrice = searchMaxPrice;
            ViewBag.SearchDuration = searchDuration;

            int totalPlans = await plansQuery.CountAsync();

            var plansInfo = new PaginationInfo<Plan>(page, totalPlans, 9);

            plansInfo.Items = await plansQuery
                .OrderBy(p => p.Name)
                .Skip(plansInfo.ItemsToSkip)
                .Take(plansInfo.ItemsPerPage)
                .ToListAsync();

            return View(plansInfo);
        }

        // GET: Plan/Details/5
        // Detalhes técnicos do plano (vista de admin)
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var plan = await _context.Plan.FirstOrDefaultAsync(m => m.PlanId == id);
            if (plan == null) return NotFound();

            return View(plan);
        }

        // GET: Plan/Create
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Plan/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([Bind("Name,Description,Price,DurationDays")] Plan plan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(plan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }

        // GET: Plan/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var plan = await _context.Plan.FindAsync(id);
            if (plan == null) return NotFound();

            return View(plan);
        }

        // POST: Plan/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id, [Bind("PlanId,Name,Description,Price,DurationDays")] Plan plan)
        {
            if (id != plan.PlanId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(plan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlanExists(plan.PlanId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }

        // GET: Plan/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var plan = await _context.Plan.FirstOrDefaultAsync(m => m.PlanId == id);
            if (plan == null) return NotFound();

            return View(plan);
        }

        // POST: Plan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var plan = await _context.Plan.FindAsync(id);
            if (plan != null)
            {
                _context.Plan.Remove(plan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // =================================================================
        // ÁREA PÚBLICA E DO CLIENTE (ACESSO PERMITIDO)
        // =================================================================

        // GET: Plan/PublicIndex
        // Esta é a "montra" de planos. Aberta a todos (anónimos incluídos).
        [AllowAnonymous]
        public async Task<IActionResult> PublicIndex(int page = 1)
        {
            var plansQuery = _context.Plan.AsQueryable();

            int totalPlans = await plansQuery.CountAsync();
            var pagination = new PaginationInfo<Plan>(page, totalPlans, 6);

            pagination.Items = await plansQuery
                .OrderBy(p => p.Price)
                .Skip(pagination.ItemsToSkip)
                .Take(pagination.ItemsPerPage)
                .ToListAsync();

            return View(pagination);
        }

        // GET: Plan/Subscribe/5
        // Apenas utilizadores autenticados podem subscrever
        [Authorize]
        public async Task<IActionResult> Subscribe(int id)
        {
            var userEmail = User.Identity?.Name;

            // Buscar o cliente e os seus planos atuais
            var client = await _context.Client
                .Include(c => c.Membership)
                .ThenInclude(m => m.MemberPlans)
                .FirstOrDefaultAsync(c => c.Email == userEmail);

            if (client == null)
            {
                // Se o utilizador Identity existe mas não tem perfil Client, manda criar
                return RedirectToAction("Create", "Client");
            }

            // REGRA DE NEGÓCIO: Impedir subscrição se já tiver plano Ativo
            if (client.Membership != null && client.Membership.MemberPlans.Any(mp => mp.Status == "Active"))
            {
                var activePlan = client.Membership.MemberPlans.First(mp => mp.Status == "Active");

                TempData["Message"] = $"You already have an active subscription ({activePlan.Plan?.Name ?? "Plan"}). You cannot subscribe to a new plan until the current one expires.";
                TempData["MessageType"] = "warning";

                return RedirectToAction("PublicIndex");
            }

            ViewBag.Plan = await _context.Plan.FindAsync(id);
            return View(client);
        }

        // POST: Plan/ConfirmSubscription
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ConfirmSubscription(int planId, int clientId)
        {
            var plan = await _context.Plan.FindAsync(planId);
            var client = await _context.Client
                .Include(c => c.Membership)
                .FirstOrDefaultAsync(c => c.ClientId == clientId);

            if (plan == null || client == null) return NotFound();

            // 1. Garantir que existe registo na tabela Member
            if (client.Membership == null)
            {
                client.Membership = new Member { ClientId = client.ClientId };
                _context.Member.Add(client.Membership);
                await _context.SaveChangesAsync();
            }

            // 2. Criar a Inscrição (MemberPlan)
            var memberPlan = new MemberPlan
            {
                MemberId = client.Membership.MemberId,
                PlanId = plan.PlanId,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(plan.DurationDays),
                Status = "Active"
            };

            _context.MemberPlan.Add(memberPlan);
            await _context.SaveChangesAsync();

            // 3. Feedback e Redirecionamento
            TempData["Message"] = $"Success! You subscribed to <strong>{plan.Name}</strong>.";
            TempData["MessageType"] = "success";

            return RedirectToAction("Details", "Member", new { id = client.Membership.MemberId });
        }

        private bool PlanExists(int id)
        {
            return _context.Plan.Any(e => e.PlanId == id);
        }
    }
}