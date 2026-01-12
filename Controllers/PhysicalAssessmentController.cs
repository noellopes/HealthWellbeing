using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels; // Necessário para PaginationInfo
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace HealthWellbeing.Controllers
{
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
        // ÁREA DO CLIENTE: O Meu Progresso (Gráficos e Histórico)
        // =============================================================
        public async Task<IActionResult> MyEvolution()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var member = await _context.Member
                .Include(m => m.Client)
                .FirstOrDefaultAsync(m => m.Client.Email == user.Email);

            if (member == null)
            {
                TempData["Error"] = "Perfil de membro não encontrado.";
                return RedirectToAction("Index", "Home");
            }

            // Busca histórico ordenado por data para os gráficos
            var assessments = await _context.PhysicalAssessment
                .Include(p => p.Trainer)
                .Where(p => p.MemberId == member.MemberId)
                .OrderBy(p => p.AssessmentDate)
                .ToListAsync();

            return View(assessments);
        }

        // =============================================================
        // ÁREA DE GESTÃO (STAFF): CRUD Completo
        // =============================================================

        // GET: PhysicalAssessment (Lista Paginada)
        [Authorize(Roles = "Administrator,Trainer")]
        public async Task<IActionResult> Index(int page = 1)
        {
            var query = _context.PhysicalAssessment
                .Include(p => p.Member).ThenInclude(m => m.Client)
                .Include(p => p.Trainer)
                .OrderByDescending(p => p.AssessmentDate);

            // 1. Contar Total
            int total = await query.CountAsync();

            // 2. Criar Objeto de Paginação
            var pagination = new PaginationInfo<PhysicalAssessment>(page, total, 10);

            // 3. Obter itens da página atual
            pagination.Items = await query
                .Skip(pagination.ItemsToSkip)
                .Take(pagination.ItemsPerPage)
                .ToListAsync();

            return View(pagination);
        }

        // GET: PhysicalAssessment/Details/5
        [Authorize(Roles = "Administrator,Trainer")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var physicalAssessment = await _context.PhysicalAssessment
                .Include(p => p.Member).ThenInclude(m => m.Client)
                .Include(p => p.Trainer)
                .FirstOrDefaultAsync(m => m.PhysicalAssessmentId == id);

            if (physicalAssessment == null) return NotFound();

            return View(physicalAssessment);
        }

        // GET: PhysicalAssessment/Create
        [Authorize(Roles = "Administrator,Trainer")]
        public IActionResult Create()
        {
            // Carrega as listas dropdown
            ViewData["MemberId"] = new SelectList(_context.Member.Include(m => m.Client), "MemberId", "Client.Name");
            ViewData["TrainerId"] = new SelectList(_context.Trainer, "TrainerId", "Name");
            return View();
        }

        // POST: PhysicalAssessment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Trainer")]
        public async Task<IActionResult> Create([Bind("PhysicalAssessmentId,AssessmentDate,Weight,Height,BodyFatPercentage,MuscleMass,Notes,MemberId,TrainerId")] PhysicalAssessment physicalAssessment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(physicalAssessment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // Se falhar, recarrega as listas
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

        // POST: PhysicalAssessment/Edit/5
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
            ViewData["MemberId"] = new SelectList(_context.Member.Include(m => m.Client), "MemberId", "Client.Name", physicalAssessment.MemberId);
            ViewData["TrainerId"] = new SelectList(_context.Trainer, "TrainerId", "Name", physicalAssessment.TrainerId);
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

        // POST: PhysicalAssessment/Delete/5
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