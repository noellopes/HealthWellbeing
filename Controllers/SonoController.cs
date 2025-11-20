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
    public class SonoController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public SonoController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Sono
        // Adicionada pesquisa por DATA e Paginação
        public async Task<IActionResult> Index(DateTime? searchData, int page = 1)
        {
            // Consulta base com Include (Eager Loading)
            var query = _context.Sono.Include(s => s.UtenteGrupo7).AsQueryable();

            // Lógica de Filtro (Por Data)
            if (searchData.HasValue)
            {
                query = query.Where(s => s.Data.Date == searchData.Value.Date);
                // Guardar na ViewBag para manter o valor no input date
                ViewBag.SearchData = searchData.Value.ToString("yyyy-MM-dd");
            }

            // Paginação
            int totalItems = await query.CountAsync();
            var pagination = new PaginationInfo<Sono>(page, totalItems);

            pagination.Items = await query
                .OrderByDescending(s => s.Data) // Ordenar do mais recente para o mais antigo
                .Skip(pagination.ItemsToSkip)
                .Take(pagination.ItemsPerPage)
                .ToListAsync();

            return View(pagination);
        }

        // GET: Sono/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var sono = await _context.Sono
                .Include(s => s.UtenteGrupo7)
                .FirstOrDefaultAsync(m => m.SonoId == id);

            if (sono == null) return NotFound();

            ViewData["NomeUtente"] = sono.UtenteGrupo7.Nome;

            return View(sono);
        }

        // GET: Sono/Create
        public IActionResult Create()
        {
            // ALTERAÇÃO AQUI:
            // Mudei o terceiro parâmetro de "UtenteGrupo7Id" para "Nome".
            // Adicionei .OrderBy(u => u.Nome) para a lista ficar ordenada alfabeticamente.
            ViewData["UtenteGrupo7Id"] = new SelectList(
                _context.UtenteGrupo7.OrderBy(u => u.Nome),
                "UtenteGrupo7Id",
                "Nome"
            );

            return View();
        }

        // POST: Sono/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SonoId,Data,HoraDeitar,HoraLevantar,HorasSono,UtenteGrupo7Id")] Sono sono)
        {
            // Remover validação de navegação
            ModelState.Remove("UtenteGrupo7");

            // VALIDAÇÃO DE DUPLICADOS:
            bool existeRegisto = await _context.Sono
                .AnyAsync(s => s.UtenteGrupo7Id == sono.UtenteGrupo7Id && s.Data == sono.Data);

            if (existeRegisto)
            {
                ModelState.AddModelError("Data", "Já existe um registo de sono para este utente nesta data.");
            }

            if (ModelState.IsValid)
            {
                // --- LÓGICA DE CÁLCULO AUTOMÁTICO ---
                TimeSpan diferenca = sono.HoraLevantar - sono.HoraDeitar;

                if (diferenca.TotalMinutes < 0)
                {
                    diferenca = diferenca.Add(new TimeSpan(24, 0, 0));
                }
                sono.HorasSono = diferenca;

                _context.Add(sono);
                await _context.SaveChangesAsync();

                // Mensagem de Sucesso
                return RedirectToAction(nameof(Details), new { id = sono.SonoId, SuccessMessage = "Registo de sono criado com sucesso" });
            }

            // ALTERAÇÃO AQUI TAMBÉM (Caso haja erro e a página recarregue):
            // É necessário recriar a lista com "Nome" para o dropdown não "partir" ou voltar a mostrar IDs
            ViewData["UtenteGrupo7Id"] = new SelectList(
                _context.UtenteGrupo7.OrderBy(u => u.Nome),
                "UtenteGrupo7Id",
                "Nome",
                sono.UtenteGrupo7Id
            );

            return View(sono);
        }

        // GET: Sono/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var sono = await _context.Sono.FindAsync(id);
            if (sono == null) return NotFound();

            ViewData["UtenteGrupo7Id"] = new SelectList(
                _context.UtenteGrupo7.OrderBy(u => u.Nome),
                "UtenteGrupo7Id",
                "Nome",
                sono.UtenteGrupo7Id
            );
            return View(sono);
        }

        // POST: Sono/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SonoId,Data,HoraDeitar,HoraLevantar,HorasSono,UtenteGrupo7Id")] Sono sono)
        {
            if (id != sono.SonoId) return NotFound();

            // Remover validação de navegação
            ModelState.Remove("UtenteGrupo7");

            // VALIDAÇÃO DE DUPLICADOS NA EDIÇÃO
            // Verifica se existe OUTRO registo (com ID diferente) para o mesmo utente na mesma data
            bool existeOutro = await _context.Sono
                .AnyAsync(s => s.UtenteGrupo7Id == sono.UtenteGrupo7Id
                            && s.Data == sono.Data
                            && s.SonoId != id);

            if (existeOutro)
            {
                ModelState.AddModelError("Data", "Já existe outro registo de sono para este utente nesta data.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var sonoExistente = await _context.Sono.FindAsync(id);

                    // Tratamento de registo apagado por outro user
                    if (sonoExistente == null)
                    {
                        if (sonoExistente == null)
                        {
                            // Passamos o modelo 'sono' (que contém os dados do form) para a View de recuperação
                            return View("InvalidSono", sono);
                        }
                    }

                    // --- LÓGICA DE CÁLCULO AUTOMÁTICO ---
                    TimeSpan diferenca = sono.HoraLevantar - sono.HoraDeitar;
                    if (diferenca.TotalMinutes < 0)
                    {
                        diferenca = diferenca.Add(new TimeSpan(24, 0, 0));
                    }
                    sono.HorasSono = diferenca;

                    // Atualizar valores
                    _context.Entry(sonoExistente).CurrentValues.SetValues(sono);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Details), new { id = sono.SonoId, SuccessMessage = "Registo de sono editado com sucesso" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SonoExists(sono.SonoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewData["UtenteGrupo7Id"] = new SelectList(
                _context.UtenteGrupo7.OrderBy(u => u.Nome),
                "UtenteGrupo7Id",
                "Nome",
                sono.UtenteGrupo7Id
            );
            return View(sono);
        }

        // GET: Sono/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var sono = await _context.Sono
                .Include(s => s.UtenteGrupo7)
                .FirstOrDefaultAsync(m => m.SonoId == id);

            if (sono == null)
            {
                TempData["SuccessMessage"] = "Este registo já foi eliminado.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["NomeUtente"] = sono.UtenteGrupo7.Nome;

            return View(sono);
        }

        // POST: Sono/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sono = await _context.Sono.FindAsync(id);

            if (sono != null)
            {
                _context.Sono.Remove(sono);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Registo de sono apagado com sucesso.";
            }
            else
            {
                TempData["SuccessMessage"] = "Este registo já tinha sido eliminado.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool SonoExists(int id)
        {
            return _context.Sono.Any(e => e.SonoId == id);
        }
    }
}
