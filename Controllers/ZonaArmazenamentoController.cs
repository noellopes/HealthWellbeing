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

            // Se tiveres DbSet<Room> no contexto:
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
            ViewBag.SearchConsumivel = searchConsumivel; // Importante para paginação

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
            if (zona.QuantidadeAtual > zona.CapacidadeMaxima)
            {
                ModelState.AddModelError("QuantidadeAtual", "A quantidade atual não pode ser superior à capacidade máxima.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(zona);
                await _context.SaveChangesAsync();
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

            if (zona.QuantidadeAtual > zona.CapacidadeMaxima)
            {
                ModelState.AddModelError("QuantidadeAtual", "A quantidade atual não pode ser superior à capacidade máxima.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(zona);
                    await _context.SaveChangesAsync();
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

            if (zona != null)
            {
                _context.ZonaArmazenamento.Remove(zona);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "🗑️ Zona eliminada com sucesso!";
            }
            else
            {
                TempData["ErrorMessage"] = "❌ Erro ao eliminar a zona.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}