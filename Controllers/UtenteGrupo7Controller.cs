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
    public class UtenteGrupo7Controller : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public UtenteGrupo7Controller(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: UtenteGrupo7
        public async Task<IActionResult> Index(string searchNome, int page = 1)
        {
            // Consulta base
            var query = _context.UtenteGrupo7.AsQueryable();

            // Lógica de Filtro (Por Nome)
            if (!string.IsNullOrEmpty(searchNome))
            {
                query = query.Where(u => u.Nome.Contains(searchNome));
                // Guardar na ViewBag para manter o valor na caixa de texto
                ViewBag.SearchNome = searchNome;
            }

            // Paginação
            int totalItems = await query.CountAsync();
            var pagination = new PaginationInfo<UtenteGrupo7>(page, totalItems);

            pagination.Items = await query
                .OrderBy(u => u.Nome) // Ordenar alfabeticamente
                .Skip(pagination.ItemsToSkip)
                .Take(pagination.ItemsPerPage)
                .ToListAsync();

            return View(pagination);
        }

        // GET: UtenteGrupo7/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var utenteGrupo7 = await _context.UtenteGrupo7
                .FirstOrDefaultAsync(m => m.UtenteGrupo7Id == id);

            if (utenteGrupo7 == null) return NotFound();

            return View(utenteGrupo7);
        }

        // GET: UtenteGrupo7/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UtenteGrupo7/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UtenteGrupo7Id,Nome")] UtenteGrupo7 utenteGrupo7)
        {
            // VALIDAÇÃO DE DUPLICADOS:
            // Impede criar dois Utentes com o mesmo nome
            bool existeRegisto = await _context.UtenteGrupo7
                .AnyAsync(u => u.Nome.ToLower() == utenteGrupo7.Nome.ToLower());

            if (existeRegisto)
            {
                ModelState.AddModelError("Nome", "Já existe um Utente com este nome.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(utenteGrupo7);
                await _context.SaveChangesAsync();

                // Mensagem de Sucesso e Redirecionamento para Details
                return RedirectToAction(nameof(Details), new { id = utenteGrupo7.UtenteGrupo7Id, SuccessMessage = "Utente criado com sucesso" });
            }
            return View(utenteGrupo7);
        }

        // GET: UtenteGrupo7/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var utenteGrupo7 = await _context.UtenteGrupo7.FindAsync(id);
            if (utenteGrupo7 == null) return NotFound();

            return View(utenteGrupo7);
        }

        // POST: UtenteGrupo7/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UtenteGrupo7Id,Nome")] UtenteGrupo7 utenteGrupo7)
        {
            if (id != utenteGrupo7.UtenteGrupo7Id) return NotFound();

            // VALIDAÇÃO DE DUPLICADOS NA EDIÇÃO
            // Verifica se existe OUTRO registo (com ID diferente) com o mesmo nome
            bool existeOutro = await _context.UtenteGrupo7
                .AnyAsync(u => u.Nome == utenteGrupo7.Nome && u.UtenteGrupo7Id != id);

            if (existeOutro)
            {
                ModelState.AddModelError("Nome", "Já existe outro Utente com este nome.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var utenteExistente = await _context.UtenteGrupo7.FindAsync(id);

                    // Tratamento de registo apagado por outro user (Concurrency)
                    if (utenteExistente == null)
                    {
                        // Passamos o modelo 'utenteGrupo7' (que contém os dados do form) para a View de recuperação
                        return View("InvalidUtenteGrupo7", utenteGrupo7);
                    }

                    // Atualizar valores de forma segura
                    _context.Entry(utenteExistente).CurrentValues.SetValues(utenteGrupo7);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Details), new { id = utenteGrupo7.UtenteGrupo7Id, SuccessMessage = "Utente editado com sucesso" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UtenteGrupo7Exists(utenteGrupo7.UtenteGrupo7Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(utenteGrupo7);
        }

        // GET: UtenteGrupo7/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var utenteGrupo7 = await _context.UtenteGrupo7
                .FirstOrDefaultAsync(m => m.UtenteGrupo7Id == id);

            if (utenteGrupo7 == null)
            {
                TempData["SuccessMessage"] = "Este Utente já foi eliminado.";
                return RedirectToAction(nameof(Index));
            }

            return View(utenteGrupo7);
        }

        // POST: UtenteGrupo7/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var utenteGrupo7 = await _context.UtenteGrupo7.FindAsync(id);

            if (utenteGrupo7 != null)
            {
                _context.UtenteGrupo7.Remove(utenteGrupo7);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Utente apagado com sucesso.";
            }
            else
            {
                TempData["SuccessMessage"] = "Este Utente já tinha sido eliminado.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool UtenteGrupo7Exists(int id)
        {
            return _context.UtenteGrupo7.Any(e => e.UtenteGrupo7Id == id);
        }
    }
}