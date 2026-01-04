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
using Microsoft.AspNetCore.Authorization;

namespace HealthWellbeing.Controllers
{
    [Authorize]
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
            var hoje = DateTime.Today;

            var consultasQuery = _context.Consulta
                .Include(c => c.Doctor)
                .Include(c => c.Speciality)
                .AsQueryable();

            // ✅ Se NÃO for Diretor Clínico -> só agendadas a partir de hoje e não canceladas
            if (!User.IsInRole("DiretorClinico"))
            {
                consultasQuery = consultasQuery.Where(c =>
                    !c.DataCancelamento.HasValue &&
                    c.DataConsulta.Date >= hoje
                );
            }
            // ✅ Se for Diretor Clínico -> vê tudo (passadas, futuras, canceladas, etc.)
            // (logo não aplicamos nenhum filtro aqui)

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var culture = new CultureInfo("pt-PT");
                var formats = new[] { "dd/MM/yyyy", "d/M/yyyy", "yyyy-MM-dd" };

                if (DateTime.TryParseExact(searchTerm.Trim(), formats, culture, DateTimeStyles.None, out var dataPesquisa))
                {
                    consultasQuery = consultasQuery.Where(c =>
                        c.DataMarcacao.Date == dataPesquisa.Date ||
                        c.DataConsulta.Date == dataPesquisa.Date
                    );
                }
                else
                {
                    consultasQuery = consultasQuery.Where(c => false);
                }
            }

            int numberConsultas = await consultasQuery.CountAsync();
            var consultasInfo = new PaginationInfo<Consulta>(page, numberConsultas);
            var pageSize = consultasInfo.ItemsPerPage > 0 ? consultasInfo.ItemsPerPage : 10;

            consultasInfo.Items = await consultasQuery
                .OrderByDescending(c => c.DataConsulta) // histórico normalmente faz mais sentido desc
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
            
            ViewData["IdMedico"] = new SelectList(_context.Set<Doctor>(), "IdMedico", "Nome");
            ViewData["IdEspecialidade"] = new SelectList(_context.Set<Specialities>(), "IdEspecialidade", "Nome");

            return View();
        }

        // POST: Consultas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdConsulta,DataMarcacao,DataConsulta,DataCancelamento,HoraInicio,HoraFim,IdMedico,IdEspecialidade")] Consulta consulta)
        {
            var dataHoraConsulta = consulta.DataConsulta.Date + consulta.HoraInicio.ToTimeSpan();

            
            if (dataHoraConsulta < DateTime.Now)
            {
                ModelState.AddModelError("DataConsulta",
                    "A data e hora da consulta não podem ser anteriores à data e hora atual.");
            }

            
            var dataHoraFim = consulta.DataConsulta.Date + consulta.HoraFim.ToTimeSpan();
            if (dataHoraFim <= dataHoraConsulta)
            {
                ModelState.AddModelError("HoraFim",
                    "A hora de fim tem de ser posterior à hora de início.");
            }

            if (ModelState.IsValid)
            {
                consulta.DataMarcacao = DateTime.Now;

                _context.Add(consulta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index),
                new
                {
                    id = consulta.IdConsulta,
                    SuccessMessage = "Consulta criada com sucesso."
                }
                );
            }

            ViewData["IdMedico"] = new SelectList(_context.Set<Doctor>(), "IdMedico", "Nome", consulta.IdMedico);
            ViewData["IdEspecialidade"] = new SelectList(_context.Set<Specialities>(), "IdEspecialidade", "Nome", consulta.IdEspecialidade);

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
