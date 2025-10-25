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
    public class RestricaoAlimentarController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public RestricaoAlimentarController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: RestricaoAlimentar
        public async Task<IActionResult> Index()
        {
            return View(await _context.RestricaoAlimentar.ToListAsync());
        }

        // GET: RestricaoAlimentar/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restricaoAlimentar = await _context.RestricaoAlimentar
                .FirstOrDefaultAsync(m => m.Id == id);
            if (restricaoAlimentar == null)
            {
                return NotFound();
            }

            return View(restricaoAlimentar);
        }

        // GET: RestricaoAlimentar/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RestricaoAlimentar/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Tipo,Gravidade,Sintomas")] RestricaoAlimentar restricaoAlimentar)
        {
            if (ModelState.IsValid)
            {
                _context.Add(restricaoAlimentar);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(restricaoAlimentar);
        }

        // GET: RestricaoAlimentar/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restricaoAlimentar = await _context.RestricaoAlimentar.FindAsync(id);
            if (restricaoAlimentar == null)
            {
                return NotFound();
            }
            return View(restricaoAlimentar);
        }

        // POST: RestricaoAlimentar/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Tipo,Gravidade,Sintomas")] RestricaoAlimentar restricaoAlimentar)
        {
            if (id != restricaoAlimentar.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(restricaoAlimentar);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RestricaoAlimentarExists(restricaoAlimentar.Id))
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
            return View(restricaoAlimentar);
        }

        // GET: RestricaoAlimentar/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restricaoAlimentar = await _context.RestricaoAlimentar
                .FirstOrDefaultAsync(m => m.Id == id);
            if (restricaoAlimentar == null)
            {
                return NotFound();
            }

            return View(restricaoAlimentar);
        }

        // POST: RestricaoAlimentar/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var restricaoAlimentar = await _context.RestricaoAlimentar.FindAsync(id);
            if (restricaoAlimentar != null)
            {
                _context.RestricaoAlimentar.Remove(restricaoAlimentar);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RestricaoAlimentarExists(int id)
        {
            return _context.RestricaoAlimentar.Any(e => e.Id == id);
        }
    }
}

