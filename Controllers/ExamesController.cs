using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using HealthWellBeing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Controllers
{
    [Authorize(Roles = "Admin,Gestor,Rececionista,Medico,Tecnico,Supervisor Tecnico,Utente")]
    public class ExamesController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ExamesController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // INDEX
        // ==========================================
        public async Task<IActionResult> Index(string pesquisaUtente, DateTime? pesquisaData, int pagina = 1)
        {
            var examesQuery = _context.Exames
                .Include(e => e.ExameTipo)
                .Include(e => e.Utente)
                .Include(e => e.ProfissionalExecutante)
                .Include(e => e.SalaDeExame)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(pesquisaUtente))
            {
                examesQuery = examesQuery.Where(e => e.Utente.Nome.Contains(pesquisaUtente));
            }

            if (pesquisaData.HasValue)
            {
                examesQuery = examesQuery.Where(e => e.DataHoraMarcacao.Date == pesquisaData.Value.Date);
            }

            int totalExames = await examesQuery.CountAsync();
            ViewBag.PesquisaUtente = pesquisaUtente;
            ViewBag.PesquisaData = pesquisaData;

            var paginationInfo = new PaginationInfo<Exame>(pagina, totalExames, 10);
            paginationInfo.Items = await examesQuery
                .OrderByDescending(e => e.DataHoraMarcacao)
                .Skip(paginationInfo.ItemsToSkip)
                .Take(paginationInfo.ItemsPerPage)
                .ToListAsync();

            return View(paginationInfo);
        }

        // ==========================================
        // DETAILS
        // ==========================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var exame = await _context.Exames
                .Include(e => e.ExameTipo)
                .Include(e => e.Utente)
                .Include(e => e.MedicoSolicitante)
                .Include(e => e.ProfissionalExecutante)
                .Include(e => e.SalaDeExame)
                .Include(e => e.RegistoMateriais)
                .FirstOrDefaultAsync(m => m.ExameId == id);

            if (exame == null) return NotFound();
            return View(exame);
        }

        // ==========================================
        // CREATE
        // ==========================================
        public async Task<IActionResult> Create(int? novoExameTipoId, int adicionarLinhas = 1)
        {
            var exame = new Exame { DataHoraMarcacao = DateTime.Now, RegistoMateriais = new List<RegistoMateriais>() };
            if (novoExameTipoId.HasValue) await CarregarReceitaEmMemoria(exame, novoExameTipoId.Value);

            ViewBag.LinhasAdicionais = adicionarLinhas;
            CarregarViewBagDropdowns(exame);
            return View(exame);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Exame exame, int[] selecionadosIds, int[] quantidades, int[] estadosIds, string[] nomesNovos, string[] tamanhosNovos, string actionType, int? novoExameTipoId, int adicionarLinhas = 1)
        {
            if (actionType == "Refresh")
            {
                await TratarRefresh(exame, selecionadosIds, novoExameTipoId);
                ViewBag.LinhasAdicionais = adicionarLinhas;
                CarregarViewBagDropdowns(exame);
                return View(exame);
            }

            if (ModelState.IsValid)
            {
                exame.MaterialEquipamentoAssociadoId = (selecionadosIds != null && selecionadosIds.Any(id => id > 0))
                    ? selecionadosIds.First(id => id > 0) : 1;

                _context.Exames.Add(exame);
                await _context.SaveChangesAsync();

                await ProcessarRegistoMateriais(exame.ExameId, selecionadosIds, quantidades, estadosIds, nomesNovos, tamanhosNovos, exame.Estado);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            CarregarViewBagDropdowns(exame);
            return View(exame);
        }

        // ==========================================
        // EDIT
        // ==========================================
        public async Task<IActionResult> Edit(int? id, int? novoExameTipoId, int adicionarLinhas = 1)
        {
            if (id == null) return NotFound();
            var exame = await _context.Exames.Include(e => e.RegistoMateriais).FirstOrDefaultAsync(e => e.ExameId == id);
            if (exame == null) return NotFound();

            if (novoExameTipoId.HasValue && novoExameTipoId != exame.ExameTipoId)
            {
                exame.RegistoMateriais.Clear();
                await CarregarReceitaEmMemoria(exame, novoExameTipoId.Value);
                exame.ExameTipoId = novoExameTipoId.Value;
            }

            ViewBag.LinhasAdicionais = adicionarLinhas;
            CarregarViewBagDropdowns(exame);
            return View(exame);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Exame exame, int[] selecionadosIds, int[] quantidades, int[] estadosIds, string[] nomesNovos, string[] tamanhosNovos, string actionType, int? novoExameTipoId, int adicionarLinhas = 1)
        {
            if (id != exame.ExameId) return NotFound();

            if (actionType == "Refresh")
            {
                await TratarRefresh(exame, selecionadosIds, novoExameTipoId);
                ViewBag.LinhasAdicionais = adicionarLinhas;
                CarregarViewBagDropdowns(exame);
                return View(exame);
            }

            if (ModelState.IsValid)
            {
                if (exame.MaterialEquipamentoAssociadoId == 0) exame.MaterialEquipamentoAssociadoId = 1;
                _context.Update(exame);

                var materiaisAtuais = _context.RegistoMateriais.Where(rm => rm.ExameId == id);
                _context.RegistoMateriais.RemoveRange(materiaisAtuais);
                await _context.SaveChangesAsync();

                await ProcessarRegistoMateriais(id, selecionadosIds, quantidades, estadosIds, nomesNovos, tamanhosNovos, exame.Estado);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            CarregarViewBagDropdowns(exame);
            return View(exame);
        }

        // ==========================================
        // DELETE
        // ==========================================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var exame = await _context.Exames
                .Include(e => e.Utente)
                .Include(e => e.ExameTipo)
                .Include(e => e.RegistoMateriais)
                .FirstOrDefaultAsync(m => m.ExameId == id);

            if (exame == null)
            {
                TempData["ErrorMessage"] = "Este exame já não existe ou já foi eliminado.";
                return RedirectToAction(nameof(Index));
            }

            // NOVA VALIDAÇÃO: Bloquear acesso à página de confirmação se já estiver realizado
            if (exame.Estado == "Realizado")
            {
                TempData["ErrorMessage"] = "Não é possível eliminar um exame que já foi marcado como 'Realizado'.";
                return RedirectToAction(nameof(Index));
            }

            return View(exame);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exame = await _context.Exames.FindAsync(id);

            if (exame == null)
            {
                TempData["ErrorMessage"] = "Erro: O exame que tenta eliminar já não existe no sistema.";
                return RedirectToAction(nameof(Index));
            }

            // NOVA VALIDAÇÃO DE SEGURANÇA NO POST: Impede a eliminação via requisição direta se realizado
            if (exame.Estado == "Realizado")
            {
                TempData["ErrorMessage"] = "Operação negada: Exames com estado 'Realizado' não podem ser removidos.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var materiais = _context.RegistoMateriais.Where(rm => rm.ExameId == id);
                _context.RegistoMateriais.RemoveRange(materiais);

                _context.Exames.Remove(exame);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Exame eliminado com sucesso.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Ocorreu um erro ao tentar eliminar o exame.";
            }

            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // MÉTODOS AUXILIARES
        // ==========================================

        private async Task TratarRefresh(Exame exame, int[] selecionadosIds, int? novoExameTipoId)
        {
            exame.RegistoMateriais = new List<RegistoMateriais>();
            if (novoExameTipoId.HasValue)
                await CarregarReceitaEmMemoria(exame, novoExameTipoId.Value);
            else if (selecionadosIds != null)
            {
                foreach (var sId in selecionadosIds.Where(id => id > 0))
                {
                    var mat = await _context.MaterialEquipamentoAssociado.FindAsync(sId);
                    if (mat != null) exame.RegistoMateriais.Add(new RegistoMateriais { Nome = mat.NomeEquipamento, Tamanho = mat.Tamanho });
                }
            }
        }

        private async Task CarregarReceitaEmMemoria(Exame exame, int tipoId)
        {
            var receita = await _context.ExameTipoRecursos.Where(x => x.ExameTipoId == tipoId).Include(x => x.Recurso).ToListAsync();
            foreach (var r in receita)
                exame.RegistoMateriais.Add(new RegistoMateriais
                {
                    Nome = r.Recurso.NomeEquipamento,
                    Tamanho = r.Recurso.Tamanho,
                    Quantidade = r.QuantidadeNecessaria,
                    MaterialStatusId = 1 // ID 1 = Reservado
                });
        }

        private async Task ProcessarRegistoMateriais(int exameId, int[] selecionadosIds, int[] quantidades, int[] estadosIds, string[] nomesNovos, string[] tamanhosNovos, string estadoExame)
        {
            int ID_RESERVADO = 1; //

            // Mapeamento automático baseado no estado do exame
            int estadoAuto = estadoExame switch
            {
                "Marcado" => 1,    // Reservado
                "Remarcado" => 1,  // Reservado
                "Cancelado" => 2,  // Cancelado
                "Realizado" => 4,  // Usado
                _ => 1
            };

            int idxNovo = 0;
            if (selecionadosIds == null) return;

            for (int i = 0; i < selecionadosIds.Length; i++)
            {
                string nome = ""; string tam = "";
                if (selecionadosIds[i] == 0)
                {
                    if (nomesNovos != null && idxNovo < nomesNovos.Length && !string.IsNullOrWhiteSpace(nomesNovos[idxNovo]))
                    {
                        var nMat = new MaterialEquipamentoAssociado { NomeEquipamento = nomesNovos[idxNovo], Tamanho = tamanhosNovos?[idxNovo] };
                        _context.MaterialEquipamentoAssociado.Add(nMat); await _context.SaveChangesAsync();
                        nome = nMat.NomeEquipamento; tam = nMat.Tamanho; idxNovo++;
                    }
                    else { idxNovo++; continue; }
                }
                else
                {
                    var m = await _context.MaterialEquipamentoAssociado.FindAsync(selecionadosIds[i]);
                    if (m == null) continue;
                    nome = m.NomeEquipamento; tam = m.Tamanho;
                }

                // Se o ID for o padrão antigo ou o novo 'Reservado', deixamos o estadoAuto assumir o controle
                int estFinal = (estadosIds != null && i < estadosIds.Length && estadosIds[i] != 9 && estadosIds[i] != ID_RESERVADO)
                                ? estadosIds[i] : estadoAuto;

                _context.RegistoMateriais.Add(new RegistoMateriais
                {
                    ExameId = exameId,
                    Nome = nome,
                    Tamanho = tam,
                    Quantidade = (quantidades != null && i < quantidades.Length) ? quantidades[i] : 1,
                    MaterialStatusId = estFinal
                });
            }
        }

        private void CarregarViewBagDropdowns(Exame exame = null)
        {
            ViewData["ExameTipoId"] = new SelectList(_context.ExameTipo.OrderBy(e => e.Nome), "ExameTipoId", "Nome", exame?.ExameTipoId);
            ViewData["UtenteId"] = new SelectList(_context.Utentes.OrderBy(u => u.Nome), "UtenteId", "Nome", exame?.UtenteId);
            ViewData["MedicoSolicitanteId"] = new SelectList(_context.Medicos.OrderBy(m => m.Nome), "Id", "Nome", exame?.MedicoSolicitanteId);
            ViewData["ProfissionalExecutanteId"] = new SelectList(_context.ProfissionalExecutante.OrderBy(p => p.Nome), "ProfissionalExecutanteId", "Nome", exame?.ProfissionalExecutanteId);
            ViewData["SalaDeExameId"] = new SelectList(_context.SalaDeExame.OrderBy(s => s.TipoSala), "SalaId", "TipoSala", exame?.SalaDeExameId);
            ViewBag.DicionarioCompleto = _context.MaterialEquipamentoAssociado.OrderBy(m => m.NomeEquipamento).ToList();
            ViewBag.ListaEstadosMateriais = new SelectList(_context.Set<EstadoMaterial>().OrderBy(e => e.Nome), "MaterialStatusId", "Nome");
        }
    }
}