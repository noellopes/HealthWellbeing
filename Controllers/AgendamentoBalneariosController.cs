using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellbeing.Controllers
{
    public class AgendamentoBalnearioController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public AgendamentoBalnearioController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: AgendamentoBalnearios
        public async Task<IActionResult> Index(string pesquisarNomeUtente, int pagina = 1)
        {
            int itensPorPagina = 5;

            var query = _context.Agendamentos
    .Include(a => a.UtenteBalneario) 
    .Include(a => a.Terapeuta)
    .Include(a => a.Servico)
    .Include(a => a.TipoServico)
    .AsQueryable();

            if (!string.IsNullOrEmpty(pesquisarNomeUtente))
            {
                
                query = query.Where(a => a.UtenteBalneario != null &&
                                    a.UtenteBalneario.NomeCompleto.Contains(pesquisarNomeUtente));
            }

            int totalItens = await query.CountAsync();

            var agendamentos = await query
                .OrderByDescending(a => a.HoraInicio)
                .Skip((pagina - 1) * itensPorPagina)
                .Take(itensPorPagina)
                .ToListAsync();

            var viewModel = new AgendamentoBalnearioViewModel
            {
                ListaAgendamentos = agendamentos,
                PesquisarNomeUtente = pesquisarNomeUtente,
                paginacao = new Paginacao(
                    totalRegistos: totalItens,
                    pagina: pagina,
                    pageSize: itensPorPagina
                )
            };

            return View(viewModel);
        }

        // GET: AgendamentoBalnearios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var agendamentoBalneario = await _context.Agendamentos
                .Include(a => a.Servico)
                .Include(a => a.Terapeuta)
                .Include(a => a.TipoServico)
                .Include(a => a.UtenteBalneario)
                .FirstOrDefaultAsync(m => m.AgendamentoId == id);
            if (agendamentoBalneario == null)
            {
                return NotFound();
            }

            return View(agendamentoBalneario);
        }
        // GET: AgendamentoBalnearios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("AgendamentoId,UtenteBalnearioId,TerapeutaId,ServicoId,TipoServicosId,HoraInicio,DuracaoMinutos,Preco,Descricao,Estado")]
            AgendamentoBalneario agendamentoBalneario)
        {
            ModelState.Remove("UtenteBalneario");
            ModelState.Remove("Servico");
            ModelState.Remove("Terapeuta");
            ModelState.Remove("TipoServico");

            if (ModelState.IsValid)
            {
                _context.Add(agendamentoBalneario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index),
                      new { id = agendamentoBalneario.AgendamentoId, successMessage = "Agendamento criado com sucesso!" });
            }

            // Recarregar listas se houver erro
            ViewBag.UtenteBalnearioId =
                new SelectList(_context.Utentes, "UtenteBalnearioId", "NomeCompleto", agendamentoBalneario.UtenteBalnearioId);

            ViewBag.TerapeutaId =
                new SelectList(_context.Terapeuta, "TerapeutaId", "Email", agendamentoBalneario.TerapeutaId);

            ViewBag.ServicoId =
                new SelectList(_context.Servicos, "ServicoId", "Nome", agendamentoBalneario.ServicoId);

            ViewBag.TipoServicosId =
                new SelectList(_context.TipoServicos, "TipoServicosId", "Nome", agendamentoBalneario.TipoServicosId);

            return View(agendamentoBalneario);
        }

// GET: AgendamentoBalnearios/Edit/5
public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var agendamentoBalneario = await _context.Agendamentos.FindAsync(id);
            if (agendamentoBalneario == null)
            {
                return NotFound();
            }
            ViewData["ServicoId"] = new SelectList(_context.Servicos.OrderBy(s => s.Nome), "ServicoId", "Nome", agendamentoBalneario.ServicoId);
            ViewData["TerapeutaId"] = new SelectList(_context.Terapeuta.OrderBy(t => t.Email), "TerapeutaId", "Email", agendamentoBalneario.TerapeutaId);
            ViewData["TipoServicosId"] = new SelectList(_context.TipoServicos.OrderBy(t => t.Nome), "TipoServicosId", "Nome", agendamentoBalneario.TipoServicosId);
            ViewData["UtenteBalnearioId"] = new SelectList(_context.Utentes.OrderBy(u => u.NomeCompleto), "UtenteBalnearioId", "NomeCompleto", agendamentoBalneario.UtenteBalnearioId);
            return View(agendamentoBalneario);
        }

        // POST: AgendamentoBalnearios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AgendamentoId,UtenteBalnearioId,TerapeutaId,ServicoId,TipoServicosId,HoraInicio,DuracaoMinutos,Preco,Descricao,Estado")] AgendamentoBalneario agendamentoBalneario)
        {
            if (id != agendamentoBalneario.AgendamentoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(agendamentoBalneario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AgendamentoBalnearioExists(agendamentoBalneario.AgendamentoId))
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
                    id = agendamentoBalneario.AgendamentoId,
                    successMessage = "Dados do Agendamento alterado com sucesso!"
                });
            }
            ViewData["ServicoId"] = new SelectList(_context.Servicos.OrderBy(s => s.Nome), "ServicoId", "Nome", agendamentoBalneario.ServicoId);
            ViewData["TerapeutaId"] = new SelectList(_context.Terapeuta, "TerapeutaId", "Email", agendamentoBalneario.TerapeutaId);
            ViewData["TipoServicoId"] = new SelectList(_context.TipoServicos, "TipoServicosId", "Nome", agendamentoBalneario.TipoServicosId);
            ViewData["UtenteBalnearioId"] = new SelectList(_context.Utentes, "UtenteBalnearioId", "NomeCompleto", agendamentoBalneario.UtenteBalnearioId);
            return View(agendamentoBalneario);
        }

        // GET: AgendamentoBalnearios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var agendamentoBalneario = await _context.Agendamentos
                .Include(a => a.Servico)
                .Include(a => a.Terapeuta)
                .Include(a => a.TipoServico)
                .Include(a => a.UtenteBalneario)
                .FirstOrDefaultAsync(m => m.AgendamentoId == id);
            if (agendamentoBalneario == null)
            {
                return NotFound();
            }

            return View(agendamentoBalneario);
        }

        // POST: AgendamentoBalnearios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var agendamentoBalneario = await _context.Agendamentos.FindAsync(id);
            if (agendamentoBalneario != null)
            {
                _context.Agendamentos.Remove(agendamentoBalneario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { successMessage = "Agendamento eliminado com sucesso!" });
        }


        [HttpGet]
        public async Task<IActionResult> GetDadosServico(int servicoId) 
        {
            try
            {
                var servico = await _context.Servicos
                    .FirstOrDefaultAsync(s => s.ServicoId == servicoId);

                if (servico == null) return NotFound();

                return Json(new
                {
                    preco = servico.Preco,
                    duracao = servico.DuracaoMinutos,
                    tipoServicoId = servico.TipoServicosId 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        public async Task<JsonResult> GetTerapeutasPorTipo(int? tipoServicoId)
        {
            // Devolvemos todos os terapeutas ativos sem olhar para a especialidade técnica
            var terapeutas = await _context.Terapeuta
                .Where(t => t.Ativo)
                .Select(t => new {
                    id = t.TerapeutaId,
                    nome = t.Nome
                })
                .ToListAsync();

            return Json(terapeutas);
        }

        public async Task<IActionResult> Agenda()
        {
            // Procura todos os agendamentos incluindo as relações para mostrar nomes em vez de IDs
            var agendamentoBalneario = await _context.Agendamentos
                 .Include(a => a.UtenteBalneario)
                .Include(a => a.Terapeuta)
                .Include(a => a.Servico)
                .Include(a => a.TipoServico)
                .OrderBy(a => a.HoraInicio) // Ordena por hora para facilitar ao terapeuta
                .ToListAsync();

            var viewModel = new AgendamentoBalnearioViewModel
            {
                ListaAgendamentos = agendamentoBalneario.ToList(),
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var agendamento = await _context.Agendamentos.FindAsync(id);
            if (agendamento == null) return NotFound();

            if (Enum.TryParse<EstadoAgendamento>(status, out var novoEstado))
            {
                agendamento.Estado = novoEstado;
                _context.Update(agendamento);
                await _context.SaveChangesAsync();

                // Redireciona para o Index onde a tabela já tem as cores configuradas
                return RedirectToAction(nameof(Index), new { successMessage = $"Agendamento marcado como {status}!" });
            }
            return RedirectToAction(nameof(Agenda));
        }
        private bool AgendamentoBalnearioExists(int id)
        {
            return _context.Agendamentos.Any(e => e.AgendamentoId == id);
        }
    }
}
