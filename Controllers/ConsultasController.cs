using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellbeing.Controllers
{
    public class ConsultasController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ConsultasController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Consultas
        public async Task<IActionResult> Index(int page = 1, string searchTerm = "")
        {

            var consultasQuery = _context.Consulta
                .Include(c => c.Doctor)
                .Include(c => c.Speciality)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Aceitar formatos comuns em PT
                var culture = new CultureInfo("pt-PT");
                var formats = new[] { "dd/MM/yyyy", "d/M/yyyy", "yyyy-MM-dd" };

                if (DateTime.TryParseExact(searchTerm.Trim(),
                                           formats,
                                           culture,
                                           DateTimeStyles.None,
                                           out var dataPesquisa))
                    
                {
                    // FILTRAR APENAS POR DATA (sem hora)
                    consultasQuery = consultasQuery.Where(c =>
                        c.DataMarcacao.Date == dataPesquisa.Date ||
                        c.DataConsulta.Date == dataPesquisa.Date
                    );
                }
                else
                {
                    // Se o utilizador escrever algo que não é data válida, podes:
                    // - ou não filtrar (deixar tudo)
                    // - ou devolver 0 resultados. Aqui vou devolver 0 resultados:
                    consultasQuery = consultasQuery.Where(c => false);
                }
            }

            // Aqui já não há nada "estranho" na query, CountAsync funciona
            int numberConsultas = await consultasQuery.CountAsync();

            var consultasInfo = new PaginationInfo<Consulta>(page, numberConsultas);

            var pageSize = consultasInfo.ItemsPerPage > 0 ? consultasInfo.ItemsPerPage : 10;

            consultasInfo.Items = await consultasQuery
                .OrderBy(c => c.DataConsulta)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.SearchTerm = searchTerm;

            return View(consultasInfo);
        }

        // GET: Consultas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consulta = await _context.Consulta
                .FirstOrDefaultAsync(m => m.IdConsulta == id);
            if (consulta == null)
            {
                return NotFound();
            }

            return View(consulta);
        }

        // GET: Consultas/Create
        public IActionResult Create()
        {
            ViewData["IdMedico"] = new SelectList(_context.Doctor, "IdMedico", "Nome");
            ViewData["IdEspecialidade"] = new SelectList(_context.Specialities, "IdEspecialidade", "Nome");

            return View();
        }

        // POST: Consultas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdConsulta,DataMarcacao,DataConsulta,DataCancelamento,HoraInicio,HoraFim")] Consulta consulta)
        {
            if (ModelState.IsValid)
            {
                _context.Add(consulta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdMedico"] = new SelectList(_context.Doctor, "IdMedico", "Nome", consulta.IdMedico);
            ViewData["IdEspecialidade"] = new SelectList(_context.Specialities, "IdEspecialidade", "Nome", consulta.IdEspecialidade);

            return View(consulta);
        }

        // GET: Consultas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consulta = await _context.Consulta.FindAsync(id);
            if (consulta == null)
            {
                return NotFound();
            }
            return View(consulta);
        }

        // POST: Consultas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdConsulta,DataMarcacao,DataConsulta,DataCancelamento,HoraInicio,HoraFim")] Consulta consulta)
        {
            if (id != consulta.IdConsulta)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(consulta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConsultaExists(consulta.IdConsulta))
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
            return View(consulta);
        }

        // GET: Consultas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consulta = await _context.Consulta
                .FirstOrDefaultAsync(m => m.IdConsulta == id);
            if (consulta == null)
            {
                return NotFound();
            }

            return View(consulta);
        }

        // POST: Consultas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var consulta = await _context.Consulta.FindAsync(id);
            if (consulta != null)
            {
                _context.Consulta.Remove(consulta);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ConsultaExists(int id)
        {
            return _context.Consulta.Any(e => e.IdConsulta == id);
        }
    }
}
