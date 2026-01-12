using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellbeing.Controllers
{
    // Acesso geral para Staff. Clientes só entram na action MyEvolution.
    [Authorize]
    public class PhysicalAssessmentController : Controller
    {
        private readonly HealthWellbeingDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PhysicalAssessmentController(HealthWellbeingDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // =============================================================
        // ÁREA DO CLIENTE: Minha Evolução
        // =============================================================
        public async Task<IActionResult> MyEvolution()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            // Procura o membro associado ao email do utilizador logado
            var member = await _context.Member
                .Include(m => m.Client)
                .FirstOrDefaultAsync(m => m.Client.Email == user.Email);

            if (member == null)
            {
                TempData["Error"] = "Member profile not found.";
                return RedirectToAction("Index", "Home");
            }

            // Vai buscar todas as avaliações deste membro, da mais recente para a mais antiga
            var assessments = await _context.PhysicalAssessment
                .Include(p => p.Trainer)
                .Where(p => p.MemberId == member.MemberId)
                .OrderByDescending(p => p.AssessmentDate)
                .ToListAsync();

            return View(assessments);
        }

        // =============================================================
        // ÁREA DE GESTÃO (STAFF APENAS)
        // =============================================================

        // GET: PhysicalAssessment
        [Authorize(Roles = "Administrator,Trainer")]
        public async Task<IActionResult> Index()
        {
            var assessments = _context.PhysicalAssessment
                .Include(p => p.Member)
                    .ThenInclude(m => m.Client)
                .Include(p => p.Trainer);

            // Retorna uma lista simples para evitar o erro de Paginação (PaginationInfo)
            return View(await assessments.ToListAsync());
        }

        // GET: PhysicalAssessment/Details/5
        [Authorize(Roles = "Administrator,Trainer")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var physicalAssessment = await _context.PhysicalAssessment
                .Include(p => p.Member)
                    .ThenInclude(m => m.Client)
                .Include(p => p.Trainer)
                .FirstOrDefaultAsync(m => m.PhysicalAssessmentId == id);

            if (physicalAssessment == null) return NotFound();

            return View(physicalAssessment);
        }

        // GET: PhysicalAssessment/Create
        [Authorize(Roles = "Administrator,Trainer")]
        public IActionResult Create()
        {
            ViewData["MemberId"] = new SelectList(_context.Member.Include(m => m.Client), "MemberId", "Client.Name");
            ViewData["TrainerId"] = new SelectList(_context.Trainer, "TrainerId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Trainer")]
        public async Task<IActionResult> Create([Bind("PhysicalAssessmentId,AssessmentDate,Weight,Height,BodyFatPercentage,MuscleMass,Notes,MemberId,TrainerId")] PhysicalAssessment physicalAssessment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(physicalAssessment);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Assessment created successfully!";
                return RedirectToAction(nameof(Index));
            }
            // Repreenche as listas se houver erro
            ViewData["MemberId"] = new SelectList(_context.Member.Include(m => m.Client), "MemberId", "Client.Name", physicalAssessment.MemberId);
            ViewData["TrainerId"] = new SelectList(_context.Trainer, "TrainerId", "Name", physicalAssessment.TrainerId);
            return View(physicalAssessment);
        }

        // GET: PhysicalAssessment/Edit/5
        [Authorize(Roles = "Administrator,Trainer")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var physicalAssessment = await _context.PhysicalAssessment.FindAsync(id);
            if (physicalAssessment == null) return NotFound();

            ViewData["MemberId"] = new SelectList(_context.Member.Include(m => m.Client), "MemberId", "Client.Name", physicalAssessment.MemberId);
            ViewData["TrainerId"] = new SelectList(_context.Trainer, "TrainerId", "Name", physicalAssessment.TrainerId);
            return View(physicalAssessment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Trainer")]
        public async Task<IActionResult> Edit(int id, [Bind("PhysicalAssessmentId,AssessmentDate,Weight,Height,BodyFatPercentage,MuscleMass,Notes,MemberId,TrainerId")] PhysicalAssessment physicalAssessment)
        {
            if (id != physicalAssessment.PhysicalAssessmentId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(physicalAssessment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhysicalAssessmentExists(physicalAssessment.PhysicalAssessmentId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(physicalAssessment);
        }

        // GET: PhysicalAssessment/Delete/5
        [Authorize(Roles = "Administrator,Trainer")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var physicalAssessment = await _context.PhysicalAssessment
                .Include(p => p.Member).ThenInclude(m => m.Client)
                .Include(p => p.Trainer)
                .FirstOrDefaultAsync(m => m.PhysicalAssessmentId == id);

            if (physicalAssessment == null) return NotFound();

            return View(physicalAssessment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Trainer")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var physicalAssessment = await _context.PhysicalAssessment.FindAsync(id);
            if (physicalAssessment != null)
            {
                _context.PhysicalAssessment.Remove(physicalAssessment);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PhysicalAssessmentExists(int id)
        {
            return _context.PhysicalAssessment.Any(e => e.PhysicalAssessmentId == id);
        }
    }
}