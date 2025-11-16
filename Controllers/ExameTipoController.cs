using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;

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
            int page = 1,
            string searchNome = "",
            string searchEspecialidade = "")
        {
            // 1. Configuração da Pesquisa (para View e Links)
            ViewBag.SearchNome = searchNome;
            ViewBag.SearchEspecialidade = searchEspecialidade;

            // 2. Aplicação dos Filtros na Query
            var examesQuery = _context.ExameTipo.AsQueryable();

            if (!string.IsNullOrEmpty(searchNome))
            {
                examesQuery = examesQuery.Where(et => et.Nome.Contains(searchNome));
            }
            if (!string.IsNullOrEmpty(searchEspecialidade))
            {
                examesQuery = examesQuery.Where(et => et.Especialidade.Contains(searchEspecialidade));
            }

            // 3. Contagem e Criação do ViewModel de Paginação
            int totalExames = await examesQuery.CountAsync();

            // Instanciar o ViewModel (ItemsPerPage=5 para forçar 2 páginas de teste)
            var examesInfo = new PaginationInfo<ExameTipo>(page, totalExames, itemsPerPage: 5);

            // 4. Ordenação e Aplicação da Paginação (Skip/Take)
            examesInfo.Items = await examesQuery
                .OrderBy(et => et.Nome)
                .Skip(examesInfo.ItemsToSkip) // Pula os itens das páginas anteriores
                .Take(examesInfo.ItemsPerPage) // Pega apenas os 5 itens da página atual
                .ToListAsync();

            // Retorna o ViewModel de Paginação
            return View(examesInfo);
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
