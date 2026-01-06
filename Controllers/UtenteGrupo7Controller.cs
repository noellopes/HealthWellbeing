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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace HealthWellbeing.Controllers
{
    // Apenas utilizadores autenticados podem aceder a este controlador
    [Authorize]
    public class UtenteGrupo7Controller : Controller
    {
        private readonly HealthWellbeingDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        // Adicionamos RoleManager para conseguir listar quem são os profissionais
        private readonly RoleManager<IdentityRole> _roleManager;

        public UtenteGrupo7Controller(HealthWellbeingDbContext context,
                                      UserManager<IdentityUser> userManager,
                                      RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // --- MÉTODOS DE SUPORTE ---

        private async Task PopularObjetivosDropDownList(object selectedId = null)
        {
            var objetivos = await _context.ObjetivoFisico
                                          .OrderBy(o => o.NomeObjetivo)
                                          .AsNoTracking()
                                          .ToListAsync();

            ViewBag.ObjetivoFisicoId = new SelectList(objetivos, "ObjetivoFisicoId", "NomeObjetivo", selectedId);
        }

        private async Task PopularProfissionaisDropDownList(object selectedId = null)
        {
            // Apenas carregamos esta lista se for Admin, para poupar recursos
            if (User.IsInRole("Administrador"))
            {
                // Busca todos os users que tenham a Role "ProfissionalSaude"
                var profissionais = await _userManager.GetUsersInRoleAsync("ProfissionalSaude");

                ViewBag.ProfissionalSaudeId = new SelectList(profissionais, "Id", "UserName", selectedId);
            }
        }

        private async Task PopularProblemasSaudeCheckboxes(UtenteGrupo7 utente = null)
        {
            var todosProblemas = await _context.ProblemaSaude
                                               .AsNoTracking()
                                               .OrderBy(p => p.ProblemaCategoria)
                                               .ThenBy(p => p.ProblemaNome)
                                               .ToListAsync();

            var utenteProblemasIds = new HashSet<int>();
            if (utente != null && utente.UtenteProblemasSaude != null)
            {
                utenteProblemasIds = new HashSet<int>(utente.UtenteProblemasSaude.Select(up => up.ProblemaSaudeId));
            }

            ViewBag.ProblemasSaudeCheckboxes = todosProblemas.Select(p => new SelectListItem
            {
                Value = p.ProblemaSaudeId.ToString(),
                Text = $"{p.ProblemaNome} ({p.ProblemaCategoria}, {p.ZonaAtingida})",
                Selected = utenteProblemasIds.Contains(p.ProblemaSaudeId)
            }).ToList();
        }

        private void UpdateUtenteProblemasSaude(UtenteGrupo7 utenteToUpdate, int[] selectedProblemas)
        {
            var selectedProblemasHs = new HashSet<int>(selectedProblemas ?? new int[] { });
            var utenteProblemasHs = new HashSet<int>(utenteToUpdate.UtenteProblemasSaude.Select(up => up.ProblemaSaudeId));
            var todosProblemasIds = _context.ProblemaSaude.Select(p => p.ProblemaSaudeId).ToList();

            foreach (var problemaId in todosProblemasIds)
            {
                if (selectedProblemasHs.Contains(problemaId))
                {
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
                    if (utenteProblemasHs.Contains(problemaId))
                    {
                        var problemaToRemove = utenteToUpdate.UtenteProblemasSaude
                            .FirstOrDefault(up => up.ProblemaSaudeId == problemaId);

                        if (problemaToRemove != null)
                        {
                            _context.Remove(problemaToRemove);
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
            var currentUserId = _userManager.GetUserId(User);

            // LOGICA DE FILTRO DO INDEX
            if (User.IsInRole("Administrador"))
            {
                // Admin vê TODOS (não faz filtro extra)
            }
            else if (User.IsInRole("ProfissionalSaude"))
            {
                // Profissional vê APENAS os seus atribuídos
                // Assumindo que tens o campo ProfissionalSaudeId no modelo
                query = query.Where(u => u.ProfissionalSaudeId == currentUserId);
            }
            else
            {
                // Se for um Utente normal, vê apenas o seu próprio registo
                query = query.Where(u => u.UserId == currentUserId);
            }

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

            // 1. Recolher os IDs únicos
            var idsProfissionais = pagination.Items
                .Where(u => !string.IsNullOrEmpty(u.ProfissionalSaudeId))
                .Select(u => u.ProfissionalSaudeId)
                .Distinct()
                .ToList();

            // 2. CORREÇÃO: Usa '_userManager.Users' em vez de '_context.Users'
            var emailsDict = await _userManager.Users
                .Where(u => idsProfissionais.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.Email);

            // 3. Passar para a View
            ViewBag.ProfissionaisEmails = emailsDict;

            return View(pagination);
        }

        // GET: UtenteGrupo7/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var utenteGrupo7 = await _context.UtenteGrupo7
                .Include(u => u.ObjetivoFisico)
                .Include(u => u.UtenteProblemasSaude).ThenInclude(up => up.ProblemaSaude)
                // REMOVIDO: .Include(u => u.ProfissionalSaude) <-- Isto dava erro porque não existe relação
                .FirstOrDefaultAsync(m => m.UtenteGrupo7Id == id);

            if (utenteGrupo7 == null) return NotFound();

            // --- VERIFICAÇÕES DE SEGURANÇA (MANTIDAS) ---
            var currentUserId = _userManager.GetUserId(User);
            if (!User.IsInRole("Administrador"))
            {
                if (User.IsInRole("ProfissionalSaude"))
                {
                    if (utenteGrupo7.ProfissionalSaudeId != currentUserId) return Forbid();
                }
                else
                {
                    if (utenteGrupo7.UserId != currentUserId) return Forbid();
                }
            }

            // --- LÓGICA NOVA: BUSCAR O EMAIL DO PROFISSIONAL ---
            string emailProfissional = "Não Atribuído";

            if (!string.IsNullOrEmpty(utenteGrupo7.ProfissionalSaudeId))
            {
                // Usamos o UserManager para encontrar o user pelo ID (guid)
                var userProfissional = await _userManager.FindByIdAsync(utenteGrupo7.ProfissionalSaudeId);
                if (userProfissional != null)
                {
                    emailProfissional = userProfissional.Email;
                }
                else
                {
                    emailProfissional = "Utilizador não encontrado (ID inválido)";
                }
            }

            // Passamos o email para a View via ViewBag
            ViewBag.EmailProfissional = emailProfissional;

            return View(utenteGrupo7);
        }

        // GET: UtenteGrupo7/Create
        public async Task<IActionResult> Create()
        {
            await PopularObjetivosDropDownList();
            await PopularProblemasSaudeCheckboxes();
            // Carrega lista de profissionais apenas se for Admin
            await PopularProfissionaisDropDownList();
            return View();
        }

        // POST: UtenteGrupo7/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UtenteGrupo7Id,Nome,ObjetivoFisicoId,ProfissionalSaudeId")] UtenteGrupo7 utenteGrupo7, int[] selectedProblemas)
        {
            // Obter o utilizador atual completo (necessário para verificar e adicionar Roles)
            var user = await _userManager.GetUserAsync(User);
            var currentUserId = user.Id;

            bool isAdmin = await _userManager.IsInRoleAsync(user, "Administrador");
            bool isProf = await _userManager.IsInRoleAsync(user, "ProfissionalSaude");

            if (isAdmin || isProf)
            {
                TempData["MensagemErro"] = "Erro: Contas de Administrador ou Profissional de Saúde não podem criar perfil de Utente.";

                await PopularObjetivosDropDownList(utenteGrupo7.ObjetivoFisicoId);
                await PopularProblemasSaudeCheckboxes(utenteGrupo7);
                await PopularProfissionaisDropDownList(utenteGrupo7.ProfissionalSaudeId);

                return View(utenteGrupo7);
            }

            utenteGrupo7.UserId = currentUserId;
            ModelState.Remove("UserId");

            // SEGURANÇA: Apenas Admin pode definir o ProfissionalSaudeId no formulário
            // (Mas como bloqueámos o Admin acima, esta linha é redundante para o próprio criador, 
            // mas boa de manter caso a lógica mude no futuro)
            if (!isAdmin)
            {
                utenteGrupo7.ProfissionalSaudeId = null;
            }

            bool existeRegisto = await _context.UtenteGrupo7.AnyAsync(u => u.Nome.ToLower() == utenteGrupo7.Nome.ToLower());
            if (existeRegisto) ModelState.AddModelError("Nome", "Já existe um Utente com este nome.");

            bool existeid = await _context.UtenteGrupo7.AnyAsync(u => u.UserId == currentUserId);
            if (existeid) ModelState.AddModelError("Nome", "Só pode registrar-se uma vez.");

            utenteGrupo7.UtenteProblemasSaude = new List<UtenteGrupo7ProblemaSaude>();
            if (selectedProblemas != null)
            {
                foreach (var problemaId in selectedProblemas)
                {
                    utenteGrupo7.UtenteProblemasSaude.Add(new UtenteGrupo7ProblemaSaude { ProblemaSaudeId = problemaId });
                }
            }

            if (ModelState.IsValid)
            {
                // 2. Guardar os dados do perfil na BD
                _context.Add(utenteGrupo7);
                await _context.SaveChangesAsync();

                // 3. ATRIBUIR ROLE "Utente" AUTOMATICAMENTE
                // Só atribuímos se não tiver nenhuma das roles proibidas (já verificado acima)
                // e se ainda não tiver a role "Utente".
                if (!await _userManager.IsInRoleAsync(user, "Utente"))
                {
                    await _userManager.AddToRoleAsync(user, "Utente");
                }

                return RedirectToAction(nameof(Details), new { id = utenteGrupo7.UtenteGrupo7Id, SuccessMessage = "Utente criado com sucesso" });
            }

            // Se falhar (ModelState inválido), recarrega tudo
            await PopularObjetivosDropDownList(utenteGrupo7.ObjetivoFisicoId);
            await PopularProblemasSaudeCheckboxes(utenteGrupo7);
            await PopularProfissionaisDropDownList(utenteGrupo7.ProfissionalSaudeId);
            return View(utenteGrupo7);
        }

        // GET: UtenteGrupo7/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var utenteGrupo7 = await _context.UtenteGrupo7
                .Include(u => u.UtenteProblemasSaude)
                .FirstOrDefaultAsync(m => m.UtenteGrupo7Id == id);

            if (utenteGrupo7 == null) return NotFound();

            // VERIFICAÇÃO DE PERMISSÃO PARA EDITAR
            var currentUserId = _userManager.GetUserId(User);

            if (User.IsInRole("Administrador"))
            {
                // Admin pode editar tudo
            }
            else if (User.IsInRole("ProfissionalSaude"))
            {
                // Profissional só edita os seus
                if (utenteGrupo7.ProfissionalSaudeId != currentUserId)
                    return Forbid(); // Ou Redirect para AccessDenied
            }
            else
            {
                // User normal só edita o seu próprio
                if (utenteGrupo7.UserId != currentUserId)
                    return Forbid();
            }

            await PopularObjetivosDropDownList(utenteGrupo7.ObjetivoFisicoId);
            await PopularProblemasSaudeCheckboxes(utenteGrupo7);
            await PopularProfissionaisDropDownList(utenteGrupo7.ProfissionalSaudeId); // Apenas Admin verá isto na View

            return View(utenteGrupo7);
        }

        // POST: UtenteGrupo7/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UtenteGrupo7Id,Nome,ObjetivoFisicoId,ProfissionalSaudeId")] UtenteGrupo7 utenteGrupo7, int[] selectedProblemas)
        {
            if (id != utenteGrupo7.UtenteGrupo7Id) return NotFound();

            ModelState.Remove("UserId");

            var utenteToUpdate = await _context.UtenteGrupo7
                .Include(u => u.UtenteProblemasSaude)
                .FirstOrDefaultAsync(u => u.UtenteGrupo7Id == id);

            if (utenteToUpdate == null) return View("InvalidUtenteGrupo7", utenteGrupo7);

            // VERIFICAÇÃO DE PERMISSÃO NO POST (CRUCIAL)
            var currentUserId = _userManager.GetUserId(User);
            if (!User.IsInRole("Administrador"))
            {
                if (User.IsInRole("ProfissionalSaude") && utenteToUpdate.ProfissionalSaudeId != currentUserId) return Forbid();
                if (!User.IsInRole("ProfissionalSaude") && utenteToUpdate.UserId != currentUserId) return Forbid();
            }

            // Atualizar campos permitidos
            utenteToUpdate.Nome = utenteGrupo7.Nome;
            utenteToUpdate.ObjetivoFisicoId = utenteGrupo7.ObjetivoFisicoId;

            // SEGURANÇA: Apenas Admin altera o Profissional
            if (User.IsInRole("Administrador"))
            {
                utenteToUpdate.ProfissionalSaudeId = utenteGrupo7.ProfissionalSaudeId;
            }
            // Nota: Se não for Admin, ignoramos o ProfissionalSaudeId que veio do form, 
            // mantendo o original que está em utenteToUpdate.

            // Validação de Duplicados
            bool existeOutro = await _context.UtenteGrupo7
                .AnyAsync(u => u.Nome == utenteToUpdate.Nome && u.UtenteGrupo7Id != id);

            if (existeOutro) ModelState.AddModelError("Nome", "Já existe outro Utente com este nome.");

            if (ModelState.IsValid)
            {
                try
                {
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

            await PopularObjetivosDropDownList(utenteGrupo7.ObjetivoFisicoId);
            await PopularProblemasSaudeCheckboxes(utenteToUpdate);
            await PopularProfissionaisDropDownList(utenteToUpdate.ProfissionalSaudeId);

            // Re-mapear seleções visuais em caso de erro
            var listaProblemas = ViewBag.ProblemasSaudeCheckboxes as List<SelectListItem>;
            if (selectedProblemas != null && listaProblemas != null)
            {
                foreach (var item in listaProblemas)
                    item.Selected = selectedProblemas.Contains(int.Parse(item.Value));
            }

            return View(utenteGrupo7);
        }

        // GET: UtenteGrupo7/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var utenteGrupo7 = await _context.UtenteGrupo7
                .Include(u => u.ObjetivoFisico) // É bom incluir para mostrar o nome do objetivo também
                .Include(u => u.Sonos)
                .FirstOrDefaultAsync(m => m.UtenteGrupo7Id == id);

            if (utenteGrupo7 == null) return NotFound();

            // LOGICA DE SEGURANÇA
            var currentUserId = _userManager.GetUserId(User);

            if (User.IsInRole("Administrador"))
            {
                // Admin pode apagar qualquer um
            }
            else if (User.IsInRole("ProfissionalSaude"))
            {
                if (utenteGrupo7.ProfissionalSaudeId != currentUserId)
                {
                    TempData["ErrorMessage"] = "Não tem permissão para eliminar este utente.";
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Apenas administradores e profissionais de saúde podem eliminar registos.";
                return RedirectToAction(nameof(Index));
            }

            // --- NOVA LÓGICA: BUSCAR EMAIL DO PROFISSIONAL ---
            string emailProfissional = "Não Atribuído";
            if (!string.IsNullOrEmpty(utenteGrupo7.ProfissionalSaudeId))
            {
                var userProf = await _userManager.FindByIdAsync(utenteGrupo7.ProfissionalSaudeId);
                if (userProf != null)
                {
                    emailProfissional = userProf.Email;
                }
            }
            ViewBag.EmailProfissional = emailProfissional;
            // ------------------------------------------------

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

            if (utenteGrupo7 == null) return RedirectToAction(nameof(Index));

            // LOGICA DE SEGURANÇA (Repetir a verificação do GET para garantir segurança)
            var currentUserId = _userManager.GetUserId(User);
            if (!User.IsInRole("Administrador"))
            {
                if (User.IsInRole("ProfissionalSaude"))
                {
                    if (utenteGrupo7.ProfissionalSaudeId != currentUserId) return Forbid();
                }
                else
                {
                    return Forbid();
                }
            }

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