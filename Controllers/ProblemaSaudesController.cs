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
            string profissional, // Termo de pesquisa para Profissional
            string gravidade,
            int page = 1)
        {
            int pageSize = 10;

            // Inclui a tabela relacionada para poder filtrar
            var query = _context.ProblemaSaude
                .Include(p => p.ProfissionalExecutante)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(categoria))
                query = query.Where(p => p.ProblemaCategoria.ToLower().Contains(categoria.ToLower()));

            if (!string.IsNullOrWhiteSpace(nome))
                query = query.Where(p => p.ProblemaNome.ToLower().Contains(nome.ToLower()));

            if (!string.IsNullOrWhiteSpace(zona))
                query = query.Where(p => p.ZonaAtingida.ToLower().Contains(zona.ToLower()));

            // Filtro na tabela relacionada (Muitos-para-Muitos)
            if (!string.IsNullOrWhiteSpace(profissional))
            {
                query = query.Where(p => p.ProfissionalExecutante.Any(
                    prof => prof.Nome.ToLower().Contains(profissional.ToLower())
                ));
            }

            if (!string.IsNullOrWhiteSpace(gravidade))
                query = query.Where(p => p.Gravidade.ToString() == gravidade);

            int totalItems = await query.CountAsync();

            var pagination = new PaginationInfoExercicios<ProblemaSaude>(page, totalItems, pageSize);

            pagination.Items = await query
                .OrderBy(p => p.ProblemaNome)
                .Skip(pagination.ItemsToSkip)
                .Take(pagination.ItemsPerPage)
                .ToListAsync();

            ViewBag.Categoria = categoria;
            ViewBag.Nome = nome;
            ViewBag.Zona = zona;
            ViewBag.Profissional = profissional;
            ViewBag.Gravidade = gravidade;

            return View(pagination);
        }

        // GET: ProblemaSaudes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // Se o ID for nulo, mostra a página Invalido
            if (id == null)
            {
                return View("Invalido");
            }

            var problemaSaude = await _context.ProblemaSaude
                .Include(p => p.ProfissionalExecutante)
                .FirstOrDefaultAsync(m => m.ProblemaSaudeId == id);

            // Se não encontrar o registo, mostra a página Invalido
            if (problemaSaude == null)
            {
                return View("Invalido");
            }

            return View(problemaSaude);
        }

        // GET: ProblemaSaudes/Create
        public IActionResult Create()
        {
            // Carrega todos os profissionais para mostrar nas checkboxes
            ViewData["Profissionais"] = _context.ProfissionalExecutante.ToList();
            return View();
        }

        // POST: ProblemaSaudes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("ProblemaSaudeId,ProblemaCategoria,ProblemaNome,ZonaAtingida,Gravidade")] ProblemaSaude problemaSaude,
            int[] selectedProfissionais) // Recebe os IDs dos profissionais selecionados
        {
            if (ModelState.IsValid)
            {
                // Lógica para associar os profissionais selecionados
                if (selectedProfissionais != null && selectedProfissionais.Any())
                {
                    problemaSaude.ProfissionalExecutante = new List<ProfissionalExecutante>();
                    foreach (var profId in selectedProfissionais)
                    {
                        var profissional = await _context.ProfissionalExecutante.FindAsync(profId);
                        if (profissional != null)
                        {
                            problemaSaude.ProfissionalExecutante.Add(profissional);
                        }
                    }
                }

                _context.Add(problemaSaude);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Se o modelo falhar, recarrega a lista de profissionais
            ViewData["Profissionais"] = _context.ProfissionalExecutante.ToList();
            return View(problemaSaude);
        }

        // GET: ProblemaSaudes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // Se o ID for nulo, mostra a página Invalido
            if (id == null)
            {
                return View("Invalido");
            }

            var problemaSaude = await _context.ProblemaSaude
                .Include(p => p.ProfissionalExecutante)
                .FirstOrDefaultAsync(m => m.ProblemaSaudeId == id);

            // Se não encontrar o registo, mostra a página Invalido
            if (problemaSaude == null)
            {
                return View("Invalido");
            }

            // Carrega a lista completa de profissionais
            ViewData["Profissionais"] = _context.ProfissionalExecutante.ToList();

            // Carrega os IDs selecionados
            ViewData["SelectedProfissionais"] = problemaSaude.ProfissionalExecutante
                .Select(p => p.ProfissionalExecutanteId).ToList();

            return View(problemaSaude);
        }

        // POST: ProblemaSaudes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("ProblemaSaudeId,ProblemaCategoria,ProblemaNome,ZonaAtingida,Gravidade")] ProblemaSaude problemaSaude,
            int[] selectedProfissionais)
        {
            if (id != problemaSaude.ProblemaSaudeId)
            {
                return View("Invalido");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Lógica correta para atualizar Muitos-para-Muitos:
                    // 1. Carregar a entidade existente do BD
                    var problemaSaudeExistente = await _context.ProblemaSaude
                        .Include(p => p.ProfissionalExecutante)
                        .FirstOrDefaultAsync(p => p.ProblemaSaudeId == id);

                    if (problemaSaudeExistente == null)
                    {
                        return View("Invalido");
                    }

                    // 2. Atualizar propriedades simples
                    problemaSaudeExistente.ProblemaCategoria = problemaSaude.ProblemaCategoria;
                    problemaSaudeExistente.ProblemaNome = problemaSaude.ProblemaNome;
                    problemaSaudeExistente.ZonaAtingida = problemaSaude.ZonaAtingida;
                    problemaSaudeExistente.Gravidade = problemaSaude.Gravidade;

                    // 3. Atualizar a coleção de profissionais
                    problemaSaudeExistente.ProfissionalExecutante?.Clear();
                    if (selectedProfissionais != null && selectedProfissionais.Any())
                    {
                        problemaSaudeExistente.ProfissionalExecutante ??= new List<ProfissionalExecutante>();
                        foreach (var profId in selectedProfissionais)
                        {
                            var profissional = await _context.ProfissionalExecutante.FindAsync(profId);
                            if (profissional != null)
                            {
                                problemaSaudeExistente.ProfissionalExecutante.Add(profissional);
                            }
                        }
                    }

                    _context.Update(problemaSaudeExistente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProblemaSaudeExists(problemaSaude.ProblemaSaudeId))
                    {
                        return View("Invalido");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // Se o modelo falhar, recarrega os dados para a View
            ViewData["Profissionais"] = _context.ProfissionalExecutante.ToList();
            ViewData["SelectedProfissionais"] = selectedProfissionais?.ToList() ?? new List<int>();
            return View(problemaSaude);
        }

        // GET: ProblemaSaudes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            // Se o ID for nulo, mostra a página Invalido
            if (id == null)
            {
                return View("Invalido");
            }

            var problemaSaude = await _context.ProblemaSaude
                .Include(p => p.ProfissionalExecutante)
                .FirstOrDefaultAsync(m => m.ProblemaSaudeId == id);

            // Se não encontrar o registo, mostra a página Invalido
            if (problemaSaude == null)
            {
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
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProblemaSaudeExists(int id)
        {
            return _context.ProblemaSaude.Any(e => e.ProblemaSaudeId == id);
        }
    }
}