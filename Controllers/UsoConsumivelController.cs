using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace HealthWellbeing.Controllers
{
    public class UsoConsumivelController : Controller
    {
        private readonly HealthWellbeingDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UsoConsumivelController(HealthWellbeingDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ==========================================
        // GET: UsoConsumivel (Lista Geral)
        // ==========================================
        public async Task<IActionResult> Index()
        {
            var healthWellbeingDbContext = _context.UsoConsumivel
                .Include(u => u.Consumivel)
                .Include(u => u.TreatmentRecord);
            return View(await healthWellbeingDbContext.ToListAsync());
        }

        // ==========================================
        // GET: UsoConsumivel/Details/5
        // ==========================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var usoConsumivel = await _context.UsoConsumivel
                .Include(u => u.Consumivel)
                .Include(u => u.TreatmentRecord)
                .FirstOrDefaultAsync(m => m.UsoConsumivelId == id);

            if (usoConsumivel == null) return NotFound();

            // Buscar o nome do user pelo UserId
            if (!string.IsNullOrEmpty(usoConsumivel.UserId))
            {
                var user = await _userManager.FindByIdAsync(usoConsumivel.UserId);
                ViewBag.UserName = user?.UserName ?? "Desconhecido";
            }
            else
            {
                ViewBag.UserName = "Sistema / Desconhecido";
            }

            return View(usoConsumivel);
        }

        // ==========================================
        // GET: UsoConsumivel/Create
        // ==========================================
        public IActionResult Create(int treatmentRecordId)
        {
            ViewData["ConsumivelID"] = new SelectList(_context.Consumivel, "ConsumivelId", "Nome");

            // Buscar todas as zonas ativas para lógica de frontend (se necessário)
            var zonas = _context.ZonaArmazenamento
                                .Where(z => z.Ativa)
                                .Select(z => new {
                                    z.ZonaId,
                                    z.NomeZona,
                                    z.ConsumivelId,
                                    z.QuantidadeAtual // Importante para saber o stock no JS
                                })
                                .ToList();

            // Passar para ViewBag como JSON para JS
            ViewBag.Zonas = Newtonsoft.Json.JsonConvert.SerializeObject(zonas);

            // Envia o id para a view
            ViewBag.TreatmentRecordId = treatmentRecordId;

            return View();
        }

        // ==========================================
        // POST: UsoConsumivel/Create (Single)
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UsoConsumivelId,TreatmentRecordId,ConsumivelID,QuantidadeUsada,DataConsumo")] UsoConsumivel usoConsumivel)
        {
            if (ModelState.IsValid)
            {
                // 1. Encontrar Zona Ativa com Stock Suficiente
                var zona = await _context.ZonaArmazenamento
                    .Where(z => z.ConsumivelId == usoConsumivel.ConsumivelId
                             && z.Ativa
                             && z.QuantidadeAtual >= usoConsumivel.QuantidadeUsada)
                    .OrderByDescending(z => z.QuantidadeAtual) // Prioriza a que tem mais stock
                    .FirstOrDefaultAsync();

                if (zona == null)
                {
                    ModelState.AddModelError("", "❌ Erro: Não existe stock suficiente em nenhuma zona ativa para realizar este consumo.");

                    // Recarregar as listas para não dar erro na View
                    ViewData["ConsumivelID"] = new SelectList(_context.Consumivel, "ConsumivelId", "Nome", usoConsumivel.ConsumivelId);
                    ViewData["TreatmentRecordId"] = new SelectList(_context.TreatmentRecord, "Id", "Id", usoConsumivel.TreatmentRecordId);
                    return View(usoConsumivel);
                }

                // 2. Configurar o Registo
                usoConsumivel.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                usoConsumivel.ZonaArmazenamentoID = zona.ZonaId; // Associa à zona encontrada

                if (usoConsumivel.DataConsumo == DateTime.MinValue)
                    usoConsumivel.DataConsumo = DateTime.Now;

                _context.Add(usoConsumivel);

                // 3.  SUBTRAIR STOCK DA ZONA
                zona.QuantidadeAtual -= usoConsumivel.QuantidadeUsada;
                _context.Update(zona);

                // 4.  SUBTRAIR STOCK TOTAL DO CONSUMÍVEL (Pai)
                var consumivelPai = await _context.Consumivel.FindAsync(usoConsumivel.ConsumivelId);
                if (consumivelPai != null)
                {
                    consumivelPai.QuantidadeAtual -= usoConsumivel.QuantidadeUsada;
                    _context.Update(consumivelPai);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ConsumivelID"] = new SelectList(_context.Consumivel, "ConsumivelId", "Nome", usoConsumivel.ConsumivelId);
            ViewData["TreatmentRecordId"] = new SelectList(_context.TreatmentRecord, "Id", "Id", usoConsumivel.TreatmentRecordId);
            return View(usoConsumivel);
        }

        // ==========================================
        // POST: UsoConsumivel/CreateMultiple (Arrays)
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMultiple(int treatmentRecordId, int[] ConsumivelID, int[] QuantidadeUsada, DateTime[] DataConsumo)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<string> erros = new List<string>(); // Para guardar erros de stock

            for (int i = 0; i < ConsumivelID.Length; i++)
            {
                int qtd = QuantidadeUsada[i];
                int consumivelId = ConsumivelID[i];

                // 1. Encontrar Zona Ativa com Stock Suficiente
                var zona = await _context.ZonaArmazenamento
                                         .Where(z => z.ConsumivelId == consumivelId
                                                  && z.Ativa
                                                  && z.QuantidadeAtual >= qtd)
                                         .OrderByDescending(z => z.QuantidadeAtual)
                                         .FirstOrDefaultAsync();

                if (zona == null)
                {
                    // Regista o erro mas continua para os próximos itens
                    erros.Add($"Consumível ID {consumivelId}: Stock insuficiente.");
                    continue;
                }

                // 2. Criar Registo
                var uso = new UsoConsumivel
                {
                    TreatmentRecordId = treatmentRecordId,
                    ConsumivelId = consumivelId,
                    QuantidadeUsada = qtd,
                    DataConsumo = DataConsumo[i],
                    UserId = userId,
                    ZonaArmazenamentoID = zona.ZonaId
                };

                _context.UsoConsumivel.Add(uso);

                // 3.  SUBTRAIR STOCK DA ZONA
                zona.QuantidadeAtual -= qtd;
                _context.Update(zona);

                // 4.  SUBTRAIR STOCK TOTAL DO CONSUMÍVEL
                var consumivelPai = await _context.Consumivel.FindAsync(consumivelId);
                if (consumivelPai != null)
                {
                    consumivelPai.QuantidadeAtual -= qtd;
                    _context.Update(consumivelPai);
                }
            }

            // Gravar tudo o que foi possível
            await _context.SaveChangesAsync();

            // Se houve erros, passa para o TempData (opcional: podes tratar isto na View de destino)
            if (erros.Count > 0)
            {
                TempData["ErrorMessage"] = "Atenção: Alguns itens não foram registados por falta de stock. Verifique o inventário.";
            }

            return RedirectToAction("Details", "TreatmentRecords", new { id = treatmentRecordId });
        }

        // ==========================================
        // GET: UsoConsumivel/Edit/5
        // ==========================================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var usoConsumivel = await _context.UsoConsumivel.FindAsync(id);
            if (usoConsumivel == null) return NotFound();

            ViewData["ConsumivelId"] = new SelectList(_context.Consumivel, "ConsumivelId", "Nome", usoConsumivel.ConsumivelId);
            ViewData["TreatmentRecordId"] = new SelectList(_context.TreatmentRecord, "Id", "Id", usoConsumivel.TreatmentRecordId);
            return View(usoConsumivel);
        }

        // ==========================================
        // POST: UsoConsumivel/Edit/5
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UsoConsumivelId,TreatmentRecordId,ConsumivelID,QuantidadeUsada,DataConsumo")] UsoConsumivel usoConsumivel)
        {
            if (id != usoConsumivel.UsoConsumivelId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // NOTA: A edição simples não repõe nem retira stock extra automaticamente
                    // para evitar complexidade e erros de cálculo. 
                    // Se for crítico, terias de comparar com o valor antigo (AsNoTracking).

                    _context.Update(usoConsumivel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsoConsumivelExists(usoConsumivel.UsoConsumivelId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ConsumivelID"] = new SelectList(_context.Consumivel, "ConsumivelId", "Nome", usoConsumivel.ConsumivelId);
            ViewData["TreatmentRecordId"] = new SelectList(_context.TreatmentRecord, "Id", "Id", usoConsumivel.TreatmentRecordId);
            return View(usoConsumivel);
        }

        // ==========================================
        // GET: UsoConsumivel/Delete/5
        // ==========================================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var usoConsumivel = await _context.UsoConsumivel
                .Include(u => u.Consumivel)
                .Include(u => u.TreatmentRecord)
                .FirstOrDefaultAsync(m => m.UsoConsumivelId == id);

            if (usoConsumivel == null) return NotFound();

            return View(usoConsumivel);
        }

        // ==========================================
        // POST: UsoConsumivel/Delete/5
        // ==========================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usoConsumivel = await _context.UsoConsumivel.FindAsync(id);
            if (usoConsumivel != null)
            {
                // NOTA: Se apagares o registo de uso, idealmente devias repor o stock.
                // Vou adicionar essa lógica para ficar perfeito:

                // 1. Repor na Zona (se ainda existir)
                if (usoConsumivel.ZonaArmazenamentoID != 0)
                {
                    var zona = await _context.ZonaArmazenamento.FindAsync(usoConsumivel.ZonaArmazenamentoID);
                    if (zona != null)
                    {
                        zona.QuantidadeAtual += usoConsumivel.QuantidadeUsada;
                        _context.Update(zona);
                    }
                }

                // 2. Repor no Total do Consumível
                var consumivel = await _context.Consumivel.FindAsync(usoConsumivel.ConsumivelId);
                if (consumivel != null)
                {
                    consumivel.QuantidadeAtual += usoConsumivel.QuantidadeUsada;
                    _context.Update(consumivel);
                }

                _context.UsoConsumivel.Remove(usoConsumivel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsoConsumivelExists(int id)
        {
            return _context.UsoConsumivel.Any(e => e.UsoConsumivelId == id);
        }
    }
}