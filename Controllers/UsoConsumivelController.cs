using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class UsoConsumivelController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public UsoConsumivelController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: UsoConsumivel
        public async Task<IActionResult> Index()
        {
            var healthWellbeingDbContext = _context.UsoConsumivel.Include(u => u.Consumivel).Include(u => u.TreatmentRecord);
            return View(await healthWellbeingDbContext.ToListAsync());
        }

        // GET: UsoConsumivel/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usoConsumivel = await _context.UsoConsumivel
                .Include(u => u.Consumivel)
                .Include(u => u.TreatmentRecord)
                .FirstOrDefaultAsync(m => m.UsoConsumivelId == id);
            if (usoConsumivel == null)
            {
                return NotFound();
            }

            return View(usoConsumivel);
        }

        // GET: UsoConsumivel/Create
        public IActionResult Create(int treatmentRecordId)
        {
            ViewData["ConsumivelID"] = new SelectList(_context.Consumivel, "ConsumivelId", "Nome");

            // envia o id para a view
            ViewBag.TreatmentRecordId = treatmentRecordId;

            return View();
        }

        // POST: UsoConsumivel/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UsoConsumivelId,TreatmentRecordId,ConsumivelID,QuantidadeUsada,DataConsumo")] UsoConsumivel usoConsumivel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usoConsumivel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ConsumivelID"] = new SelectList(_context.Consumivel, "ConsumivelId", "Nome", usoConsumivel.ConsumivelID);
            ViewData["TreatmentRecordId"] = new SelectList(_context.TreatmentRecord, "Id", "Id", usoConsumivel.TreatmentRecordId);
            return View(usoConsumivel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMultiple(int treatmentRecordId, int[] ConsumivelID, int[] QuantidadeUsada, DateTime[] DataConsumo)
        {
            for (int i = 0; i < ConsumivelID.Length; i++)
            {
                var uso = new UsoConsumivel
                {
                    TreatmentRecordId = treatmentRecordId,
                    ConsumivelID = ConsumivelID[i],
                    QuantidadeUsada = QuantidadeUsada[i],
                    DataConsumo = DataConsumo[i]
                };

                _context.UsoConsumivel.Add(uso);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "TreatmentRecords", new { id = treatmentRecordId });
        }
        // GET: UsoConsumivel/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usoConsumivel = await _context.UsoConsumivel.FindAsync(id);
            if (usoConsumivel == null)
            {
                return NotFound();
            }
            ViewData["ConsumivelID"] = new SelectList(_context.Consumivel, "ConsumivelId", "Nome", usoConsumivel.ConsumivelID);
            ViewData["TreatmentRecordId"] = new SelectList(_context.TreatmentRecord, "Id", "Id", usoConsumivel.TreatmentRecordId);
            return View(usoConsumivel);
        }

        // POST: UsoConsumivel/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UsoConsumivelId,TreatmentRecordId,ConsumivelID,QuantidadeUsada,DataConsumo")] UsoConsumivel usoConsumivel)
        {
            if (id != usoConsumivel.UsoConsumivelId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usoConsumivel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsoConsumivelExists(usoConsumivel.UsoConsumivelId))
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
            ViewData["ConsumivelID"] = new SelectList(_context.Consumivel, "ConsumivelId", "Nome", usoConsumivel.ConsumivelID);
            ViewData["TreatmentRecordId"] = new SelectList(_context.TreatmentRecord, "Id", "Id", usoConsumivel.TreatmentRecordId);
            return View(usoConsumivel);
        }

        // GET: UsoConsumivel/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usoConsumivel = await _context.UsoConsumivel
                .Include(u => u.Consumivel)
                .Include(u => u.TreatmentRecord)
                .FirstOrDefaultAsync(m => m.UsoConsumivelId == id);
            if (usoConsumivel == null)
            {
                return NotFound();
            }

            return View(usoConsumivel);
        }

        // POST: UsoConsumivel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usoConsumivel = await _context.UsoConsumivel.FindAsync(id);
            if (usoConsumivel != null)
            {
                _context.UsoConsumivel.Remove(usoConsumivel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsoConsumivelExists(int id)
        {
            return _context.UsoConsumivel.Any(e => e.UsoConsumivelId == id);
        }
    }
}
