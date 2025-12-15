using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellBeing.Models;
using HealthWellbeing.Data;
using HealthWellbeing.ViewModels; // Necessário para a Paginação

namespace HealthWellbeing.Controllers
{
    public class ExamesController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ExamesController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Exames
        // Corrigido para suportar Paginação e Pesquisa, correspondendo ao modelo da View Index 
        public async Task<IActionResult> Index(int pagina = 1, string pesquisaUtente = "", string pesquisaTipo = "")
        {
            // 1. Guardar os termos de pesquisa para a View não os perder
            ViewBag.PesquisaUtente = pesquisaUtente;
            ViewBag.PesquisaTipo = pesquisaTipo;

            // 2. Preparar a Query com os Includes necessários (Eager Loading)
            var examesQuery = _context.Exames
                .Include(e => e.ExameTipo)
                .Include(e => e.MaterialEquipamentoAssociado)
                .Include(e => e.MedicoSolicitante)
                .Include(e => e.ProfissionalExecutante)
                .Include(e => e.SalaDeExame)
                .Include(e => e.Utente)
                .AsQueryable();

            // 3. Aplicar Filtros de Pesquisa
            if (!string.IsNullOrEmpty(pesquisaUtente))
            {
                examesQuery = examesQuery.Where(e => e.Utente.Nome.Contains(pesquisaUtente));
            }

            if (!string.IsNullOrEmpty(pesquisaTipo))
            {
                examesQuery = examesQuery.Where(e => e.ExameTipo.Nome.Contains(pesquisaTipo));
            }

            // 4. Configurar Paginação
            int totalExames = await examesQuery.CountAsync();
          // Cria o objeto PaginationInfo que a View Index espera 
            var paginationInfo = new PaginationInfo<Exame>(pagina, totalExames, itemsPerPage: 5);

            // 5. Obter os dados da página atual
            paginationInfo.Items = await examesQuery
                .OrderByDescending(e => e.DataHoraMarcacao) // Ordenar por data (mais recente primeiro)
                .Skip(paginationInfo.ItemsToSkip)
                .Take(paginationInfo.ItemsPerPage)
                .ToListAsync();

            // Retorna o objeto correto (PaginationInfo) em vez de uma Lista
            return View(paginationInfo);
        }

        // GET: Exames/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var exame = await _context.Exames
                .Include(e => e.ExameTipo)
                .Include(e => e.MaterialEquipamentoAssociado)
                .Include(e => e.MedicoSolicitante)
                .Include(e => e.ProfissionalExecutante)
                .Include(e => e.SalaDeExame)
                .Include(e => e.Utente)
                .FirstOrDefaultAsync(m => m.ExameId == id);

            if (exame == null) return NotFound();

            return View(exame);
        }

        // GET: Exames/Create
        public IActionResult Create()
        {
            CarregarViewBagDropdowns();
            return View();
        }

        // POST: Exames/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExameId,DataHoraMarcacao,Estado,Notas,UtenteId,ExameTipoId,MedicoSolicitanteId,ProfissionalExecutanteId,SalaDeExameId,MaterialEquipamentoAssociadoId")] Exame exame)
        {
            // Validação de Negócio: Verificar se a sala já está ocupada naquele horário
            bool salaOcupada = await _context.Exames.AnyAsync(e =>
                e.SalaDeExameId == exame.SalaDeExameId &&
                e.DataHoraMarcacao == exame.DataHoraMarcacao);

            if (salaOcupada)
            {
                ModelState.AddModelError("DataHoraMarcacao", "A sala selecionada já está ocupada neste horário.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(exame);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Exame marcado com sucesso!";
                return RedirectToAction(nameof(Index));
            }

            // Se houver erro, recarrega as listas
            CarregarViewBagDropdowns(exame);
            return View(exame);
        }

        // GET: Exames/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var exame = await _context.Exames.FindAsync(id);
            if (exame == null) return NotFound();

            CarregarViewBagDropdowns(exame);
            return View(exame);
        }

        // POST: Exames/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExameId,DataHoraMarcacao,Estado,Notas,UtenteId,ExameTipoId,MedicoSolicitanteId,ProfissionalExecutanteId,SalaDeExameId,MaterialEquipamentoAssociadoId")] Exame exame)
        {
            if (id != exame.ExameId) return NotFound();

            // Validação de Sala Ocupada (ignorando o próprio exame que está a ser editado)
            bool salaOcupadaOutro = await _context.Exames.AnyAsync(e =>
                e.SalaDeExameId == exame.SalaDeExameId &&
                e.DataHoraMarcacao == exame.DataHoraMarcacao &&
                e.ExameId != id);

            if (salaOcupadaOutro)
            {
                ModelState.AddModelError("DataHoraMarcacao", "A sala selecionada já está ocupada neste horário.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(exame);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Exame atualizado com sucesso!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExameExists(exame.ExameId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            CarregarViewBagDropdowns(exame);
            return View(exame);
        }

        // GET: Exames/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var exame = await _context.Exames
                .Include(e => e.ExameTipo)
                .Include(e => e.MaterialEquipamentoAssociado)
                .Include(e => e.MedicoSolicitante)
                .Include(e => e.ProfissionalExecutante)
                .Include(e => e.SalaDeExame)
                .Include(e => e.Utente)
                .FirstOrDefaultAsync(m => m.ExameId == id);

            if (exame == null) return NotFound();

            return View(exame);
        }

        // POST: Exames/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int ExameTipoId) // O nome do parametro aqui vem do form delete, mas usamos o ID da rota geralmente
        {
            // Nota: O seu form Delete.cshtml envia "ExameTipoId"[cite: 58], mas o controller espera o ID do exame. 
                        // Vou assumir que o ID vem da rota (asp-route-id).
            var id = ExameTipoId;
            // Se o Id vier da URL, usamos RouteValues. Mas o código scaffold usa o parametro do metodo.

            // Correção para apanhar o ID correto vindo da view
            if (RouteData.Values["id"] != null)
            {
                id = int.Parse(RouteData.Values["id"].ToString());
            }

            var exame = await _context.Exames.FindAsync(id);
            if (exame != null)
            {
                // Impedir apagar exames realizados
                if (exame.Estado == "Realizado")
                {
                    TempData["ErrorMessage"] = "Não é possível apagar um exame já realizado.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Exames.Remove(exame);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Exame eliminado com sucesso!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ExameExists(int id)
        {
            return _context.Exames.Any(e => e.ExameId == id);
        }

        // Método Auxiliar para evitar repetição de código no Create e Edit
        private void CarregarViewBagDropdowns(Exame exame = null)
        {
           // Usa "ViewData" porque é o que está nas suas Views [cite: 7, 44]
            ViewData["ExameTipoId"] = new SelectList(_context.ExameTipo, "ExameTipoId", "Nome", exame?.ExameTipoId);

            // Atenção: Usei "NomeEquipamento" pois é o que está no seu código anterior
            ViewData["MaterialEquipamentoAssociadoId"] = new SelectList(_context.MaterialEquipamentoAssociado, "MaterialEquipamentoAssociadoId", "NomeEquipamento", exame?.MaterialEquipamentoAssociadoId);

            ViewData["MedicoSolicitanteId"] = new SelectList(_context.Medicos, "Id", "Nome", exame?.MedicoSolicitanteId);
            ViewData["ProfissionalExecutanteId"] = new SelectList(_context.ProfissionalExecutante, "ProfissionalExecutanteId", "Nome", exame?.ProfissionalExecutanteId); // Mudei para Nome para ser mais legível
            ViewData["SalaDeExameId"] = new SelectList(_context.SalaDeExame, "SalaId", "TipoSala", exame?.SalaDeExameId);
            ViewData["UtenteId"] = new SelectList(_context.Utentes, "UtenteId", "Nome", exame?.UtenteId); // Mudei para Nome
        }
    }
}