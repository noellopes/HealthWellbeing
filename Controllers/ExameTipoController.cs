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
    public class ExameTipoController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ExameTipoController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: ExameTipoes
        public async Task<IActionResult> Index(
            int page = 1, // Parâmetro de paginação
            string searchNome = "", // Parâmetro de pesquisa por Nome
            string searchEspecialidade = "") // Parâmetro de pesquisa por Especialidade
        {
            // 1. Armazenar termos de pesquisa na ViewBag (necessário para manter o formulário preenchido)
            ViewBag.SearchNome = searchNome;
            ViewBag.SearchEspecialidade = searchEspecialidade;

            // 2. Criar a consulta AsQueryable para filtros
            var examesQuery = _context.ExameTipo.AsQueryable();

            // 3. Aplicação dos filtros (Lógica de Pesquisa do ficheiro 13_search.pdf)
            if (!string.IsNullOrEmpty(searchNome))
            {
                // Filtra a consulta onde o Nome contém a string de pesquisa
                examesQuery = examesQuery.Where(et => et.Nome.Contains(searchNome));
            }

            if (!string.IsNullOrEmpty(searchEspecialidade))
            {
                // Filtra a consulta onde a Especialidade contém a string de pesquisa
                examesQuery = examesQuery.Where(et => et.Especialidade.Contains(searchEspecialidade));
            }

            // 4. Paginação (Apenas Ordenação por enquanto, para usar o IEnumerable<T>)
            // Se usasse o PaginationInfo<T>, a lógica seria:
            // int totalExames = await examesQuery.CountAsync(); [cite: 553]
            // var examesInfo = new PaginationInfo<ExameTipo>(page, totalExames); [cite: 554]

            var examesPaginados = await examesQuery
                .OrderBy(et => et.Nome) // Ordenar por Nome
                // .Skip() e .Take() seriam inseridos aqui para paginação [cite: 557]
                .ToListAsync();

            // Retorna a lista filtrada/ordenada
            return View(examesPaginados);
        }

        // GET: ExameTipoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exameTipo = await _context.ExameTipo
                .FirstOrDefaultAsync(m => m.ExameTipoId == id);
            if (exameTipo == null)
            {
                return NotFound();
            }

            return View(exameTipo);
        }

        // GET: ExameTipoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ExameTipoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExameTipoId,Nome,Descricao,Especialidade")] ExameTipo exameTipo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(exameTipo);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"O tipo de exame '{exameTipo.Nome}' foi criado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            return View(exameTipo);
        }

        // GET: ExameTipoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exameTipo = await _context.ExameTipo.FindAsync(id);
            if (exameTipo == null)
            {
                return NotFound();
            }
            return View(exameTipo);
        }

        // POST: ExameTipoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExameTipoId,Nome,Descricao,Especialidade")] ExameTipo exameTipo)
        {
            if (id != exameTipo.ExameTipoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(exameTipo);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"O tipo de exame '{exameTipo.Nome}' foi atualizado com sucesso!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExameTipoExists(exameTipo.ExameTipoId))
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
            return View(exameTipo);
        }

        // GET: ExameTipoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exameTipo = await _context.ExameTipo
                .FirstOrDefaultAsync(m => m.ExameTipoId == id);
            if (exameTipo == null)
            {
                return NotFound();
            }

            return View(exameTipo);
        }

        // POST: ExameTipoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exameTipo = await _context.ExameTipo.FindAsync(id);
            if (exameTipo != null)
            {
                _context.ExameTipo.Remove(exameTipo);
                TempData["SuccessMessage"] = $"O tipo de exame '{exameTipo.Nome}' foi apagado com sucesso!";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExameTipoExists(int id)
        {
            return _context.ExameTipo.Any(e => e.ExameTipoId == id);
        }
    }
}
