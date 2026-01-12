using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using System.Text.Json;
using System.Security.Claims;
using HealthWellbeing.Services;
using HealthWellbeing.ViewModels;

namespace HealthWellbeing.Controllers
{
    [Authorize]
    public class ReceitaController : Controller
    {
        private readonly HealthWellbeingDbContext _context;
        private readonly IReceitaAjusteService _ajusteService;

        public ReceitaController(HealthWellbeingDbContext context, IReceitaAjusteService ajusteService)
        {
            _context = context;
            _ajusteService = ajusteService;
        }

        // GET: Receita
        public async Task<IActionResult> Index(string searchNome, int? minTempoPreparo, int? maxTempoPreparo, int page = 1, int pageSize = 10)
        {
            // Base query
            var query = _context.Receita
                .Include(r => r.Componentes)
                    .ThenInclude(c => c.Alimento)
                .AsQueryable();

            // Filter by name (if provided)
            if (!string.IsNullOrWhiteSpace(searchNome))
            {
                var term = searchNome.Trim().ToLower();
                query = query.Where(r => r.Nome.ToLower().Contains(term) || 
                                        (r.Descricao != null && r.Descricao.ToLower().Contains(term)));
            }

            // Filter by preparation time range (if provided)
            if (minTempoPreparo.HasValue)
            {
                query = query.Where(r => r.TempoPreparo >= minTempoPreparo.Value);
            }
            if (maxTempoPreparo.HasValue)
            {
                query = query.Where(r => r.TempoPreparo <= maxTempoPreparo.Value);
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Calculate pagination
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            // Apply pagination and execute query
            var receitas = await query
                .OrderBy(r => r.Nome)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Pass pagination and filter info to view
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.SearchNome = searchNome ?? string.Empty;
            ViewBag.MinTempoPreparo = minTempoPreparo;
            ViewBag.MaxTempoPreparo = maxTempoPreparo;

            return View(receitas);
        }

        // GET: Receita/Details/5
        public async Task<IActionResult> Details(int? id, string? clientId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receita = await _context.Receita
                .Include(r => r.Componentes)
                    .ThenInclude(c => c.Alimento)
                .FirstOrDefaultAsync(m => m.ReceitaId == id);
            
            if (receita == null)
            {
                return NotFound();
            }

            // Carregar componentes relacionados com detalhes do alimento
            var componentesDetalhados = receita.Componentes
                .Select(c => new {
                    c.ComponenteReceitaId,
                    AlimentoNome = c.Alimento!.Name,
                    c.Quantidade,
                    Unidade = c.UnidadeMedida.ToString(),
                    c.IsOpcional
                })
                .ToList<dynamic>();
            
            ViewBag.ComponentesDetalhados = componentesDetalhados;

            // Carregar restrições relacionadas (apenas para Nutricionista)
            if (User.IsInRole("Nutricionista") && receita.RestricoesAlimentarId != null && receita.RestricoesAlimentarId.Any())
            {
                var restricoesDetalhadas = await _context.RestricaoAlimentar
                    .Where(r => receita.RestricoesAlimentarId.Contains(r.RestricaoAlimentarId))
                    .Select(r => new
                    {
                        r.RestricaoAlimentarId,
                        r.Nome,
                        Tipo = r.Tipo.ToString(),
                        Gravidade = r.Gravidade.ToString(),
                        r.Descricao
                    })
                    .ToListAsync<dynamic>();

                ViewBag.RestricoesDetalhadas = restricoesDetalhadas;
            }
            else
            {
                ViewBag.RestricoesDetalhadas = null;
            }

            ReceitaAjustadaVm? adaptadaVm = null;
            string? avisoAdaptacao = null;

            // Cliente: versão adaptada do próprio utilizador, se a receita estiver no seu plano.
            if (User.IsInRole("Cliente"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrWhiteSpace(userId))
                {
                    var client = await _context.Client
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.IdentityUserId == userId);

                    if (client == null)
                    {
                        avisoAdaptacao = "Não foi possível localizar o seu perfil de cliente.";
                    }
                    else
                    {
                        var plano = await _context.PlanoAlimentar
                            .AsNoTracking()
                            .FirstOrDefaultAsync(p => p.ClientId == client.ClientId);

                        if (plano == null)
                        {
                            avisoAdaptacao = "Ainda não existe um plano alimentar associado ao seu perfil.";
                        }
                        else
                        {
                            var estaNoPlano = await _context.ReceitasParaPlanosAlimentares
                                .AsNoTracking()
                                .AnyAsync(x => x.PlanoAlimentarId == plano.PlanoAlimentarId && x.ReceitaId == receita.ReceitaId);

                            if (!estaNoPlano)
                            {
                                avisoAdaptacao = "A versão adaptada está disponível apenas para receitas associadas ao seu plano alimentar.";
                            }
                            else
                            {
                                var ajustado = await _ajusteService.GerarAjustesAsync(client.ClientId, plano.PlanoAlimentarId);
                                var receitaAjustada = ajustado.Receitas.FirstOrDefault(r => r.ReceitaId == receita.ReceitaId);
                                adaptadaVm = receitaAjustada?.ToVm();
                                avisoAdaptacao ??= ajustado.Aviso;
                            }
                        }
                    }
                }
            }

            // Nutricionista: permite escolher um cliente que tenha esta receita atribuída.
            var clientesDisponiveis = new List<ClienteOptionVm>();
            string? clientIdSelecionado = null;

            if (User.IsInRole("Nutricionista"))
            {
                var planosComReceita = await _context.PlanoAlimentar
                    .AsNoTracking()
                    .Include(p => p.Client)
                    .Where(p => p.ClientId != null)
                    .Where(p => _context.ReceitasParaPlanosAlimentares
                        .AsNoTracking()
                        .Any(rp => rp.PlanoAlimentarId == p.PlanoAlimentarId && rp.ReceitaId == receita.ReceitaId))
                    .OrderBy(p => p.Client!.Name)
                    .ToListAsync();

                clientesDisponiveis = planosComReceita
                    .Select(p => new ClienteOptionVm
                    {
                        ClientId = p.ClientId!,
                        PlanoAlimentarId = p.PlanoAlimentarId,
                        Display = !string.IsNullOrWhiteSpace(p.Client!.Email)
                            ? $"{p.Client.Name} ({p.Client.Email})"
                            : p.Client.Name
                    })
                    .ToList();

                if (!string.IsNullOrWhiteSpace(clientId))
                {
                    var escolhido = clientesDisponiveis.FirstOrDefault(c => c.ClientId == clientId);
                    if (escolhido == null)
                    {
                        avisoAdaptacao = "O cliente selecionado não tem esta receita atribuída.";
                    }
                    else
                    {
                        clientIdSelecionado = clientId;
                        var ajustado = await _ajusteService.GerarAjustesAsync(escolhido.ClientId, escolhido.PlanoAlimentarId);
                        var receitaAjustada = ajustado.Receitas.FirstOrDefault(r => r.ReceitaId == receita.ReceitaId);
                        adaptadaVm = receitaAjustada?.ToVm();
                        avisoAdaptacao ??= ajustado.Aviso;
                    }
                }
                else if (clientesDisponiveis.Count == 0)
                {
                    avisoAdaptacao = "Não existem clientes com esta receita atribuída.";
                }
            }

            var vm = new ReceitaComparacaoDetailsViewModel
            {
                Receita = receita,
                ReceitaAdaptada = adaptadaVm,
                AvisoAdaptacao = avisoAdaptacao,
                ClientIdSelecionado = clientIdSelecionado,
                ClientesDisponiveis = clientesDisponiveis
            };

            return View(vm);
        }

        // GET: Receita/Create
        [Authorize(Roles = "Nutricionista")]
        public IActionResult Create()
        {
            // Carregar restrições com tipo e gravidade
            var restricoes = _context.RestricaoAlimentar
                .Select(r => new {
                    r.RestricaoAlimentarId,
                    Display = $"{r.Nome} - {r.Tipo} ({r.Gravidade})"
                })
                .ToList();
           
            ViewData["RestricoesAlimentares"] = new MultiSelectList(restricoes, "RestricaoAlimentarId", "Display");

            ViewBag.Alimentos = new SelectList(_context.Alimentos.OrderBy(a => a.Name).ToList(), "AlimentoId", "Name");
            return View();
        }

        // POST: Receita/Create
        [Authorize(Roles = "Nutricionista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReceitaId,Nome,Descricao,ModoPreparo,TempoPreparo,Porcoes,Calorias,Proteinas,HidratosCarbono,Gorduras,RestricoesAlimentarId")] Receita receita, string? PendingComponentesJson)
        {
            if (ModelState.IsValid)
            {
                _context.Add(receita);
                await _context.SaveChangesAsync();

                if (!string.IsNullOrWhiteSpace(PendingComponentesJson))
                {
                    try
                    {
                        var comps = JsonSerializer.Deserialize<List<PendingCompDto>>(PendingComponentesJson);
                        if (comps != null && comps.Any())
                        {
                            var toAdd = comps.Select(pc => new ComponenteReceita
                            {
                                AlimentoId = pc.AlimentoId,
                                ReceitaId = receita.ReceitaId,
                                UnidadeMedida = Enum.Parse<UnidadeMedidaEnum>(pc.UnidadeMedida),
                                Quantidade = pc.Quantidade,
                                IsOpcional = pc.IsOpcional
                            }).ToList();

                            _context.ComponenteReceita.AddRange(toAdd);
                            await _context.SaveChangesAsync();
                        }
                    }
                    catch
                    {
                        // Não interrompe a criação da receita
                    }
                }

                TempData["AlertMessage"] = $"Receita '{receita.Nome}' criada com sucesso!";
                TempData["AlertType"] = "success";

                return RedirectToAction(nameof(Index));
            }

            var restricoes = _context.RestricaoAlimentar
                .Select(r => new
                {
                    r.RestricaoAlimentarId,
                    Display = $"{r.Nome} - {r.Tipo} ({r.Gravidade})"
                })
                .ToList();
            ViewData["RestricoesAlimentares"] = new MultiSelectList(restricoes, "RestricaoAlimentarId", "Display");
            ViewBag.Alimentos = new SelectList(_context.Alimentos.OrderBy(a => a.Name).ToList(), "AlimentoId", "Name");
            return View(receita);
        }

        private record PendingCompDto(int AlimentoId, string UnidadeMedida, int Quantidade, bool IsOpcional);

        [Authorize(Roles = "Nutricionista")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receita = await _context.Receita
                .Include(r => r.Componentes)
                    .ThenInclude(c => c.Alimento)
                .FirstOrDefaultAsync(r => r.ReceitaId == id);
            
            if (receita == null)
            {
                return NotFound();
            }

            if (_context.ComponenteReceita == null || _context.RestricaoAlimentar == null)
            {
                return Problem("Required data is missing in the database.");
            }
            
            // Carregar restrições com tipo e gravidade
            var restricoes = _context.RestricaoAlimentar
                .Select(r => new {
                    r.RestricaoAlimentarId,
                    Display = $"{r.Nome} - {r.Tipo} ({r.Gravidade})"
                })
                .ToList();
            ViewData["RestricoesAlimentares"] = new MultiSelectList(restricoes, "RestricaoAlimentarId", "Display", receita.RestricoesAlimentarId);
            return View(receita);
        }

        // POST: Receita/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Nutricionista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReceitaId,Nome,Descricao,ModoPreparo,TempoPreparo,Porcoes,Calorias,Proteinas,HidratosCarbono,Gorduras,RestricoesAlimentarId")] Receita receita)
        {
            if (id != receita.ReceitaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(receita);

                    await _context.SaveChangesAsync();

                    TempData["AlertMessage"] = $"Receita '{receita.Nome}' atualizada com sucesso!";
                    TempData["AlertType"] = "warning";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReceitaExists(receita.ReceitaId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(receita);
        }

        // GET: Receita/Delete/5
        [Authorize(Roles = "Nutricionista")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receita = await _context.Receita
                .Include(r => r.Componentes)
                    .ThenInclude(c => c.Alimento)
                .FirstOrDefaultAsync(m => m.ReceitaId == id);
            if (receita == null)
            {
                return NotFound();
            }

            return View(receita);
        }

        // POST: Receita/Delete/5
        [Authorize(Roles = "Nutricionista")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var receita = await _context.Receita.FindAsync(id);
            if (receita != null)
            {
                var receitaNome = receita.Nome;
                _context.Receita.Remove(receita);
                await _context.SaveChangesAsync();
                
                TempData["AlertMessage"] = $"Receita '{receitaNome}' apagada com sucesso!";
                TempData["AlertType"] = "danger";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ReceitaExists(int id)
        {
            return _context.Receita.Any(e => e.ReceitaId == id);
        }
    }
}
