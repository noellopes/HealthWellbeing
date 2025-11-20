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
    public class ExameTipoController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ExameTipoController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: ExameTipoes
        public async Task<IActionResult> Index(

            int page = 1,
            string searchNome = "",
            string searchEspecialidade = "")

        {
            // 1. Configuração da Pesquisa (para View e Links)
            ViewBag.SearchNome = searchNome;
            ViewBag.SearchEspecialidade = searchEspecialidade;

            // 2. Aplicação dos Filtros na Query
            var examesQuery = _context.ExameTipo.Include(et => et.Especialidade).AsQueryable();

            if (!string.IsNullOrEmpty(searchNome))
            {
                examesQuery = examesQuery.Where(et => et.Nome.Contains(searchNome));
            }
            if (!string.IsNullOrEmpty(searchEspecialidade))
            {
                examesQuery = examesQuery.Where(et => et.Especialidade.Nome.Contains(searchEspecialidade));
            }


            // 3. Contagem e Criação do ViewModel de Paginação
            int totalExames = await examesQuery.CountAsync();

            // Instanciar o ViewModel (ItemsPerPage=5 para forçar 2 páginas de teste)
            var examesInfo = new PaginationInfo<ExameTipo>(page, totalExames, itemsPerPage: 5);

            // 4. Ordenação e Aplicação da Paginação (Skip/Take)
            examesInfo.Items = await examesQuery
                .OrderBy(et => et.Nome)
                .Skip(examesInfo.ItemsToSkip) // Pula os itens das páginas anteriores
                .Take(examesInfo.ItemsPerPage) // Pega apenas os 5 itens da página atual
                .ToListAsync();

            // Retorna o ViewModel de Paginação
            return View(examesInfo);
        }

        // GET: ExameTipoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exameTipo = await _context.ExameTipo
                .Include(et => et.Especialidade) // ADICIONADO: Carrega a Especialidade
                .Include(et => et.ExameTipoRecursos!)      // Carrega a tabela intermédia
                    .ThenInclude(etr => etr.Recurso)       // Carrega o objeto MaterialEquipamentoAssociado
                .FirstOrDefaultAsync(m => m.ExameTipoId == id);
            if (exameTipo == null)
            {
                return NotFound();
            }

            return View(exameTipo);
        }

        // GET: ExameTipoes/Create
        public IActionResult Create()
        {
            ViewData["EspecialidadeId"] = new SelectList(_context.Especialidades.OrderBy(e => e.Nome), "EspecialidadeId", "Nome");
            return View();
        }

        // POST: ExameTipoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExameTipoId,Nome,Descricao,EspecialidadeId")] ExameTipo exameTipo)
        {
            // 1. Verificar se o nome já existe na base de dados
            var nomeJaExiste = await _context.ExameTipo
                .AnyAsync(et => et.Nome == exameTipo.Nome);

            if (nomeJaExiste)
            {
                // 2. Adicionar erro ao ModelState se o nome for repetido
                ModelState.AddModelError("Nome", "Já existe um Tipo de Exame com este nome. Por favor, escolha um nome único.");
            }
            if (ModelState.IsValid)
            {
                _context.Add(exameTipo);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"O tipo de exame '{exameTipo.Nome}' foi criado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["EspecialidadeId"] = new SelectList(_context.Especialidades, "EspecialidadeId", "Nome", exameTipo.EspecialidadeId);
            return View(exameTipo);
        }

        // GET: ExameTipoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exameTipo = await _context.ExameTipo
                .Include(et => et.Especialidade) // ADICIONADO: Carrega a Especialidade
                .Include(et => et.ExameTipoRecursos) // Carrega os recursos associados
                .FirstOrDefaultAsync(m => m.ExameTipoId == id);
            if (exameTipo == null)
            {
                return NotFound();
            }

            // 2. Carregar TODOS os materiais/equipamentos disponíveis na BD
            var todosMateriais = await _context.MaterialEquipamentoAssociado.ToListAsync();

            // 3. Criar o ViewModel e popular as checkboxes
            var viewModel = new TipoExameRecursosViewModel
            {
                ExameTipoId = exameTipo.ExameTipoId,
                NomeExame = exameTipo.Nome,
                Descricao = exameTipo.Descricao,       // <--- ADICIONADO
                EspecialidadeId = exameTipo.EspecialidadeId, // <--- ADICIONADO
                Recursos = todosMateriais.Select(m => new RecursoCheckBoxItem
                {
                    Id = m.MaterialEquipamentoAssociadoId,
                    Nome = m.NomeEquipamento,
                    // Verifica se este material já está na lista de recursos deste exame
                    IsSelected = exameTipo.ExameTipoRecursos!.Any(etr => etr.MaterialEquipamentoAssociadoId == m.MaterialEquipamentoAssociadoId)
                }).ToList()
            };

            ViewData["EspecialidadeId"] = new SelectList(_context.Especialidades, "EspecialidadeId", "Nome", exameTipo.EspecialidadeId);
            return View(viewModel);
        }

        // POST: ExameTipoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TipoExameRecursosViewModel viewModel)
        {
            if (id != viewModel.ExameTipoId)
            {
                return NotFound();
            }

            // 1. Validação de Unicidade (Nome)
            // Verifica se o novo nome já existe em OUTRO registo
            bool nomeJaExiste = await _context.ExameTipo.AnyAsync(
                et => et.Nome == viewModel.NomeExame && et.ExameTipoId != id);

            if (nomeJaExiste)
            {
                ModelState.AddModelError("NomeExame", "Já existe um tipo de exame com este nome.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 2. Carregar a Entidade da BD com as relações atuais (Crucial!)
                    // Precisamos do .Include para saber o que apagar e o que adicionar
                    var exameTipoToUpdate = await _context.ExameTipo
                        .Include(et => et.ExameTipoRecursos)
                        .FirstOrDefaultAsync(m => m.ExameTipoId == id);

                    if (exameTipoToUpdate == null) return NotFound();

                    // 3. Atualizar propriedades escalares (Dados normais)
                    exameTipoToUpdate.Nome = viewModel.NomeExame;
                    exameTipoToUpdate.Descricao = viewModel.Descricao;
                    exameTipoToUpdate.EspecialidadeId = viewModel.EspecialidadeId;

                    // 4. ATUALIZAR A RELAÇÃO M:N (A Lógica "Mágica")

                    //Identificar os IDs que o utilizador selecionou na View
                    var selectedIds = viewModel.Recursos
                        .Where(x => x.IsSelected)
                        .Select(x => x.Id)
                        .ToList();

                    //Identificar os IDs que JÁ existem na base de dados
                    var currentIds = exameTipoToUpdate.ExameTipoRecursos
                        .Select(r => r.MaterialEquipamentoAssociadoId)
                        .ToList();

                    //Adicionar novos (Selecionados mas não existentes na BD)
                    foreach (var selectedId in selectedIds)
                    {
                        if (!currentIds.Contains(selectedId))
                        {
                            // Cria o novo registo na tabela de junção
                            exameTipoToUpdate.ExameTipoRecursos.Add(new ExameTipoRecurso
                            {
                                ExameTipoId = id,
                                MaterialEquipamentoAssociadoId = selectedId
                            });
                        }
                    }

                    // Remover antigos (Existentes na BD mas não selecionados)
                    // Encontrar os objetos de junção para remover
                    var recursosParaRemover = exameTipoToUpdate.ExameTipoRecursos
                        .Where(r => !selectedIds.Contains(r.MaterialEquipamentoAssociadoId))
                        .ToList();

                    foreach (var recursoRemover in recursosParaRemover)
                    {
                        // Remove o registo da tabela de junção
                        _context.Remove(recursoRemover);
                    }

                    
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"O tipo de exame '{viewModel.NomeExame}' foi atualizado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExameTipoExists(viewModel.ExameTipoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // Se falhar (erro de validação), repopular a dropdown e retornar a View
            ViewData["EspecialidadeId"] = new SelectList(
                _context.Especialidades.OrderBy(e => e.Nome), "EspecialidadeId", "Nome", viewModel.EspecialidadeId);

            return View(viewModel);
        }

        // GET: ExameTipoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exameTipo = await _context.ExameTipo
                .Include(et => et.Especialidade) // ADICIONADO: Carrega a Especialidade
                .FirstOrDefaultAsync(m => m.ExameTipoId == id);
            if (exameTipo == null)
            {
                return NotFound();
            }

            return View(exameTipo);
        }

        // POST: ExameTipoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exameTipo = await _context.ExameTipo.FindAsync(id);
            if (exameTipo != null)
            {
               
                // Checar se existem exames (na tabela 'Exames') que referenciam este ExameTipoId
                int numExamesAssociados = await _context.Exames
                    .CountAsync(e => e.ExameTipoId == id); // Usa a FK ExameTipoId

                if (numExamesAssociados > 0)
                {
                    // mensagem de ERRO
                    TempData["ErrorMessage"] = $"Não é possível apagar o tipo de exame '{exameTipo.Nome}'. Existem {numExamesAssociados} exames registados que utilizam este tipo.";

                    // Retornamos à View Index sem apagar
                    return RedirectToAction(nameof(Index));
                }

                //Se não houver associações, proceder à exclusão
                _context.ExameTipo.Remove(exameTipo);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"O tipo de exame '{exameTipo.Nome}' foi apagado com sucesso!";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExameTipoExists(int id)
        {
            return _context.ExameTipo.Any(e => e.ExameTipoId == id);
        }
    }
}
