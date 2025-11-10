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
        private readonly ApplicationDbContext _context;

        // Construtor que injeta a base de dados no controlador
        public TerapeutaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Terapeuta

        [HttpGet]
        public async Task<IActionResult> Index(string? successMessage = null, int page = 1, int pageSize = 10)
        {
            // Passa mensagem de sucesso (vinda de Create/Edit/Delete)
            ViewBag.SuccessMessage = successMessage;

            // Contagem do numero total de terapeutas existentes
            int totalRegistos = await _context.Terapeutas.CountAsync();

            // Calculo do número total de páginas
            int totalPaginas = totalRegistos == 0 ? 1 : (int)Math.Ceiling((double)totalRegistos / pageSize);

            // Garantia que o número da página é válido
            page = Math.Max(1, Math.Min(page, totalPaginas == 0 ? 1 : totalPaginas));

            // Vai selecionar apenas os terapeutas da página atual
            var terapeutas = await _context.Terapeutas
                .OrderBy(t => t.Nome)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Passagem de variáveis para a View
            ViewBag.Page = page;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.TotalRegistos = totalRegistos;
            ViewBag.PageSize = pageSize;

            // lista de terapeutas
            return View(terapeutas);
        }


        // GET: Terapeuta/Details/5

        public async Task<IActionResult> Details(int? id, string? successMessage = null)
        {
            if (id == null)
            {
                return NotFound();
            }

            var terapeutaModel = await _context.Terapeutas
                .FirstOrDefaultAsync(m => m.TerapeutaId == id);

            if (terapeutaModel == null)
            {
                return NotFound();
            }

            // Mensagem de sucesso
            ViewBag.SuccessMessage = successMessage;
            return View(terapeutaModel);
        }


        // GET: Terapeuta/Create

        public IActionResult Create()
        {
            return View();
        }

        // POST: Terapeuta/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TerapeutaId,Nome,Especialidade,Telefone,Email,AnosExperiencia,Ativo")] TerapeutaModel terapeutaModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(terapeutaModel);
                await _context.SaveChangesAsync();

                // Redireciona para a página de detalhes
                return RedirectToAction(nameof(Details), new
                {
                    id = terapeutaModel.TerapeutaId,
                    successMessage = "Terapeuta criado com sucesso!"
                });
            }

            return View(terapeutaModel);
        }

        // GET: Terapeuta/Edit/5

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var terapeutaModel = await _context.Terapeutas.FindAsync(id);
            if (terapeutaModel == null)
            {
                return NotFound();
            }

            return View(terapeutaModel);
        }

        // POST: Terapeuta/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TerapeutaId,Nome,Especialidade,Telefone,Email,AnosExperiencia,Ativo")] TerapeutaModel terapeutaModel)
        {
            if (id != terapeutaModel.TerapeutaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(terapeutaModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TerapeutaModelExists(terapeutaModel.TerapeutaId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                // Redireciona para os detalhes após edição
                return RedirectToAction(nameof(Details), new
                {
                    id = terapeutaModel.TerapeutaId,
                    successMessage = "Dados do terapeuta atualizados com sucesso!"
                });
            }

            return View(terapeutaModel);
        }

        // GET: Terapeuta/Delete/5

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var terapeutaModel = await _context.Terapeutas
                .FirstOrDefaultAsync(m => m.TerapeutaId == id);

            if (terapeutaModel == null)
            {
                return NotFound();
            }

            return View(terapeutaModel);
        }

        // POST: Terapeuta/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var terapeutaModel = await _context.Terapeutas.FindAsync(id);

            if (terapeutaModel != null)
            {
                _context.Terapeutas.Remove(terapeutaModel);
                await _context.SaveChangesAsync();
            }

            // Redireciona para a lista com mensagem de sucesso
            return RedirectToAction(nameof(Index), new
            {
                successMessage = "Terapeuta eliminado com sucesso!"
            });
        }


        // Função para verificar se o terapeuta existe

        private bool TerapeutaModelExists(int id)
        {
            return _context.Terapeutas.Any(e => e.TerapeutaId == id);
        }
    }
}