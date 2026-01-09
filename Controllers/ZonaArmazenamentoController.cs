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

            // --- LÓGICA DE ALERTAS E PENDÊNCIAS (MODIFICADO) ---
            var todosConsumiveis = await _context.Consumivel.ToListAsync();
            foreach (var c in todosConsumiveis)
            {
                // Alerta de Stock Crítico
                if (c.QuantidadeAtual <= c.QuantidadeMinima)
                {
                    TempData[$"Aviso_{c.ConsumivelId}"] = $"Stock Crítico: {c.Nome}. Reposição necessária!";
                }

                // Alerta de Encomenda Pendente (Sinalizado pelo ConsumivelController)
                if (TempData.ContainsKey($"Pendente_{c.ConsumivelId}"))
                {
                    TempData.Keep($"Pendente_{c.ConsumivelId}"); // Mantém o sinal ativo até ser clicado
                }
            }

            // --- Filtros de Pesquisa ---
            if (!string.IsNullOrEmpty(searchNome))
                zonasQuery = zonasQuery.Where(z => z.NomeZona.Contains(searchNome));

            if (!string.IsNullOrEmpty(searchLocalizacao))
                zonasQuery = zonasQuery.Where(z => z.Room.Name.Contains(searchLocalizacao));

            if (searchConsumivel.HasValue)
                zonasQuery = zonasQuery.Where(z => z.ConsumivelId == searchConsumivel.Value);

            switch (estado)
            {
                case "ativas": zonasQuery = zonasQuery.Where(z => z.Ativa == true); break;
                case "inativas": zonasQuery = zonasQuery.Where(z => z.Ativa == false); break;
            }

            // Dropdowns e paginação
            ViewBag.ConsumiveisList = new SelectList(_context.Consumivel.OrderBy(c => c.Nome), "ConsumivelId", "Nome", searchConsumivel);
            ViewBag.SearchNome = searchNome;
            ViewBag.SearchLocalizacao = searchLocalizacao;
            ViewBag.Estado = estado;
            ViewBag.SearchConsumivel = searchConsumivel;

            int totalZonas = await zonasQuery.CountAsync();
            var pagination = new PaginationInfo<ZonaArmazenamento>(page, totalZonas, 10);
            pagination.Items = await zonasQuery.OrderBy(z => z.NomeZona).Skip(pagination.ItemsToSkip).Take(pagination.ItemsPerPage).ToListAsync();

            return View(pagination);
        }

        // --- AÇÃO PARA CONFIRMAR RECEÇÃO (MODIFICADO) ---
        // Agora este método é responsável por atualizar o stock físico
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarRececao(int consumivelId)
        {
            var consumivel = await _context.Consumivel.FindAsync(consumivelId);

            // Procura a última compra registada para este consumível que ainda não foi arrumada
            var ultimaCompra = await _context.Compra
                .Where(c => c.ConsumivelId == consumivelId)
                .OrderByDescending(c => c.CompraId)
                .FirstOrDefaultAsync();

            if (consumivel != null && ultimaCompra != null)
            {
                int quantidadeParaDistribuir = ultimaCompra.Quantidade;

                // DISTRIBUIÇÃO INTELIGENTE PELAS ZONAS ATIVAS
                var zonasDisponiveis = await _context.ZonaArmazenamento
                    .Where(z => z.ConsumivelId == consumivelId && z.Ativa)
                    .OrderBy(z => z.NomeZona)
                    .ToListAsync();

                foreach (var zona in zonasDisponiveis)
                {
                    if (quantidadeParaDistribuir <= 0) break;

                    int espacoLivre = zona.CapacidadeMaxima - zona.QuantidadeAtual;
                    if (espacoLivre > 0)
                    {
                        int aAdicionar = Math.Min(quantidadeParaDistribuir, espacoLivre);
                        zona.QuantidadeAtual += aAdicionar;
                        quantidadeParaDistribuir -= aAdicionar;
                        _context.Update(zona);
                    }
                }

                await _context.SaveChangesAsync();

                // Sincroniza o total do Consumível
                await AtualizarQuantidadeAtualConsumivel(consumivelId);

                // Limpa o sinalizador de pendência
                TempData.Remove($"Pendente_{consumivelId}");

                TempData["SuccessMessage"] = $"✅ Stock de {consumivel.Nome} atualizado e arrumado nas zonas!";
            }

            return RedirectToAction(nameof(Index));
        }

        // AÇÃO ANTIGA (AJUSTADA PARA O NOVO FLUXO)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AceitarPropostaZona(int id)
        {
            // Este botão apenas envia para o controller de consumíveis para criar a encomenda
            return RedirectToAction("AceitarEncomenda", "Consumivel", new { id = id });
        }

        // -----------------------------
        // MÉTODOS DE SUPORTE E CRUD
        // -----------------------------

        private void PreencherDropDowns(int? consumivelId = null, int? roomId = null)
        {
            ViewBag.Consumiveis = new SelectList(_context.Consumivel.OrderBy(c => c.Nome), "ConsumivelId", "Nome", consumivelId);
            ViewBag.Rooms = new SelectList(_context.Set<Room>().OrderBy(r => r.Name), "RoomId", "Name", roomId);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var zona = await _context.ZonaArmazenamento.Include(z => z.Consumivel).Include(z => z.Room).FirstOrDefaultAsync(m => m.ZonaId == id);
            if (zona == null) return NotFound();

            ViewBag.TotalConsumivel = await _context.ZonaArmazenamento.Where(z => z.ConsumivelId == zona.ConsumivelId).SumAsync(z => z.QuantidadeAtual);
            return View(zona);
        }

        public IActionResult Create() { PreencherDropDowns(); return View(); }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ZonaId,NomeZona,ConsumivelId,RoomId,CapacidadeMaxima,QuantidadeAtual,Ativa")] ZonaArmazenamento zona)
        {
            if (zona.QuantidadeAtual > zona.CapacidadeMaxima)
                ModelState.AddModelError("QuantidadeAtual", "A quantidade atual não pode exceder a capacidade.");

            if (zona.Ativa == false && zona.QuantidadeAtual > 0)
                ModelState.AddModelError("Ativa", "Zonas inativas devem ter stock 0.");

            if (ModelState.IsValid)
            {
                _context.Add(zona);
                await _context.SaveChangesAsync();
                await AtualizarQuantidadeAtualConsumivel(zona.ConsumivelId);
                await AtualizarQuantidadeMaximaConsumivel(zona.ConsumivelId);
                TempData["SuccessMessage"] = "✅ Zona criada com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            PreencherDropDowns(zona.ConsumivelId, zona.RoomId);
            return View(zona);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var zona = await _context.ZonaArmazenamento.FindAsync(id);
            if (zona == null) return NotFound();
            PreencherDropDowns(zona.ConsumivelId, zona.RoomId);
            return View(zona);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ZonaId,NomeZona,ConsumivelId,RoomId,CapacidadeMaxima,QuantidadeAtual,Ativa")] ZonaArmazenamento zona)
        {
            if (id != zona.ZonaId) return NotFound();

            if (zona.QuantidadeAtual > zona.CapacidadeMaxima)
                ModelState.AddModelError("QuantidadeAtual", "A quantidade atual excede a capacidade.");

            if (zona.Ativa == false && zona.QuantidadeAtual > 0)
                ModelState.AddModelError("Ativa", "Inative a zona apenas com stock 0.");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(zona);
                    await _context.SaveChangesAsync();
                    await AtualizarQuantidadeAtualConsumivel(zona.ConsumivelId);
                    await AtualizarQuantidadeMaximaConsumivel(zona.ConsumivelId);
                    TempData["SuccessMessage"] = "💾 Alterações guardadas!";
                }
                catch (DbUpdateConcurrencyException) { if (!_context.ZonaArmazenamento.Any(e => e.ZonaId == id)) return NotFound(); throw; }
                return RedirectToAction(nameof(Index));
            }
            PreencherDropDowns(zona.ConsumivelId, zona.RoomId);
            return View(zona);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var zona = await _context.ZonaArmazenamento.Include(z => z.Consumivel).Include(z => z.Room).FirstOrDefaultAsync(m => m.ZonaId == id);
            return (zona == null) ? NotFound() : View(zona);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var zona = await _context.ZonaArmazenamento.FindAsync(id);
            if (zona == null) return RedirectToAction(nameof(Index));

            if (zona.Ativa)
            {
                TempData["ErrorMessage"] = "❌ Inative a zona antes de apagar.";
                return RedirectToAction(nameof(Index));
            }

            int consumivelId = zona.ConsumivelId;
            _context.ZonaArmazenamento.Remove(zona);
            await _context.SaveChangesAsync();

            await AtualizarQuantidadeAtualConsumivel(consumivelId);
            await AtualizarQuantidadeMaximaConsumivel(consumivelId);

            TempData["SuccessMessage"] = "🗑️ Zona eliminada!";
            return RedirectToAction(nameof(Index));
        }

        private async Task AtualizarQuantidadeAtualConsumivel(int consumivelId)
        {
            var quantidadeTotal = await _context.ZonaArmazenamento.Where(z => z.ConsumivelId == consumivelId).SumAsync(z => z.QuantidadeAtual);
            var consumivel = await _context.Consumivel.FindAsync(consumivelId);
            if (consumivel != null) { consumivel.QuantidadeAtual = quantidadeTotal; _context.Update(consumivel); await _context.SaveChangesAsync(); }
        }

        private async Task AtualizarQuantidadeMaximaConsumivel(int consumivelId)
        {
            var quantidadeMaximaTotal = await _context.ZonaArmazenamento.Where(z => z.ConsumivelId == consumivelId).SumAsync(z => z.CapacidadeMaxima);
            var consumivel = await _context.Consumivel.FindAsync(consumivelId);
            if (consumivel != null) { consumivel.QuantidadeMaxima = quantidadeMaximaTotal; _context.Update(consumivel); await _context.SaveChangesAsync(); }
        }
    }
}