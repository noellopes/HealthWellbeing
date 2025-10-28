using Microsoft.AspNetCore.Mvc;
using HealthWellbeing.Data;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class ZonasArmazenamentoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ZonasArmazenamentoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ZonasArmazenamento
        public IActionResult Index()
        {
            var zonas = _context.ZonasArmazenamento.ToList();
            return View(zonas);
        }

        // GET: ZonasArmazenamento/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ZonasArmazenamento/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ZonaArmazenamento zona)
        {
            if (ModelState.IsValid)
            {
                _context.ZonasArmazenamento.Add(zona);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(zona);
        }

        // GET: ZonasArmazenamento/Edit/5
        public IActionResult Edit(int id)
        {
            var zona = _context.ZonasArmazenamento.Find(id);
            if (zona == null)
            {
                return NotFound();
            }
            return View(zona);
        }

        // POST: ZonasArmazenamento/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ZonaArmazenamento zona)
        {
            if (id != zona.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.ZonasArmazenamento.Update(zona);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(zona);
        }

        // GET: ZonasArmazenamento/Delete/5
        public IActionResult Delete(int id)
        {
            var zona = _context.ZonasArmazenamento.Find(id);
            if (zona == null)
            {
                return NotFound();
            }
            return View(zona);
        }

        // POST: ZonasArmazenamento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var zona = _context.ZonasArmazenamento.Find(id);
            if (zona != null)
            {
                _context.ZonasArmazenamento.Remove(zona);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: ZonasArmazenamento/Details/5
        public IActionResult Details(int id)
        {
            var zona = _context.ZonasArmazenamento.Find(id);
            if (zona == null)
            {
                return NotFound();
            }
            return View(zona);
        }
    }
}
