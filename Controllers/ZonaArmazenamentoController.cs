using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace HealthWellbeing.Controllers
{
    [Authorize(Roles = "Gestor de armazenamento")]
    public class ZonaArmazenamentoController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ZonaArmazenamentoController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // -----------------------------
        // HELPERS
        // -----------------------------
        private void PreencherDropDowns(int? consumivelId = null, int? roomId = null)
        {
            ViewBag.Consumiveis = new SelectList(
                _context.Consumivel.OrderBy(c => c.Nome),
                "ConsumivelId",
                "Nome",
                consumivelId
            );

            
            ViewBag.Rooms = new SelectList(
                _context.Set<Room>().OrderBy(r => r.Name),
                "RoomId",
                "Name",
                roomId
            );
        }

        
        // -----------------------------
        // GET: ZonaArmazenamento (Index)
        // -----------------------------
        public async Task<IActionResult> Index(
            int page = 1,
            string searchNome = "",
            string searchLocalizacao = "",
            string estado = "todas",
            int? searchConsumivel = null)
        {
            var zonasQuery = _context.ZonaArmazenamento
                .Include(z => z.Consumivel)
                .Include(z => z.Room)
                .AsQueryable();

            // 1. Pesquisa por nome da zona
            if (!string.IsNullOrEmpty(searchNome))
                zonasQuery = zonasQuery.Where(z => z.NomeZona.Contains(searchNome));

            // 2. Pesquisa por sala
            if (!string.IsNullOrEmpty(searchLocalizacao))
                zonasQuery = zonasQuery.Where(z => z.Room.Name.Contains(searchLocalizacao));

            // 3. Pesquisa por Consumível (Dropdown)
            if (searchConsumivel.HasValue)
            {
                zonasQuery = zonasQuery.Where(z => z.ConsumivelId == searchConsumivel.Value);
            }

            // 4. Filtro por estado
            switch (estado)
            {
                case "ativas":
                    zonasQuery = zonasQuery.Where(z => z.Ativa == true);
                    break;
                case "inativas":
                    zonasQuery = zonasQuery.Where(z => z.Ativa == false);
                    break;
            }

            // --- Carregar a lista para a Dropdown de Pesquisa na View ---
            ViewBag.ConsumiveisList = new SelectList(
                _context.Consumivel.OrderBy(c => c.Nome),
                "ConsumivelId",
                "Nome",
                searchConsumivel // Mantém selecionado o que o user escolheu
            );

            // Manter valores na View
            ViewBag.SearchNome = searchNome;
            ViewBag.SearchLocalizacao = searchLocalizacao;
            ViewBag.Estado = estado;
            ViewBag.SearchConsumivel = searchConsumivel; 

            // Paginação
            int totalZonas = await zonasQuery.CountAsync();
            var pagination = new PaginationInfo<ZonaArmazenamento>(page, totalZonas, 10);

            pagination.Items = await zonasQuery
                .OrderBy(z => z.NomeZona)
                .Skip(pagination.ItemsToSkip)
                .Take(pagination.ItemsPerPage)
                .ToListAsync();

            return View(pagination);
        }

        // -----------------------------
        // GET: ZonaArmazenamento/Details/5
        // -----------------------------
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var zona = await _context.ZonaArmazenamento
                .Include(z => z.Consumivel)
                .Include(z => z.Room)
                .FirstOrDefaultAsync(m => m.ZonaId == id);

            if (zona == null) return NotFound();

            var totalStock = await _context.ZonaArmazenamento
                .Where(z => z.ConsumivelId == zona.ConsumivelId)
                .SumAsync(z => z.QuantidadeAtual);

            ViewBag.TotalConsumivel = totalStock;

            return View(zona);
        }

        // -----------------------------
        // GET: ZonaArmazenamento/Create
        // -----------------------------
        public IActionResult Create()
        {
            PreencherDropDowns();
            return View();
        }

        // -----------------------------
        // POST: ZonaArmazenamento/Create
        // -----------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ZonaId,NomeZona,ConsumivelId,RoomId,CapacidadeMaxima,QuantidadeAtual,Ativa")] ZonaArmazenamento zona)
        {
            // Validação 1: Capacidade Máxima
            if (zona.QuantidadeAtual > zona.CapacidadeMaxima)
            {
                ModelState.AddModelError("QuantidadeAtual", "A quantidade atual não pode ser superior à capacidade máxima.");
            }

            // Validação 2: Inativa vs Stock 
            if (zona.Ativa == false && zona.QuantidadeAtual > 0)
            {
                ModelState.AddModelError("Ativa", "Não é possível ter stock numa zona inativa. Para inativar a zona, a quantidade deve ser 0.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(zona);
                await _context.SaveChangesAsync();

                
                await AtualizarQuantidadeAtualConsumivel(zona.ConsumivelId);
                await AtualizarQuantidadeMaximaConsumivel(zona.ConsumivelId);


                TempData["SuccessMessage"] = "✅ Zona criada com sucesso!";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "❌ Erro ao criar a zona.";
            PreencherDropDowns(zona.ConsumivelId, zona.RoomId);
            return View(zona);
        }

        // -----------------------------
        // GET: ZonaArmazenamento/Edit/5
        // -----------------------------
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var zona = await _context.ZonaArmazenamento.FindAsync(id);
            if (zona == null) return NotFound();

            PreencherDropDowns(zona.ConsumivelId, zona.RoomId);
            return View(zona);
        }

        // -----------------------------
        // POST: ZonaArmazenamento/Edit/5
        // -----------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ZonaId,NomeZona,ConsumivelId,RoomId,CapacidadeMaxima,QuantidadeAtual,Ativa")] ZonaArmazenamento zona)
        {
            if (id != zona.ZonaId) return NotFound();

            // Validação 1: Capacidade Máxima
            if (zona.QuantidadeAtual > zona.CapacidadeMaxima)
            {
                ModelState.AddModelError("QuantidadeAtual", "A quantidade atual não pode ser superior à capacidade máxima.");
            }

            // Validação 2: Inativa vs Stock (NOVO)
            if (zona.Ativa == false && zona.QuantidadeAtual > 0)
            {
                ModelState.AddModelError("Ativa", "Conflito: Uma zona inativa não pode ter consumíveis (Quantidade tem de ser 0).");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(zona);
                    await _context.SaveChangesAsync();

                    // Atualiza o total no consumível pai
                    await AtualizarQuantidadeAtualConsumivel(zona.ConsumivelId);
                    await AtualizarQuantidadeMaximaConsumivel(zona.ConsumivelId);

                    TempData["SuccessMessage"] = "💾 Alterações guardadas com sucesso!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ZonaArmazenamento.Any(e => e.ZonaId == id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "❌ Erro ao editar a zona.";
            PreencherDropDowns(zona.ConsumivelId, zona.RoomId);
            return View(zona);
        }

        // -----------------------------
        // GET: ZonaArmazenamento/Delete/5
        // -----------------------------
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var zona = await _context.ZonaArmazenamento
                .Include(z => z.Consumivel)
                .Include(z => z.Room)
                .FirstOrDefaultAsync(m => m.ZonaId == id);

            if (zona == null) return NotFound();

            return View(zona);
        }

        // -----------------------------
        // POST: ZonaArmazenamento/Delete/5
        // -----------------------------
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var zona = await _context.ZonaArmazenamento.FindAsync(id);

            if (zona == null)
            {
                TempData["ErrorMessage"] = "❌ Erro ao eliminar a zona.";
                return RedirectToAction(nameof(Index));
            }

            if (zona.Ativa)
            {
                TempData["ErrorMessage"] = "❌ Não é possível apagar uma zona que está Ativa. Por favor, inative-a primeiro.";
                return RedirectToAction(nameof(Index));
            }

            // Verifica se existem stocks associados na tabela Stock
            int numStocks = await _context.Stock.CountAsync(s => s.ZonaID == zona.ZonaId);
            if (numStocks > 0)
            {
                TempData["ErrorMessage"] = $"❌ Não é possível apagar esta zona. Existem {numStocks} registos de stock associados.";
                return RedirectToAction(nameof(Index));
            }

            // Guardar ID do consumível para atualizar total depois
            int consumivelId = zona.ConsumivelId;

            // Remove a zona
            _context.ZonaArmazenamento.Remove(zona);
            await _context.SaveChangesAsync();

            // Atualiza o total no consumível pai
            await AtualizarQuantidadeAtualConsumivel(consumivelId);
            // Atualiza consumível
            await AtualizarQuantidadeAtualConsumivel(zona.ConsumivelId);
            await AtualizarQuantidadeMaximaConsumivel(zona.ConsumivelId);

            TempData["SuccessMessage"] = "🗑️ Zona eliminada com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // -----------------------------
        // GET: ZonaArmazenamento/Transferir/5
        // -----------------------------
        public async Task<IActionResult> Transferir(int? id)
        {
            if (id == null) return NotFound();

            // Vai buscar a zona de origem e o nome do produto
            var zonaOrigem = await _context.ZonaArmazenamento
                .Include(z => z.Consumivel)
                .FirstOrDefaultAsync(z => z.ZonaId == id);

            if (zonaOrigem == null) return NotFound();

            // Prepara o Modelo para o formulário
            var model = new TransferenciaStock
            {
                ZonaOrigemId = zonaOrigem.ZonaId,
                ZonaOrigemNome = zonaOrigem.NomeZona,
                ConsumivelNome = zonaOrigem.Consumivel?.Nome,
                QuantidadeAtualOrigem = zonaOrigem.QuantidadeAtual,
                QuantidadeATransferir = 0
            };

            // Carregar APENAS zonas de destino válidas:
            // 1. Mesmo Consumível
            // 2. Zona diferente da origem
            // 3. Zona Ativa
            var zonasDestino = await _context.ZonaArmazenamento
                .Where(z => z.ConsumivelId == zonaOrigem.ConsumivelId
                            && z.ZonaId != zonaOrigem.ZonaId
                            && z.Ativa == true)
                .Select(z => new
                {
                    z.ZonaId,
                    Display = $"{z.NomeZona} (Livre: {z.CapacidadeMaxima - z.QuantidadeAtual})"
                })
                .ToListAsync();

            if (!zonasDestino.Any())
            {
                TempData["ErrorMessage"] = "⚠️ Não existem outras zonas ativas para este produto para onde possa transferir stock.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ZonasDestino = new SelectList(zonasDestino, "ZonaId", "Display");

            return View(model);
        }

        // -----------------------------
        // POST: ZonaArmazenamento/Transferir
        // -----------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Transferir(TransferenciaStock model)
        {
            if (ModelState.IsValid)
            {
                var zonaOrigem = await _context.ZonaArmazenamento.FindAsync(model.ZonaOrigemId);
                var zonaDestino = await _context.ZonaArmazenamento.FindAsync(model.ZonaDestinoId);

                if (zonaOrigem == null || zonaDestino == null)
                {
                    TempData["ErrorMessage"] = "❌ Erro: Zona de origem ou destino não encontrada.";
                    return RedirectToAction(nameof(Index));
                }

                // --- VALIDAÇÕES LÓGICAS ---

                // 1. Tem stock suficiente na origem?
                if (zonaOrigem.QuantidadeAtual < model.QuantidadeATransferir)
                {
                    ModelState.AddModelError("QuantidadeATransferir", "Não tem stock suficiente na origem.");
                }

                // 2. Tem espaço suficiente no destino?
                int espacoLivreDestino = zonaDestino.CapacidadeMaxima - zonaDestino.QuantidadeAtual;
                if (espacoLivreDestino < model.QuantidadeATransferir)
                {
                    ModelState.AddModelError("QuantidadeATransferir", $"O destino só tem espaço para {espacoLivreDestino} unidades.");
                }

                // Se passou nas validações manuais acima
                if (ModelState.IsValid)
                {
                    // Executa a Movimentação
                    zonaOrigem.QuantidadeAtual -= model.QuantidadeATransferir;
                    zonaDestino.QuantidadeAtual += model.QuantidadeATransferir;

                    _context.Update(zonaOrigem);
                    _context.Update(zonaDestino);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"✅ Transferência de {model.QuantidadeATransferir} unidades realizada com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
            }

            // --- CASO DE ERRO (Recarregar a View) ---
            var zonaOrigemDb = await _context.ZonaArmazenamento.FindAsync(model.ZonaOrigemId);

            if (zonaOrigemDb != null)
            {
                var zonasDestino = await _context.ZonaArmazenamento
               .Where(z => z.ConsumivelId == zonaOrigemDb.ConsumivelId
                           && z.ZonaId != model.ZonaOrigemId
                           && z.Ativa == true)
               .Select(z => new
               {
                   z.ZonaId,
                   Display = $"{z.NomeZona} (Livre: {z.CapacidadeMaxima - z.QuantidadeAtual})"
               })
               .ToListAsync();

                ViewBag.ZonasDestino = new SelectList(zonasDestino, "ZonaId", "Display", model.ZonaDestinoId);
            }

            return View(model);
        }



        private async Task AtualizarQuantidadeAtualConsumivel(int consumivelId)
        {
            // Obter todas as zonas que têm este consumível
            var quantidadeTotal = await _context.ZonaArmazenamento
                .Where(z => z.ConsumivelId == consumivelId)
                .SumAsync(z => z.QuantidadeAtual);

            // Atualizar o Consumível
            var consumivel = await _context.Consumivel.FindAsync(consumivelId);
            if (consumivel != null)
            {
                consumivel.QuantidadeAtual = quantidadeTotal;
                _context.Update(consumivel);
                await _context.SaveChangesAsync();
            }
        }
        private async Task AtualizarQuantidadeMaximaConsumivel(int consumivelId)
        {
            // Soma todas as capacidades máximas das zonas associadas
            var quantidadeMaximaTotal = await _context.ZonaArmazenamento
                .Where(z => z.ConsumivelId == consumivelId)
                .SumAsync(z => z.CapacidadeMaxima);

            // Atualiza o Consumível
            var consumivel = await _context.Consumivel.FindAsync(consumivelId);
            if (consumivel != null)
            {
                consumivel.QuantidadeMaxima = quantidadeMaximaTotal;
                _context.Update(consumivel);
                await _context.SaveChangesAsync();
            }
        }
    }
}