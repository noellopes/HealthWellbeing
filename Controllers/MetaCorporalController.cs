using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;

namespace HealthWellbeing.Controllers
{
    [Authorize]
    public class MetaCorporalController : Controller
    {
        private readonly HealthWellbeingDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private const int PageSize = 10;

        public MetaCorporalController(HealthWellbeingDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: MetaCorporal
        public async Task<IActionResult> Index(string? searchString, bool? apenasAtivos, int page = 1)
        {
            var userId = _userManager.GetUserId(User);

            IQueryable<MetaCorporal> query = _context.MetaCorporal
                .Include(m => m.Client);

            if (User.IsInRole("Nutricionista"))
            {
                // Nutritionist sees all goals
            }
            else if (User.IsInRole("Cliente"))
            {
                // Client sees only their own goals
                var client = await _context.Client.FirstOrDefaultAsync(c => c.IdentityUserId == userId);
                if (client == null)
                {
                    return View(new MetaCorporalIndexViewModel 
                    { 
                        Metas = new List<MetaCorporal>(), 
                        PageIndex = 1, 
                        TotalPages = 0, 
                        TotalCount = 0, 
                        PageSize = PageSize 
                    });
                }
                query = query.Where(m => m.ClientId == client.ClientId);
            }
            else
            {
                return Forbid();
            }

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(m => m.Client != null && m.Client.Name.Contains(searchString));
            }

            // Apply active filter
            if (apenasAtivos == true)
            {
                query = query.Where(m => m.Ativo);
            }

            // Store filter values for view
            ViewData["CurrentSearch"] = searchString;
            ViewData["ApenasAtivos"] = apenasAtivos;

            var totalCount = await query.CountAsync();
            var metas = await query
                .OrderByDescending(m => m.CriadoEm)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            var viewModel = new MetaCorporalIndexViewModel
            {
                Metas = metas,
                PageIndex = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize),
                TotalCount = totalCount,
                PageSize = PageSize
            };

            return View(viewModel);
        }

        // GET: MetaCorporal/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var meta = await _context.MetaCorporal
                .Include(m => m.Client)
                .FirstOrDefaultAsync(m => m.MetaCorporalId == id);

            if (meta == null) return NotFound();

            // Authorization check
            if (!await CanAccessMeta(meta))
                return Forbid();

            return View(meta);
        }

        // GET: MetaCorporal/Create
        [Authorize(Roles = "Nutricionista")]
        public async Task<IActionResult> Create()
        {
            ViewData["ClientId"] = new SelectList(await _context.Client.ToListAsync(), "ClientId", "Name");
            return View();
        }

        // POST: MetaCorporal/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Nutricionista")]
        public async Task<IActionResult> Create([Bind("ClientId,PesoObjetivo,GorduraCorporalObjetivo,ColesterolObjetivo,IMCObjetivo,MassaMuscularObjetivo,DataInicio,DataObjetivo,Notas,Ativo")] MetaCorporal meta)
        {
            var userId = _userManager.GetUserId(User);
            meta.CriadoEm = DateTime.Now;
            meta.CriadoPor = userId;

            ModelState.Remove("CriadoPor");

            if (ModelState.IsValid)
            {
                _context.Add(meta);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Meta corporal criada com sucesso!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["ClientId"] = new SelectList(await _context.Client.ToListAsync(), "ClientId", "Name", meta.ClientId);
            return View(meta);
        }

        // GET: MetaCorporal/Edit/5
        [Authorize(Roles = "Nutricionista")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var meta = await _context.MetaCorporal.FindAsync(id);
            if (meta == null) return NotFound();

            ViewData["ClientId"] = new SelectList(await _context.Client.ToListAsync(), "ClientId", "Name", meta.ClientId);
            return View(meta);
        }

        // POST: MetaCorporal/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Nutricionista")]
        public async Task<IActionResult> Edit(int id, [Bind("MetaCorporalId,ClientId,PesoObjetivo,GorduraCorporalObjetivo,ColesterolObjetivo,IMCObjetivo,MassaMuscularObjetivo,DataInicio,DataObjetivo,Notas,CriadoEm,CriadoPor,Ativo")] MetaCorporal meta)
        {
            if (id != meta.MetaCorporalId) return NotFound();

            var userId = _userManager.GetUserId(User);
            meta.AtualizadoEm = DateTime.Now;
            meta.AtualizadoPor = userId;

            ModelState.Remove("CriadoPor");
            ModelState.Remove("AtualizadoPor");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(meta);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Meta corporal atualizada com sucesso!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MetaCorporalExists(meta.MetaCorporalId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["ClientId"] = new SelectList(await _context.Client.ToListAsync(), "ClientId", "Name", meta.ClientId);
            return View(meta);
        }

        // GET: MetaCorporal/Delete/5
        [Authorize(Roles = "Nutricionista")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var meta = await _context.MetaCorporal
                .Include(m => m.Client)
                .FirstOrDefaultAsync(m => m.MetaCorporalId == id);

            if (meta == null) return NotFound();

            return View(meta);
        }

        // POST: MetaCorporal/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Nutricionista")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var meta = await _context.MetaCorporal.FindAsync(id);
            if (meta != null)
            {
                _context.MetaCorporal.Remove(meta);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Meta corporal eliminada com sucesso!";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> CanAccessMeta(MetaCorporal meta)
        {
            var userId = _userManager.GetUserId(User);

            if (User.IsInRole("Nutricionista"))
            {
                // Nutritionist can access all metas
                return true;
            }
            else if (User.IsInRole("Cliente"))
            {
                var client = await _context.Client.FirstOrDefaultAsync(c => c.IdentityUserId == userId);
                return client != null && meta.ClientId == client.ClientId;
            }
            return false;
        }

        private bool MetaCorporalExists(int id)
        {
            return _context.MetaCorporal.Any(e => e.MetaCorporalId == id);
        }
    }
}
