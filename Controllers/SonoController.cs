using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace HealthWellbeing.Controllers
{
    public class SonoController : Controller
    {
        private readonly HealthWellbeingDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public SonoController(HealthWellbeingDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Sono
        public async Task<IActionResult> Index(DateTime? searchData, int? searchUtenteId, int page = 1)
        {
            var query = _context.Sono.Include(s => s.UtenteGrupo7).AsQueryable();
            var userId = _userManager.GetUserId(User);

            // --- LÓGICA DE PERMISSÕES ---

            // 1. Verificar se é Admin ou Profissional
            bool isStaff = User.IsInRole("Administrador") || User.IsInRole("Profissional");

            if (isStaff)
            {
                // CENÁRIO STAFF:
                // Pode ver tudo, mas pode querer filtrar por um utente específico.

                // Carrega a lista de utentes para a Dropdown na View
                ViewData["ListaUtentes"] = new SelectList(_context.UtenteGrupo7.OrderBy(u => u.Nome), "UtenteGrupo7Id", "Nome", searchUtenteId);

                // Se selecionou alguém na dropdown, aplica o filtro
                if (searchUtenteId.HasValue)
                {
                    query = query.Where(s => s.UtenteGrupo7Id == searchUtenteId.Value);
                    ViewBag.CurrentUtenteFilter = searchUtenteId.Value; // Para manter o filtro na paginação
                }
            }
            else
            {
                // CENÁRIO UTENTE NORMAL:
                // Só vê os registos associados ao seu Login ID
                query = query.Where(s => s.UtenteGrupo7.UserId == userId);
            }

            // Filtro de Data
            if (searchData.HasValue)
            {
                query = query.Where(s => s.Data.Date == searchData.Value.Date);
                ViewBag.SearchData = searchData.Value.ToString("yyyy-MM-dd");
            }

            // Paginação
            int totalItems = await query.CountAsync();
            var pagination = new PaginationInfo<Sono>(page, totalItems);

            pagination.Items = await query
                .OrderByDescending(s => s.Data)
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
        [Authorize]
        public async Task<IActionResult> Create()
        {
            // Verifica se é Staff (Admin ou Profissional)
            bool isStaff = User.IsInRole("Administrador") || User.IsInRole("Profissional");

            if (isStaff)
            {
                // CENÁRIO 1: STAFF
                // Carregamos a lista de todos os utentes para a Dropdown
                ViewData["UtenteGrupo7Id"] = new SelectList(
                    _context.UtenteGrupo7.OrderBy(u => u.Nome),
                    "UtenteGrupo7Id",
                    "Nome"
                );
            }
            else
            {
                // CENÁRIO 2: UTENTE NORMAL
                // Verificamos se ele já tem perfil criado
                var userId = _userManager.GetUserId(User);
                var temPerfil = await _context.UtenteGrupo7.AnyAsync(u => u.UserId == userId);

                if (!temPerfil)
                {
                    TempData["Error"] = "Precisa criar um perfil de Utente antes de registar o sono.";
                    return RedirectToAction("Create", "UtenteGrupo7");
                }
                // Não carregamos ViewData["UtenteGrupo7Id"] porque ele não vai ver a lista
            }

            return View();
        }

        // POST: Sono/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("SonoId,Data,HoraDeitar,HoraLevantar,HorasSono,UtenteGrupo7Id")] Sono sono)
        {
            // Remover validação da propriedade de navegação (objeto completo)
            ModelState.Remove("UtenteGrupo7");

            bool isStaff = User.IsInRole("Administrador") || User.IsInRole("Profissional");

            if (isStaff)
            {
                // --- LÓGICA DE STAFF ---
                // O ID vem da Dropdown (sono.UtenteGrupo7Id já deve vir preenchido)

                if (sono.UtenteGrupo7Id == 0) // Validação extra
                {
                    ModelState.AddModelError("UtenteGrupo7Id", "Por favor selecione um Utente.");
                }
                // Não removemos o erro do UtenteGrupo7Id do ModelState porque aqui ele É obrigatório no form
            }
            else
            {
                // --- LÓGICA DE UTENTE NORMAL ---
                // Ignoramos o que vem do form (se vier) e usamos o Login

                var userId = _userManager.GetUserId(User);
                var utente = await _context.UtenteGrupo7.FirstOrDefaultAsync(u => u.UserId == userId);

                if (utente == null)
                {
                    ModelState.AddModelError("", "Erro: Perfil de Utente não encontrado.");
                    return View(sono);
                }

                // Forçamos o ID correto
                sono.UtenteGrupo7Id = utente.UtenteGrupo7Id;

                // Removemos o erro de validação, pois o campo estava vazio/invisível no HTML
                ModelState.Remove("UtenteGrupo7Id");
            }

            // --- VALIDAÇÃO DE DUPLICADOS (Igual para os dois) ---
            bool existeRegisto = await _context.Sono
                .AnyAsync(s => s.UtenteGrupo7Id == sono.UtenteGrupo7Id && s.Data == sono.Data);

            if (existeRegisto)
            {
                ModelState.AddModelError("Data", "Já existe registo de sono para este utente nesta data.");
            }

            if (ModelState.IsValid)
            {
                // Cálculo de horas
                TimeSpan diferenca = sono.HoraLevantar - sono.HoraDeitar;
                if (diferenca.TotalMinutes < 0) diferenca = diferenca.Add(new TimeSpan(24, 0, 0));
                sono.HorasSono = diferenca;

                _context.Add(sono);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = sono.SonoId, SuccessMessage = "Registo criado com sucesso!" });
            }

            // SE FALHAR E TIVERMOS DE VOLTAR À VIEW:
            // Se for Staff, temos de recarregar a lista para a dropdown não desaparecer
            if (isStaff)
            {
                ViewData["UtenteGrupo7Id"] = new SelectList(
                    _context.UtenteGrupo7.OrderBy(u => u.Nome),
                    "UtenteGrupo7Id",
                    "Nome",
                    sono.UtenteGrupo7Id
                );
            }

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
