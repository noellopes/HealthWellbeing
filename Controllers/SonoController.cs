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

            bool isAdmin = User.IsInRole("Administrador");
            bool isProf = User.IsInRole("ProfissionalSaude");

            if (isAdmin)
            {
                // 1. ADMINISTRADOR: Vê tudo
                ViewData["ListaUtentes"] = new SelectList(
                    await _context.UtenteGrupo7.OrderBy(u => u.Nome).ToListAsync(),
                    "UtenteGrupo7Id",
                    "Nome",
                    searchUtenteId
                );
            }
            else if (isProf)
            {
                // 2. PROFISSIONAL: 
                query = query.Where(s => s.UtenteGrupo7.ProfissionalSaudeId == userId);

                // b) Dropdown apenas com os meus utentes
                var meusUtentes = await _context.UtenteGrupo7
                    .Where(u => u.ProfissionalSaudeId == userId)
                    .OrderBy(u => u.Nome)
                    .ToListAsync();

                ViewData["ListaUtentes"] = new SelectList(meusUtentes, "UtenteGrupo7Id", "Nome", searchUtenteId);
            }
            else
            {
                // 3. UTENTE: Vê apenas os seus próprios registos
                query = query.Where(s => s.UtenteGrupo7.UserId == userId);
            }


            if (searchUtenteId.HasValue)
            {
                query = query.Where(s => s.UtenteGrupo7Id == searchUtenteId.Value);
                ViewBag.CurrentUtenteFilter = searchUtenteId.Value;
            }

            if (searchData.HasValue)
            {
                query = query.Where(s => s.Data.Date == searchData.Value.Date);
                ViewBag.SearchData = searchData.Value.ToString("yyyy-MM-dd");
            }

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

            // Segurança: Impedir ver detalhes de outros se não for Staff
            var userId = _userManager.GetUserId(User);
            bool isStaff = User.IsInRole("Administrador") || User.IsInRole("ProfissionalSaude");

            if (!isStaff && sono.UtenteGrupo7.UserId != userId)
            {
                return Forbid();
            }

            ViewData["NomeUtente"] = sono.UtenteGrupo7.Nome;
            return View(sono);
        }

        // GET: Sono/Create
        [Authorize]
        public async Task<IActionResult> Create()
        {
            bool isStaff = User.IsInRole("Administrador") || User.IsInRole("ProfissionalSaude");

            if (isStaff)
            {
                ViewData["UtenteGrupo7Id"] = new SelectList(
                    _context.UtenteGrupo7.OrderBy(u => u.Nome), "UtenteGrupo7Id", "Nome");
            }
            else
            {
                var userId = _userManager.GetUserId(User);
                var temPerfil = await _context.UtenteGrupo7.AnyAsync(u => u.UserId == userId);

                if (!temPerfil)
                {
                    TempData["Error"] = "Precisa criar um perfil de Utente antes de registar o sono.";
                    return RedirectToAction("Create", "UtenteGrupo7");
                }
            }

            return View();
        }

        // POST: Sono/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("SonoId,Data,HoraDeitar,HoraLevantar,HorasSono,UtenteGrupo7Id")] Sono sono)
        {
            ModelState.Remove("UtenteGrupo7");
            bool isStaff = User.IsInRole("Administrador") || User.IsInRole("ProfissionalSaude");

            if (sono.Data.Date > DateTime.Now.Date)
            {
                ModelState.AddModelError("Data", "Não é permitido registar sono em datas futuras.");
            }

            if (isStaff)
            {
                if (sono.UtenteGrupo7Id == 0)
                    ModelState.AddModelError("UtenteGrupo7Id", "Por favor selecione um Utente.");
            }
            else
            {
                var userId = _userManager.GetUserId(User);
                var utente = await _context.UtenteGrupo7.FirstOrDefaultAsync(u => u.UserId == userId);

                if (utente == null)
                {
                    ModelState.AddModelError("", "Erro: Perfil de Utente não encontrado.");
                    return View(sono);
                }
                sono.UtenteGrupo7Id = utente.UtenteGrupo7Id;
                ModelState.Remove("UtenteGrupo7Id");
            }

            // Validação de Duplicados
            bool existeRegisto = await _context.Sono
                .AnyAsync(s => s.UtenteGrupo7Id == sono.UtenteGrupo7Id && s.Data == sono.Data);

            if (existeRegisto)
            {
                ModelState.AddModelError("Data", "Já existe registo de sono para este utente nesta data.");
            }

            if (ModelState.IsValid)
            {
                TimeSpan diferenca = sono.HoraLevantar - sono.HoraDeitar;
                if (diferenca.TotalMinutes < 0) diferenca = diferenca.Add(new TimeSpan(24, 0, 0));
                sono.HorasSono = diferenca;

                _context.Add(sono);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = sono.SonoId, SuccessMessage = "Registo criado com sucesso!" });
            }

            if (isStaff)
            {
                ViewData["UtenteGrupo7Id"] = new SelectList(
                    _context.UtenteGrupo7.OrderBy(u => u.Nome), "UtenteGrupo7Id", "Nome", sono.UtenteGrupo7Id);
            }

            return View(sono);
        }

        // GET: Sono/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var sono = await _context.Sono
                .Include(s => s.UtenteGrupo7)
                .FirstOrDefaultAsync(s => s.SonoId == id);

            if (sono == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            bool isStaff = User.IsInRole("Administrador") || User.IsInRole("ProfissionalSaude");

            if (!isStaff && sono.UtenteGrupo7.UserId != userId) return Forbid();

            if (isStaff)
            {
                ViewData["UtenteGrupo7Id"] = new SelectList(
                    _context.UtenteGrupo7.OrderBy(u => u.Nome), "UtenteGrupo7Id", "Nome", sono.UtenteGrupo7Id);
            }

            ViewBag.NomeUtenteAtual = sono.UtenteGrupo7?.Nome;

            return View(sono);
        }

        // POST: Sono/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SonoId,Data,HoraDeitar,HoraLevantar,HorasSono,UtenteGrupo7Id")] Sono sono)
        {
            if (id != sono.SonoId) return NotFound();

            ModelState.Remove("UtenteGrupo7");
            bool isStaff = User.IsInRole("Administrador") || User.IsInRole("ProfissionalSaude");

            if (sono.Data.Date > DateTime.Now.Date)
            {
                ModelState.AddModelError("Data", "Não é permitido alterar para datas futuras.");
            }

            bool existeOutro = await _context.Sono
                .AnyAsync(s => s.UtenteGrupo7Id == sono.UtenteGrupo7Id && s.Data == sono.Data && s.SonoId != id);

            if (existeOutro)
            {
                ModelState.AddModelError("Data", "Já existe outro registo de sono para este utente nesta data.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var sonoExistente = await _context.Sono.FindAsync(id);
                    if (sonoExistente == null) return View("InvalidSono", sono);

                    // --- 2. SEGURANÇA: IMPEDIR MUDANÇA DE UTENTE ---
                    if (isStaff)
                    {
                        // Staff pode mudar o utente se quiser, usamos o valor do form
                        sonoExistente.UtenteGrupo7Id = sono.UtenteGrupo7Id;
                    }
                    // Se NÃO for staff, ignoramos o 'sono.UtenteGrupo7Id' que vem do form 
                    // e mantemos o 'sonoExistente.UtenteGrupo7Id' inalterado.

                    // Atualizar restantes dados
                    sonoExistente.Data = sono.Data;
                    sonoExistente.HoraDeitar = sono.HoraDeitar;
                    sonoExistente.HoraLevantar = sono.HoraLevantar;

                    // Recalcular horas
                    TimeSpan diferenca = sono.HoraLevantar - sono.HoraDeitar;
                    if (diferenca.TotalMinutes < 0) diferenca = diferenca.Add(new TimeSpan(24, 0, 0));
                    sonoExistente.HorasSono = diferenca;

                    _context.Update(sonoExistente);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Details), new { id = sono.SonoId, SuccessMessage = "Registo de sono editado com sucesso" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SonoExists(sono.SonoId)) return NotFound();
                    else throw;
                }
            }

            if (isStaff)
            {
                ViewData["UtenteGrupo7Id"] = new SelectList(
                    _context.UtenteGrupo7.OrderBy(u => u.Nome), "UtenteGrupo7Id", "Nome", sono.UtenteGrupo7Id);
            }

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

            // Segurança Delete GET
            var userId = _userManager.GetUserId(User);
            bool isStaff = User.IsInRole("Administrador") || User.IsInRole("ProfissionalSaude");

            if (!isStaff && sono.UtenteGrupo7.UserId != userId) return Forbid();

            ViewData["NomeUtente"] = sono.UtenteGrupo7.Nome;
            return View(sono);
        }

        // POST: Sono/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sono = await _context.Sono
                .Include(s => s.UtenteGrupo7)
                .FirstOrDefaultAsync(s => s.SonoId == id);

            if (sono == null) return RedirectToAction(nameof(Index));

            // Segurança Delete POST
            var userId = _userManager.GetUserId(User);
            bool isStaff = User.IsInRole("Administrador") || User.IsInRole("ProfissionalSaude");

            if (!isStaff && sono.UtenteGrupo7.UserId != userId) return Forbid();

            _context.Sono.Remove(sono);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Registo de sono apagado com sucesso.";

            return RedirectToAction(nameof(Index));
        }

        private bool SonoExists(int id)
        {
            return _context.Sono.Any(e => e.SonoId == id);
        }
    }
}