using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;

namespace HealthWellbeing.Controllers
{
    public class ProblemaSaudesController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ProblemaSaudesController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: ProblemaSaudes
        public async Task<IActionResult> Index(
            string categoria,
            string nome,
            string zona,
            string gravidade,
            int page = 1,
            string SuccessMessage = "")
        {
            int pageSize = 10;
            var query = _context.ProblemaSaude.AsQueryable();

            // --- Filtros ---
            if (!string.IsNullOrWhiteSpace(categoria))
                query = query.Where(p => p.ProblemaCategoria.ToLower().Contains(categoria.ToLower()));

            if (!string.IsNullOrWhiteSpace(nome))
                query = query.Where(p => p.ProblemaNome.ToLower().Contains(nome.ToLower()));

            if (!string.IsNullOrWhiteSpace(zona))
                query = query.Where(p => p.ZonaAtingida.ToLower().Contains(zona.ToLower()));

            if (!string.IsNullOrWhiteSpace(gravidade))
                query = query.Where(p => p.Gravidade.ToString() == gravidade);

            // Guardar filtros
            ViewBag.Categoria = categoria;
            ViewBag.Nome = nome;
            ViewBag.Zona = zona;
            ViewBag.Gravidade = gravidade;

            ViewBag.SuccessMessage = SuccessMessage;

            // --- Paginação ---
            int totalItems = await query.CountAsync();
            var pagination = new PaginationInfo<ProblemaSaude>(page, totalItems, pageSize);

            if (totalItems > 0)
            {
                pagination.Items = await query
                    .OrderBy(p => p.ProblemaNome)
                    .Skip(pagination.ItemsToSkip)
                    .Take(pagination.ItemsPerPage)
                    .ToListAsync();
            }
            else
            {
                pagination.Items = new List<ProblemaSaude>();
            }

            return View(pagination);
        }

        // GET: ProblemaSaudes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return View("Invalido"); // ID nulo mostra página de erro

            var problemaSaude = await _context.ProblemaSaude
                .FirstOrDefaultAsync(m => m.ProblemaSaudeId == id);

            if (problemaSaude == null)
            {
                // CORREÇÃO: Se não existe, mostra a view Invalido em vez de erro 404
                return View("Invalido");
            }

            return View(problemaSaude);
        }

        // GET: ProblemaSaudes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ProblemaSaudes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("ProblemaSaudeId,ProblemaCategoria,ProblemaNome,ZonaAtingida,Gravidade")] ProblemaSaude problemaSaude)
        {
            // Validação de Duplicados
            bool nomeExiste = await _context.ProblemaSaude
                .AnyAsync(p => p.ProblemaNome.ToLower() == problemaSaude.ProblemaNome.ToLower());

            if (nomeExiste)
            {
                ModelState.AddModelError("ProblemaNome", "Já existe um problema de saúde com este nome.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(problemaSaude);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { SuccessMessage = "Criado com sucesso!" });
            }
            return View(problemaSaude);
        }

        // GET: ProblemaSaudes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return View("Invalido");

            var problemaSaude = await _context.ProblemaSaude.FindAsync(id);

            if (problemaSaude == null)
            {
                // CORREÇÃO: Se tentas editar e não existe (ex: ID 9), mostra a página Invalido
                return View("Invalido");
            }

            return View(problemaSaude);
        }

        // POST: ProblemaSaudes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("ProblemaSaudeId,ProblemaCategoria,ProblemaNome,ZonaAtingida,Gravidade")] ProblemaSaude problemaSaude)
        {
            if (id != problemaSaude.ProblemaSaudeId) return View("Invalido");

            // Validação de Duplicados
            bool nomeExiste = await _context.ProblemaSaude
                .AnyAsync(p => p.ProblemaNome.ToLower() == problemaSaude.ProblemaNome.ToLower()
                               && p.ProblemaSaudeId != id);

            if (nomeExiste)
            {
                ModelState.AddModelError("ProblemaNome", "Já existe outro problema de saúde com este nome.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var problemaSaudeExistente = await _context.ProblemaSaude
                        .FirstOrDefaultAsync(p => p.ProblemaSaudeId == id);

                    if (problemaSaudeExistente == null)
                    {
                        // CORREÇÃO (Concorrência): Se foi apagado enquanto editavas, 
                        // manda os dados para a view Invalido para o utilizador poder recuperar/recriar.
                        return View("Invalido", problemaSaude);
                    }

                    problemaSaudeExistente.ProblemaCategoria = problemaSaude.ProblemaCategoria;
                    problemaSaudeExistente.ProblemaNome = problemaSaude.ProblemaNome;
                    problemaSaudeExistente.ZonaAtingida = problemaSaude.ZonaAtingida;
                    problemaSaudeExistente.Gravidade = problemaSaude.Gravidade;

                    _context.Update(problemaSaudeExistente);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index), new { SuccessMessage = "Editado com sucesso!" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProblemaSaudeExists(problemaSaude.ProblemaSaudeId))
                    {
                        // Se o erro foi de concorrência e o registo desapareceu
                        return View("Invalido", problemaSaude);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(problemaSaude);
        }

        // GET: ProblemaSaudes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return View("Invalido");

            var problemaSaude = await _context.ProblemaSaude
                .FirstOrDefaultAsync(m => m.ProblemaSaudeId == id);

            if (problemaSaude == null)
            {
                // No Delete, também podes mostrar a página Invalido
                return View("Invalido");
            }

            return View(problemaSaude);
        }

        // POST: ProblemaSaudes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var problemaSaude = await _context.ProblemaSaude.FindAsync(id);
            if (problemaSaude != null)
            {
                _context.ProblemaSaude.Remove(problemaSaude);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index), new { SuccessMessage = "Eliminado com sucesso!" });
        }

        private bool ProblemaSaudeExists(int id)
        {
            return _context.ProblemaSaude.Any(e => e.ProblemaSaudeId == id);
        }
    }
}