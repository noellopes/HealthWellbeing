using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellbeing.Controllers
{
    [Authorize(Roles = "Admin, Gestor")]
    public class ExameTipoController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ExameTipoController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: ExameTipoes
        public async Task<IActionResult> Index(int page = 1, string searchNome = "", string searchEspecialidade = "")
        {
            ViewBag.SearchNome = searchNome;
            ViewBag.SearchEspecialidade = searchEspecialidade;

            var examesQuery = _context.ExameTipo
                .Include(et => et.Especialidade)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchNome))
            {
                examesQuery = examesQuery.Where(et => et.Nome.Contains(searchNome));
            }
            if (!string.IsNullOrEmpty(searchEspecialidade))
            {
                examesQuery = examesQuery.Where(et => et.Especialidade.Nome.Contains(searchEspecialidade));
            }

            ViewBag.TotalExames = await _context.ExameTipo.CountAsync();
            ViewBag.TotalEspecialidades = await _context.Especialidades.CountAsync();

            int totalExames = await examesQuery.CountAsync();
            var examesInfo = new PaginationInfo<ExameTipo>(page, totalExames, itemsPerPage: 5);

            examesInfo.Items = await examesQuery
                .OrderBy(et => et.Nome)
                .Skip(examesInfo.ItemsToSkip)
                .Take(examesInfo.ItemsPerPage)
                .ToListAsync();

            return View(examesInfo);
        }

        // GET: ExameTipoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var exameTipo = await _context.ExameTipo
                .Include(et => et.Especialidade)
                .Include(et => et.ExameTipoRecursos!)
                    .ThenInclude(etr => etr.Recurso)
                .FirstOrDefaultAsync(m => m.ExameTipoId == id);

            if (exameTipo == null) return NotFound();

            return View(exameTipo);
        }

        // GET: ExameTipoes/Create
        public async Task<IActionResult> Create()
        {
            var todosMateriais = await _context.MaterialEquipamentoAssociado
                .OrderBy(m => m.NomeEquipamento)
                .ToListAsync();

            var viewModel = new TipoExameRecursosViewModel
            {
                Recursos = todosMateriais.Select(m => new RecursoCheckBoxItem
                {
                    Id = m.MaterialEquipamentoAssociadoId,
                    Nome = m.NomeEquipamento,
                    Tamanho = m.Tamanho, // <--- ADICIONADO: Para aparecer na lista
                    IsSelected = false,
                    Quantidade = 1,
                }).ToList()
            };

            // Inicializa ViewData para os inputs do novo material
            ViewData["NovoMaterialNome"] = "";
            ViewData["NovoMaterialTamanho"] = "";
            ViewData["NovoMaterialQtd"] = 0;

            ViewData["EspecialidadeId"] = new SelectList(_context.Especialidades.OrderBy(e => e.Nome), "EspecialidadeId", "Nome");
            return View(viewModel);
        }

        // POST: ExameTipoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            TipoExameRecursosViewModel viewModel,
            string submitAction,
            string? novoMaterialNome,
            string? novoMaterialTamanho,
            int? novoMaterialQuantidade
        )
        {
            // --- LÓGICA 1: ADICIONAR NOVO MATERIAL (Sem sair da página) ---
            if (submitAction == "add_material")
            {
                bool dadosValidos = true;

                if (string.IsNullOrWhiteSpace(novoMaterialNome))
                {
                    ModelState.AddModelError("", "Erro: O Nome do material é obrigatório.");
                    dadosValidos = false;
                }
                if (string.IsNullOrWhiteSpace(novoMaterialTamanho))
                {
                    ModelState.AddModelError("", "Erro: O Tamanho é obrigatório (ex: 'S', '10ml', 'Único').");
                    dadosValidos = false;
                }
                if (novoMaterialQuantidade == null || novoMaterialQuantidade < 0)
                {
                    ModelState.AddModelError("", "Erro: O Stock inicial deve ser válido.");
                    dadosValidos = false;
                }

                if (dadosValidos)
                {
                    // Verifica duplicados (Nome + Tamanho)
                    bool existe = await _context.MaterialEquipamentoAssociado
                        .AnyAsync(m => m.NomeEquipamento == novoMaterialNome && m.Tamanho == novoMaterialTamanho);

                    if (!existe)
                    {
                        var novoMat = new MaterialEquipamentoAssociado
                        {
                            NomeEquipamento = novoMaterialNome!,
                            Tamanho = novoMaterialTamanho!,
                            Quantidade = novoMaterialQuantidade!.Value
                        };
                        _context.Add(novoMat);
                        await _context.SaveChangesAsync();

                        // Limpa inputs após sucesso
                        ModelState.Remove("novoMaterialNome");
                        ModelState.Remove("novoMaterialTamanho");
                        ModelState.Remove("novoMaterialQuantidade");
                        ViewData["NovoMaterialNome"] = "";
                        ViewData["NovoMaterialTamanho"] = "";
                        ViewData["NovoMaterialQtd"] = 0;
                        ViewData["FeedbackMaterial"] = "Material adicionado com sucesso!";
                    }
                    else
                    {
                        ModelState.AddModelError("", $"Já existe '{novoMaterialNome}' com tamanho '{novoMaterialTamanho}'.");
                        ViewData["NovoMaterialNome"] = novoMaterialNome;
                        ViewData["NovoMaterialTamanho"] = novoMaterialTamanho;
                        ViewData["NovoMaterialQtd"] = novoMaterialQuantidade;
                    }
                }
                else
                {
                    ViewData["NovoMaterialNome"] = novoMaterialNome;
                    ViewData["NovoMaterialTamanho"] = novoMaterialTamanho;
                    ViewData["NovoMaterialQtd"] = novoMaterialQuantidade;
                }

                viewModel = await ReconstruirViewModel(viewModel, novoMaterialNome);
                ViewData["EspecialidadeId"] = new SelectList(_context.Especialidades.OrderBy(e => e.Nome), "EspecialidadeId", "Nome", viewModel.EspecialidadeId);
                return View(viewModel);
            }

            // --- LÓGICA 2: GRAVAR EXAME (Fluxo Normal) ---
            if (await _context.ExameTipo.AnyAsync(et => et.Nome == viewModel.NomeExame))
            {
                ModelState.AddModelError("NomeExame", "Já existe um exame com este nome.");
            }

            if (ModelState.IsValid)
            {
                var novoExame = new ExameTipo
                {
                    Nome = viewModel.NomeExame,
                    Descricao = viewModel.Descricao,
                    EspecialidadeId = viewModel.EspecialidadeId,
                    ExameTipoRecursos = new List<ExameTipoRecurso>()
                };

                foreach (var item in viewModel.Recursos.Where(r => r.IsSelected))
                {
                    novoExame.ExameTipoRecursos.Add(new ExameTipoRecurso
                    {
                        MaterialEquipamentoAssociadoId = item.Id,
                        QuantidadeNecessaria = item.Quantidade
                    });
                }

                _context.Add(novoExame);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tipo de exame criado com sucesso!";
                return RedirectToAction(nameof(Index));
            }

            viewModel = await ReconstruirViewModel(viewModel, null);
            ViewData["NovoMaterialNome"] = novoMaterialNome ?? "";
            ViewData["NovoMaterialTamanho"] = novoMaterialTamanho ?? "";
            ViewData["NovoMaterialQtd"] = novoMaterialQuantidade ?? 0;
            ViewData["EspecialidadeId"] = new SelectList(_context.Especialidades.OrderBy(e => e.Nome), "EspecialidadeId", "Nome", viewModel.EspecialidadeId);
            return View(viewModel);
        }

        // GET: ExameTipoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var exameTipo = await _context.ExameTipo
                .Include(et => et.ExameTipoRecursos)
                .FirstOrDefaultAsync(m => m.ExameTipoId == id);

            if (exameTipo == null) return NotFound();

            var todosMateriais = await _context.MaterialEquipamentoAssociado
                .OrderBy(m => m.NomeEquipamento)
                .ToListAsync();

            var viewModel = new TipoExameRecursosViewModel
            {
                ExameTipoId = exameTipo.ExameTipoId,
                NomeExame = exameTipo.Nome,
                Descricao = exameTipo.Descricao,
                EspecialidadeId = exameTipo.EspecialidadeId,
                Recursos = todosMateriais.Select(m =>
                {
                    var ligacao = exameTipo.ExameTipoRecursos!
                        .FirstOrDefault(r => r.MaterialEquipamentoAssociadoId == m.MaterialEquipamentoAssociadoId);

                    return new RecursoCheckBoxItem
                    {
                        Id = m.MaterialEquipamentoAssociadoId,
                        Nome = m.NomeEquipamento,
                        Tamanho = m.Tamanho, // <--- ADICIONADO
                        IsSelected = ligacao != null,
                        Quantidade = ligacao?.QuantidadeNecessaria ?? 1,
                    };
                }).ToList()
            };

            ViewData["NovoMaterialNome"] = "";
            ViewData["NovoMaterialTamanho"] = "";
            ViewData["NovoMaterialQtd"] = 0;
            ViewData["EspecialidadeId"] = new SelectList(_context.Especialidades, "EspecialidadeId", "Nome", exameTipo.EspecialidadeId);
            return View(viewModel);
        }

        // POST: ExameTipoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            TipoExameRecursosViewModel viewModel,
            string submitAction,
            string? novoMaterialNome,
            string? novoMaterialTamanho,
            int? novoMaterialQuantidade
        )
        {
            if (id != viewModel.ExameTipoId) return NotFound();

            // --- LÓGICA 1: ADICIONAR NOVO MATERIAL NO EDIT ---
            if (submitAction == "add_material")
            {
                bool dadosValidos = true;

                if (string.IsNullOrWhiteSpace(novoMaterialNome))
                {
                    ModelState.AddModelError("", "Erro: Nome obrigatório.");
                    dadosValidos = false;
                }
                if (string.IsNullOrWhiteSpace(novoMaterialTamanho))
                {
                    ModelState.AddModelError("", "Erro: Tamanho obrigatório.");
                    dadosValidos = false;
                }
                if (novoMaterialQuantidade == null || novoMaterialQuantidade < 0)
                {
                    ModelState.AddModelError("", "Erro: Stock inválido.");
                    dadosValidos = false;
                }

                if (dadosValidos)
                {
                    bool existe = await _context.MaterialEquipamentoAssociado
                        .AnyAsync(m => m.NomeEquipamento == novoMaterialNome && m.Tamanho == novoMaterialTamanho);

                    if (!existe)
                    {
                        var novoMat = new MaterialEquipamentoAssociado
                        {
                            NomeEquipamento = novoMaterialNome!,
                            Tamanho = novoMaterialTamanho!,
                            Quantidade = novoMaterialQuantidade!.Value
                        };
                        _context.Add(novoMat);
                        await _context.SaveChangesAsync();

                        ModelState.Remove("novoMaterialNome");
                        ModelState.Remove("novoMaterialTamanho");
                        ModelState.Remove("novoMaterialQuantidade");
                        ViewData["NovoMaterialNome"] = "";
                        ViewData["NovoMaterialTamanho"] = "";
                        ViewData["NovoMaterialQtd"] = 0;
                        ViewData["FeedbackMaterial"] = "Material adicionado com sucesso!";
                    }
                    else
                    {
                        ModelState.AddModelError("", "Esse material já existe.");
                        ViewData["NovoMaterialNome"] = novoMaterialNome;
                        ViewData["NovoMaterialTamanho"] = novoMaterialTamanho;
                        ViewData["NovoMaterialQtd"] = novoMaterialQuantidade;
                    }
                }
                else
                {
                    ViewData["NovoMaterialNome"] = novoMaterialNome;
                    ViewData["NovoMaterialTamanho"] = novoMaterialTamanho;
                    ViewData["NovoMaterialQtd"] = novoMaterialQuantidade;
                }

                viewModel = await ReconstruirViewModel(viewModel, novoMaterialNome);
                ViewData["EspecialidadeId"] = new SelectList(_context.Especialidades.OrderBy(e => e.Nome), "EspecialidadeId", "Nome", viewModel.EspecialidadeId);
                return View(viewModel);
            }

            // --- LÓGICA 2: ATUALIZAR EXAME ---
            if (await _context.ExameTipo.AnyAsync(et => et.Nome == viewModel.NomeExame && et.ExameTipoId != id))
            {
                ModelState.AddModelError("NomeExame", "Já existe outro exame com este nome.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var exameTipoToUpdate = await _context.ExameTipo
                        .Include(et => et.ExameTipoRecursos)
                        .FirstOrDefaultAsync(m => m.ExameTipoId == id);

                    if (exameTipoToUpdate == null) return NotFound();

                    exameTipoToUpdate.Nome = viewModel.NomeExame;
                    exameTipoToUpdate.Descricao = viewModel.Descricao;
                    exameTipoToUpdate.EspecialidadeId = viewModel.EspecialidadeId;

                    if (exameTipoToUpdate.ExameTipoRecursos.Any())
                    {
                        _context.ExameTipoRecursos.RemoveRange(exameTipoToUpdate.ExameTipoRecursos);
                    }

                    foreach (var item in viewModel.Recursos.Where(r => r.IsSelected))
                    {
                        exameTipoToUpdate.ExameTipoRecursos.Add(new ExameTipoRecurso
                        {
                            ExameTipoId = id,
                            MaterialEquipamentoAssociadoId = item.Id,
                            QuantidadeNecessaria = item.Quantidade
                        });
                    }

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Atualizado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExameTipoExists(viewModel.ExameTipoId)) return NotFound();
                    else throw;
                }
            }

            viewModel = await ReconstruirViewModel(viewModel, null);
            ViewData["NovoMaterialNome"] = novoMaterialNome ?? "";
            ViewData["NovoMaterialTamanho"] = novoMaterialTamanho ?? "";
            ViewData["NovoMaterialQtd"] = novoMaterialQuantidade ?? 0;
            ViewData["EspecialidadeId"] = new SelectList(_context.Especialidades.OrderBy(e => e.Nome), "EspecialidadeId", "Nome", viewModel.EspecialidadeId);
            return View(viewModel);
        }

        // GET: ExameTipoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var exameTipo = await _context.ExameTipo
                .Include(et => et.Especialidade)
                .FirstOrDefaultAsync(m => m.ExameTipoId == id);

            if (exameTipo == null) return NotFound();

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
                if (await _context.Exames.AnyAsync(e => e.ExameTipoId == id))
                {
                    TempData["ErrorMessage"] = "Não pode apagar este Tipo de Exame porque existem marcações associadas.";
                    return RedirectToAction(nameof(Index));
                }

                _context.ExameTipo.Remove(exameTipo);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Apagado com sucesso!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ExameTipoExists(int id)
        {
            return _context.ExameTipo.Any(e => e.ExameTipoId == id);
        }

        // --- Helper para reconstruir a lista mantendo o estado do utilizador ---
        private async Task<TipoExameRecursosViewModel> ReconstruirViewModel(TipoExameRecursosViewModel viewModel, string? novoMaterialNome)
        {
            var todosMateriais = await _context.MaterialEquipamentoAssociado
                .OrderBy(m => m.NomeEquipamento)
                .ToListAsync();

            var listaAtualizada = new List<RecursoCheckBoxItem>();

            foreach (var materialDb in todosMateriais)
            {
                var itemFormulario = viewModel.Recursos?
                    .FirstOrDefault(r => r.Id == materialDb.MaterialEquipamentoAssociadoId);

                bool isNewItem = (!string.IsNullOrEmpty(novoMaterialNome) && materialDb.NomeEquipamento == novoMaterialNome);

                listaAtualizada.Add(new RecursoCheckBoxItem
                {
                    Id = materialDb.MaterialEquipamentoAssociadoId,
                    Nome = materialDb.NomeEquipamento,
                    Tamanho = materialDb.Tamanho, // <--- ADICIONADO AQUI TAMBÉM
                    IsSelected = itemFormulario?.IsSelected ?? isNewItem,
                    Quantidade = itemFormulario?.Quantidade ?? 1
                });
            }

            viewModel.Recursos = listaAtualizada;
            return viewModel;
        }
    }
}