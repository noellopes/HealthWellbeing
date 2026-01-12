using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class TerapeutaController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        // Construtor que injeta a base de dados no controlador
        public TerapeutaController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Terapeuta
        [HttpGet]
        public async Task<IActionResult> Index(
            string? nomeFiltro,
            string? especialidadeFiltro,
            bool? ativoFiltro,
            string? successMessage = null,
            int page = 1,
            int pageSize = 10)
        {
            // Passa mensagem de sucesso (vinda de Create/Edit/Delete)
            ViewBag.SuccessMessage = successMessage;

            // Passar filtros à View
            ViewBag.NomeFiltro = nomeFiltro;
            ViewBag.EspecialidadeFiltro = especialidadeFiltro;
            ViewBag.AtivoFiltro = ativoFiltro;

            // Query base
            var query = _context.Terapeuta.AsQueryable();

            // Aplicar filtro: nome
            if (!string.IsNullOrWhiteSpace(nomeFiltro))
            {
                query = query.Where(t => t.Nome.Contains(nomeFiltro));
            }

            // Aplicar filtro: especialidade
            if (!string.IsNullOrWhiteSpace(especialidadeFiltro))
            {
                query = query.Where(t => t.Especialidade.Contains(especialidadeFiltro));
            }

            // Aplicar filtro: ativo/inativo
            if (ativoFiltro.HasValue)
            {
                query = query.Where(t => t.Ativo == ativoFiltro.Value);
            }

            // Contagem após filtros
            int totalRegistos = await query.CountAsync();

            // Número total de páginas
            int totalPaginas = totalRegistos == 0 ? 1 : (int)Math.Ceiling((double)totalRegistos / pageSize);

            // Garantir que a página é válida
            page = Math.Max(1, Math.Min(page, totalPaginas));

            // Aplicar paginação
            var terapeutas = await query
                .OrderBy(t => t.Nome)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Passar info à View
            ViewBag.Page = page;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.TotalRegistos = totalRegistos;
            ViewBag.PageSize = pageSize;

            return View(terapeutas);
        }

        // GET: Terapeuta/Details/5
        public async Task<IActionResult> Details(int? id, string? successMessage = null)
        {
            if (id == null)
            {
                return NotFound();
            }

            var terapeuta = await _context.Terapeuta
                .FirstOrDefaultAsync(m => m.TerapeutaId == id);

            if (terapeuta == null)
            {
                return NotFound();
            }

            ViewBag.SuccessMessage = successMessage;
            return View(terapeuta);
        }

        // GET: Terapeuta/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Terapeuta/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TerapeutaId,Nome,Especialidade,Telefone,Email,AnoEntrada,Ativo")] Terapeuta terapeuta)
        {
            if (terapeuta.AnoEntrada > DateTime.Now.Year)
            {
                ModelState.AddModelError("AnoEntrada", "O ano de entrada não pode ser superior ao ano atual.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(terapeuta);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Details), new
                {
                    id = terapeuta.TerapeutaId,
                    successMessage = "Terapeuta criado com sucesso!"
                });
            }

            return View(terapeuta);
        }

        // GET: Terapeuta/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var terapeuta = await _context.Terapeuta.FindAsync(id);
            if (terapeuta == null)
            {
                return NotFound();
            }

            return View(terapeuta);
        }

        // POST: Terapeuta/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TerapeutaId,Nome,Especialidade,Telefone,Email,AnoEntrada,Ativo")] Terapeuta terapeuta)
        {
            if (id != terapeuta.TerapeutaId)
            {
                return NotFound();
            }

            if (terapeuta.AnoEntrada > DateTime.Now.Year)
            {
                ModelState.AddModelError("AnoEntrada", "O ano de entrada não pode ser superior ao ano atual.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(terapeuta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TerapeutaExists(terapeuta.TerapeutaId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Details), new
                {
                    id = terapeuta.TerapeutaId,
                    successMessage = "Dados do terapeuta atualizados com sucesso!"
                });
            }

            return View(terapeuta);
        }

        // GET: Terapeuta/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var terapeuta = await _context.Terapeuta
                .FirstOrDefaultAsync(m => m.TerapeutaId == id);

            if (terapeuta == null)
            {
                return NotFound();
            }

            return View(terapeuta);
        }

        // POST: Terapeuta/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var terapeuta = await _context.Terapeuta.FindAsync(id);

            if (terapeuta != null)
            {
                _context.Terapeuta.Remove(terapeuta);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index), new
            {
                successMessage = "Terapeuta eliminado com sucesso!"
            });
        }

        // Verifica se existe terapeuta
        private bool TerapeutaExists(int id)
        {
            return _context.Terapeuta.Any(e => e.TerapeutaId == id);
        }
    }
}
