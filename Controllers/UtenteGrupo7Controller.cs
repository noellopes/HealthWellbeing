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

        // --- MÉTODOS DE SUPORTE ---

        // Preenche a Dropdown do Objetivo Físico
        private async Task PopularObjetivosDropDownList(object selectedId = null)
        {
            var objetivos = await _context.ObjetivoFisico
                                          .OrderBy(o => o.NomeObjetivo)
                                          .AsNoTracking()
                                          .ToListAsync();

            ViewBag.ObjetivoFisicoId = new SelectList(objetivos, "ObjetivoFisicoId", "NomeObjetivo", selectedId);
        }

        // Preenche as Checkboxes dos Problemas de Saúde
        private async Task PopularProblemasSaudeCheckboxes(UtenteGrupo7 utente = null)
        {
            // 1. Buscar todos os problemas disponíveis
            var todosProblemas = await _context.ProblemaSaude
                                               .AsNoTracking()
                                               .OrderBy(p => p.ProblemaCategoria)
                                               .ThenBy(p => p.ProblemaNome)
                                               .ToListAsync();

            // 2. Identificar quais já estão selecionados (no caso de Edit)
            var utenteProblemasIds = new HashSet<int>();
            if (utente != null && utente.UtenteProblemasSaude != null)
            {
                utenteProblemasIds = new HashSet<int>(
                    utente.UtenteProblemasSaude.Select(up => up.ProblemaSaudeId));
            }

            // 3. Criar a lista de SelectListItem para a View
            ViewBag.ProblemasSaudeCheckboxes = todosProblemas.Select(p => new SelectListItem
            {
                Value = p.ProblemaSaudeId.ToString(),
                Text = $"{p.ProblemaNome} ({p.ProblemaCategoria}, {p.ZonaAtingida})",
                Selected = utenteProblemasIds.Contains(p.ProblemaSaudeId)
            }).ToList();
        }

        // Atualiza a relação Muitos-para-Muitos durante a Edição
        private void UpdateUtenteProblemasSaude(UtenteGrupo7 utenteToUpdate, int[] selectedProblemas)
        {
            var selectedProblemasHs = new HashSet<int>(selectedProblemas ?? new int[] { });
            var utenteProblemasHs = new HashSet<int>(utenteToUpdate.UtenteProblemasSaude.Select(up => up.ProblemaSaudeId));

            var todosProblemasIds = _context.ProblemaSaude.Select(p => p.ProblemaSaudeId).ToList();

            foreach (var problemaId in todosProblemasIds)
            {
                if (selectedProblemasHs.Contains(problemaId))
                {
                    // Se foi selecionado mas não existe na coleção: ADICIONAR
                    if (!utenteProblemasHs.Contains(problemaId))
                    {
                        utenteToUpdate.UtenteProblemasSaude.Add(new UtenteGrupo7ProblemaSaude
                        {
                            UtenteGrupo7Id = utenteToUpdate.UtenteGrupo7Id,
                            ProblemaSaudeId = problemaId
                        });
                    }
                }
                else
                {
                    // Se não foi selecionado mas existe na coleção: REMOVER
                    if (utenteProblemasHs.Contains(problemaId))
                    {
                        var problemaToRemove = utenteToUpdate.UtenteProblemasSaude
                            .FirstOrDefault(up => up.ProblemaSaudeId == problemaId);

                        if (problemaToRemove != null)
                        {
                            _context.Remove(problemaToRemove); // Remove da tabela de junção
                        }
                    }
                }
            }
        }

        // --- AÇÕES DO CONTROLADOR ---

        // GET: UtenteGrupo7
        public async Task<IActionResult> Index(string searchNome, int page = 1)
        {
            var query = _context.UtenteGrupo7.AsQueryable();

            if (!string.IsNullOrEmpty(searchNome))
            {
                query = query.Where(u => u.Nome.Contains(searchNome));
                ViewBag.SearchNome = searchNome;
            }

            int totalItems = await query.CountAsync();
            var pagination = new PaginationInfo<UtenteGrupo7>(page, totalItems);

            pagination.Items = await query
                .OrderBy(u => u.Nome)
                .Skip(pagination.ItemsToSkip)
                .Take(pagination.ItemsPerPage)
                .ToListAsync();

            return View(pagination);
        }

        // GET: UtenteGrupo7/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var utenteGrupo7 = await _context.UtenteGrupo7
                .Include(u => u.ObjetivoFisico)
                .Include(u => u.UtenteProblemasSaude) // Inclui a tabela de junção
                    .ThenInclude(up => up.ProblemaSaude) // Inclui o ProblemaSaude real
                .FirstOrDefaultAsync(m => m.UtenteGrupo7Id == id);

            if (utenteGrupo7 == null) return NotFound();

            return View(utenteGrupo7);
        }

        // GET: UtenteGrupo7/Create
        public async Task<IActionResult> Create()
        {
            await PopularObjetivosDropDownList();
            await PopularProblemasSaudeCheckboxes(); // Carrega as checkboxes vazias
            return View();
        }

        // POST: UtenteGrupo7/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UtenteGrupo7Id,Nome,ObjetivoFisicoId")] UtenteGrupo7 utenteGrupo7, int[] selectedProblemas)
        {
            // Validação de Nome Duplicado
            bool existeRegisto = await _context.UtenteGrupo7
                .AnyAsync(u => u.Nome.ToLower() == utenteGrupo7.Nome.ToLower());

            if (existeRegisto)
            {
                ModelState.AddModelError("Nome", "Já existe um Utente com este nome.");
            }

            // Inicializar e preencher a coleção de problemas de saúde
            utenteGrupo7.UtenteProblemasSaude = new List<UtenteGrupo7ProblemaSaude>();
            if (selectedProblemas != null)
            {
                foreach (var problemaId in selectedProblemas)
                {
                    utenteGrupo7.UtenteProblemasSaude.Add(new UtenteGrupo7ProblemaSaude
                    {
                        ProblemaSaudeId = problemaId
                    });
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(utenteGrupo7);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = utenteGrupo7.UtenteGrupo7Id, SuccessMessage = "Utente criado com sucesso" });
            }

            // Recarregar listas em caso de erro
            await PopularObjetivosDropDownList(utenteGrupo7.ObjetivoFisicoId);
            await PopularProblemasSaudeCheckboxes(utenteGrupo7);
            return View(utenteGrupo7);
        }

        // GET: UtenteGrupo7/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var utenteGrupo7 = await _context.UtenteGrupo7
                .Include(u => u.UtenteProblemasSaude) // Necessário para marcar as checkboxes
                .FirstOrDefaultAsync(m => m.UtenteGrupo7Id == id);

            if (utenteGrupo7 == null) return NotFound();

            await PopularObjetivosDropDownList(utenteGrupo7.ObjetivoFisicoId);
            await PopularProblemasSaudeCheckboxes(utenteGrupo7); // Carrega checkboxes com seleção atual

            return View(utenteGrupo7);
        }

        // POST: UtenteGrupo7/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UtenteGrupo7Id,Nome,ObjetivoFisicoId")] UtenteGrupo7 utenteGrupo7, int[] selectedProblemas)
        {
            if (id != utenteGrupo7.UtenteGrupo7Id) return NotFound();

            // Buscar a entidade na BD com as relações para poder editar
            var utenteToUpdate = await _context.UtenteGrupo7
                .Include(u => u.UtenteProblemasSaude)
                .FirstOrDefaultAsync(u => u.UtenteGrupo7Id == id);

            if (utenteToUpdate == null) return View("InvalidUtenteGrupo7", utenteGrupo7);

            // Atualizar valores simples (Nome, Objetivo)
            _context.Entry(utenteToUpdate).CurrentValues.SetValues(utenteGrupo7);

            // Validação de Duplicados
            bool existeOutro = await _context.UtenteGrupo7
                .AnyAsync(u => u.Nome == utenteToUpdate.Nome && u.UtenteGrupo7Id != id);

            if (existeOutro)
            {
                ModelState.AddModelError("Nome", "Já existe outro Utente com este nome.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Atualizar a relação Many-to-Many
                    UpdateUtenteProblemasSaude(utenteToUpdate, selectedProblemas);

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new { id = utenteToUpdate.UtenteGrupo7Id, SuccessMessage = "Utente editado com sucesso" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UtenteGrupo7Exists(utenteToUpdate.UtenteGrupo7Id)) return NotFound();
                    else throw;
                }
            }

            // Recarregar listas em caso de erro
            await PopularObjetivosDropDownList(utenteGrupo7.ObjetivoFisicoId);
            await PopularProblemasSaudeCheckboxes(utenteToUpdate);

            // Forçar visualmente a seleção que o utilizador tentou fazer (se falhou a validação)
            var listaProblemas = ViewBag.ProblemasSaudeCheckboxes as List<SelectListItem>;
            if (selectedProblemas != null && listaProblemas != null)
            {
                foreach (var item in listaProblemas)
                {
                    item.Selected = selectedProblemas.Contains(int.Parse(item.Value));
                }
            }

            return View(utenteGrupo7);
        }

        // GET: UtenteGrupo7/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var utenteGrupo7 = await _context.UtenteGrupo7
                .Include(u => u.Sonos) // Para verificar dependências
                .FirstOrDefaultAsync(m => m.UtenteGrupo7Id == id);

            if (utenteGrupo7 == null)
            {
                TempData["SuccessMessage"] = "Este Utente já foi eliminado.";
                return RedirectToAction(nameof(Index));
            }

            int numDependencias = utenteGrupo7.Sonos?.Count ?? 0;
            ViewBag.NumDependencias = numDependencias;
            ViewBag.PodeEliminar = numDependencias == 0;

            return View(utenteGrupo7);
        }

        // POST: UtenteGrupo7/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var utenteGrupo7 = await _context.UtenteGrupo7
                .Include(u => u.Sonos)
                .FirstOrDefaultAsync(m => m.UtenteGrupo7Id == id);

            if (utenteGrupo7 == null)
            {
                TempData["SuccessMessage"] = "Este Utente já tinha sido eliminado.";
                return RedirectToAction(nameof(Index));
            }

            // Verificação de Segurança de dependências
            if ((utenteGrupo7.Sonos?.Count ?? 0) > 0)
            {
                TempData["ErrorMessage"] = "Não é possível eliminar o utente. Existem registos associados.";
                return RedirectToAction(nameof(Delete), new { id = id });
            }

            _context.UtenteGrupo7.Remove(utenteGrupo7);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Utente apagado com sucesso.";

            return RedirectToAction(nameof(Index));
        }

        private bool UtenteGrupo7Exists(int id)
        {
            return _context.UtenteGrupo7.Any(e => e.UtenteGrupo7Id == id);
        }
    }
}