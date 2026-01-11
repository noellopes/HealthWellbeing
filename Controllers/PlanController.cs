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
	public class PlanController : Controller
	{
		private readonly HealthWellbeingDbContext _context;

		public PlanController(HealthWellbeingDbContext context)
		{
			_context = context;
		}

        // GET: Plan
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
        public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var plan = await _context.Plan
				.FirstOrDefaultAsync(m => m.PlanId == id);

			if (plan == null)
			{
				return NotFound();
			}

			return View(plan);
		}

		// GET: Plan/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: Plan/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
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
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var plan = await _context.Plan.FindAsync(id);
			if (plan == null)
			{
				return NotFound();
			}
			return View(plan);
		}

		// POST: Plan/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("PlanId,Name,Description,Price,DurationDays")] Plan plan)
		{
			if (id != plan.PlanId)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(plan);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!PlanExists(plan.PlanId))
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
			return View(plan);
		}

		// GET: Plan/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var plan = await _context.Plan
				.FirstOrDefaultAsync(m => m.PlanId == id);
			if (plan == null)
			{
				return NotFound();
			}

			return View(plan);
		}

		// POST: Plan/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
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

        // GET: Plan/PublicIndex
        [AllowAnonymous]
        public async Task<IActionResult> PublicIndex(int page = 1)
        {
            var plansQuery = _context.Plan.AsQueryable();

            // 1. Contar o total de planos
            int totalPlans = await plansQuery.CountAsync();

            // 2. Configurar a paginação
            var pagination = new PaginationInfo<Plan>(page, totalPlans, 6);

            // 3. Obter apenas os planos da página atual
            pagination.Items = await plansQuery
                .OrderBy(p => p.Price)
                .Skip(pagination.ItemsToSkip)
                .Take(pagination.ItemsPerPage)
                .ToListAsync();

            // 4. Enviar o objeto de paginação para a vista
            return View(pagination);
        }

        // GET: Plan/Subscribe/5
        // Chamado quando clica em "Subscribe Now" na lista pública
        [Authorize] // Só funciona se estiver logado
        public async Task<IActionResult> Subscribe(int id)
        {
            // 1. Verificar se o plano existe
            var plan = await _context.Plan.FindAsync(id);
            if (plan == null) return NotFound();

            // 2. Encontrar o Cliente ligado ao utilizador atual
            var userEmail = User.Identity?.Name;
            var client = await _context.Client
                .Include(c => c.Membership)
                .FirstOrDefaultAsync(c => c.Email == userEmail);

            // Se por acaso o user não tiver perfil de cliente (erro de dados), manda criar
            if (client == null)
            {
                TempData["Message"] = "Please complete your client profile before subscribing.";
                return RedirectToAction("Create", "Client");
            }

            // 3. Passar dados para a vista de confirmação
            ViewBag.Plan = plan; // Passamos o plano no ViewBag
            return View(client); // Passamos o cliente como Model principal
        }

        // POST: Plan/ConfirmSubscription
        // Chamado quando clica em "Confirm & Pay"
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

            // 1. Criar o registo de Membro se ainda não existir
            if (client.Membership == null)
            {
                client.Membership = new Member { ClientId = client.ClientId };
                _context.Member.Add(client.Membership);
                await _context.SaveChangesAsync(); // Gravar para gerar o MemberId
            }

            // 2. Criar a Inscrição (MemberPlan) com estado "Active"
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

            // 3. Sucesso! Redirecionar para o Dashboard do Membro
            TempData["Message"] = $"Success! You subscribed to <strong>{plan.Name}</strong>.";
            TempData["MessageType"] = "success";

            // Redireciona para os detalhes do membro (o Dashboard do utilizador)
            return RedirectToAction("Details", "Member", new { id = client.Membership.MemberId });
        }

        private bool PlanExists(int id)
		{
			return _context.Plan.Any(e => e.PlanId == id);
		}
	}
}
