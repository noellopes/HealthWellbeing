using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using System.Threading.Tasks;
using System.Linq;

namespace HealthWellbeing.Controllers
{
    public class UtenteBalnearioController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UtenteBalnearioController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UtenteBalneario
        public async Task<IActionResult> Index()
        {
            var utentes = await _context.UtenteBalnearios.ToListAsync();
            return View(utentes);
        }

        // GET: UtenteBalneario/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var utente = await _context.UtenteBalnearios
                .FirstOrDefaultAsync(u => u.UtenteBalnearioId == id);

            if (utente == null)
                return NotFound();

            return View(utente);
        }

        // GET: UtenteBalneario/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UtenteBalneario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UtenteBalneario utenteBalneario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(utenteBalneario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(utenteBalneario);
        }

    }
}
