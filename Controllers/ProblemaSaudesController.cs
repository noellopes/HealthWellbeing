using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModel;

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
            string profissional,
            string gravidade,
            int page = 1)
        {
            int pageSize = 10;

            // Nota: Se a tabela no DbContext se chamar 'ProblemaSaudes' (plural), mude abaixo
            var query = _context.ProblemaSaude
                .Include(p => p.ProfissionalExecutante)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(categoria))
                query = query.Where(p => p.ProblemaCategoria.ToLower().Contains(categoria.ToLower()));

            if (!string.IsNullOrWhiteSpace(nome))
                query = query.Where(p => p.ProblemaNome.ToLower().Contains(nome.ToLower()));

            if (!string.IsNullOrWhiteSpace(zona))
                query = query.Where(p => p.ZonaAtingida.ToLower().Contains(zona.ToLower()));

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

        // GET: ProblemaSaudes/Create
        public IActionResult Create()
        {
            // Corrigido: Acedendo à tabela correta de Profissionais
            ViewData["Profissionais"] = _context.ProfissionaisExecutantes.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProblemaSaudeId,ProblemaCategoria,ProblemaNome,ZonaAtingida,Gravidade")] ProblemaSaude problemaSaude, int[] selectedProfissionais)
        {
            if (ModelState.IsValid)
            {
                if (selectedProfissionais != null)
                {
                    problemaSaude.ProfissionalExecutante = new List<ProfissionalExecutante>();
                    foreach (var profId in selectedProfissionais)
                    {
                        var prof = await _context.ProfissionaisExecutantes.FindAsync(profId);
                        if (prof != null) problemaSaude.ProfissionalExecutante.Add(prof);
                    }
                }

                _context.Add(problemaSaude);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Profissionais"] = _context.ProfissionaisExecutantes.ToList();
            return View(problemaSaude);
        }

        // GET: ProblemaSaudes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var problemaSaude = await _context.ProblemaSaude
                .Include(p => p.ProfissionalExecutante)
                .FirstOrDefaultAsync(m => m.ProblemaSaudeId == id);

            if (problemaSaude == null) return NotFound();

            // CORREÇÃO AQUI: Usando o nome correto da tabela e da propriedade ID
            ViewData["Profissionais"] = _context.ProfissionaisExecutantes.ToList();

            // Supondo que no seu Model 'ProfissionalExecutante' a chave é 'Id' ou 'ProfissionalExecutanteId'
            // Ajuste o Select abaixo conforme o seu Model real
            ViewData["SelectedProfissionais"] = problemaSaude.ProfissionalExecutante
                .Select(p => p.Id).ToList();

            return View(problemaSaude);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProblemaSaudeId,ProblemaCategoria,ProblemaNome,ZonaAtingida,Gravidade")] ProblemaSaude problemaSaude, int[] selectedProfissionais)
        {
            if (id != problemaSaude.ProblemaSaudeId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var entity = await _context.ProblemaSaude
                        .Include(p => p.ProfissionalExecutante)
                        .FirstOrDefaultAsync(p => p.ProblemaSaudeId == id);

                    if (entity == null) return NotFound();

                    entity.ProblemaCategoria = problemaSaude.ProblemaCategoria;
                    entity.ProblemaNome = problemaSaude.ProblemaNome;
                    entity.ZonaAtingida = problemaSaude.ZonaAtingida;
                    entity.Gravidade = problemaSaude.Gravidade;

                    // Atualizar Relacionamento Muitos-para-Muitos
                    entity.ProfissionalExecutante.Clear();
                    if (selectedProfissionais != null)
                    {
                        foreach (var profId in selectedProfissionais)
                        {
                            var prof = await _context.ProfissionaisExecutantes.FindAsync(profId);
                            if (prof != null) entity.ProfissionalExecutante.Add(prof);
                        }
                    }

                    _context.Update(entity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProblemaSaudeExists(problemaSaude.ProblemaSaudeId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(problemaSaude);
        }

        // Restantes métodos (Details, Delete) seguem a mesma lógica de incluir o contexto correto...

        private bool ProblemaSaudeExists(int id)
        {
            return _context.ProblemaSaude.Any(e => e.ProblemaSaudeId == id);
        }
    }
}