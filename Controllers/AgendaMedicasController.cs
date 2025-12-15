using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellbeing.Controllers
{
    public class AgendaMedicasController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public AgendaMedicasController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: AgendaMedicas
        public async Task<IActionResult> Index()
        {
            var agendaMedica = await _context.AgendaMedica
        .Include(a => a.Medico)
        .ThenInclude(m => m.Especialidade)   // Doctor.Especialidade (Specialities)
        .ToListAsync();

            // construir o modelo para a view
            var model = agendaMedica
                .Where(a => a.Medico != null && a.Medico.Especialidade != null)
                .GroupBy(a => a.Medico.Especialidade.Nome)
                .Select(g => new EspecialidadeAgendaVM
                {
                    Especialidade = g.Key,
                    Medicos = g
                        .GroupBy(a => a.Medico!)   // agrupar por médico
                        .Select(mg => new MedicoAgendaVM
                        {
                            Medico = mg.Key.Nome,
                                
                            Segunda = string.Join(", ",
                                mg.Where(x => x.DiaSemana == DayOfWeek.Monday)
                                  .Select(x => $"{x.HoraInicio:HH\\:mm}-{x.HoraFim:HH\\:mm}")),

                            Terca = string.Join(", ",
                                mg.Where(x => x.DiaSemana == DayOfWeek.Tuesday)
                                  .Select(x => $"{x.HoraInicio:HH\\:mm}-{x.HoraFim:HH\\:mm}")),

                            Quarta = string.Join(", ",
                                mg.Where(x => x.DiaSemana == DayOfWeek.Wednesday)
                                  .Select(x => $"{x.HoraInicio:HH\\:mm}-{x.HoraFim:HH\\:mm}")),

                            Quinta = string.Join(", ",
                                mg.Where(x => x.DiaSemana == DayOfWeek.Thursday)
                                  .Select(x => $"{x.HoraInicio:HH\\:mm}-{x.HoraFim:HH\\:mm}")),

                            Sexta = string.Join(", ",
                                mg.Where(x => x.DiaSemana == DayOfWeek.Friday)
                                  .Select(x => $"{x.HoraInicio:HH\\:mm}-{x.HoraFim:HH\\:mm}")),
                        })
                        .OrderBy(m => m.Medico)
                        .ToList()
                })
                .OrderBy(e => e.Especialidade)
                .ToList();

            return View(model);
        }

        // GET: AgendaMedicas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var agendaMedica = await _context.AgendaMedica
                .Include(a => a.Medico)
                .FirstOrDefaultAsync(m => m.IdAgendaMedica == id);
            if (agendaMedica == null)
            {
                return NotFound();
            }

            return View(agendaMedica);
        }

        // GET: AgendaMedicas/Create
        public IActionResult Create()
        {
            ViewData["IdMedico"] = new SelectList(_context.Doctor, "IdMedico", "Email");
            return View();
        }

        // POST: AgendaMedicas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdAgendaMedica,IdMedico,DiaSemana,HoraInicio,HoraFim")] AgendaMedica agendaMedica)
        {
            if (ModelState.IsValid)
            {
                _context.Add(agendaMedica);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdMedico"] = new SelectList(_context.Doctor, "IdMedico", "Email", agendaMedica.IdMedico);
            return View(agendaMedica);
        }

        // GET: AgendaMedicas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var agendaMedica = await _context.AgendaMedica.FindAsync(id);
            if (agendaMedica == null)
            {
                return NotFound();
            }
            ViewData["IdMedico"] = new SelectList(_context.Doctor, "IdMedico", "Email", agendaMedica.IdMedico);
            return View(agendaMedica);
        }

        // POST: AgendaMedicas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdAgendaMedica,IdMedico,DiaSemana,HoraInicio,HoraFim")] AgendaMedica agendaMedica)
        {
            if (id != agendaMedica.IdAgendaMedica)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(agendaMedica);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AgendaMedicaExists(agendaMedica.IdAgendaMedica))
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
            ViewData["IdMedico"] = new SelectList(_context.Doctor, "IdMedico", "Email", agendaMedica.IdMedico);
            return View(agendaMedica);
        }

        // GET: AgendaMedicas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var agendaMedica = await _context.AgendaMedica
                .Include(a => a.Medico)
                .FirstOrDefaultAsync(m => m.IdAgendaMedica == id);
            if (agendaMedica == null)
            {
                return NotFound();
            }

            return View(agendaMedica);
        }

        // POST: AgendaMedicas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {   
            var agendaMedica = await _context.AgendaMedica.FindAsync(id);
            if (agendaMedica != null)
            {
                _context.AgendaMedica.Remove(agendaMedica);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AgendaMedicaExists(int id)
        {
            return _context.AgendaMedica.Any(e => e.IdAgendaMedica == id);
        }

        
    }
}
