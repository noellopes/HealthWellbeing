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
                .Include(u => u.TreatmentRecord)
                .Include(u => u.ZonaArmazenamento);

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
                                    z.QuantidadeAtual 
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
                    .OrderByDescending(z => z.QuantidadeAtual) 
                    .FirstOrDefaultAsync();

                if (zona == null)
                {
                    ModelState.AddModelError("", "❌ Erro: Não existe stock suficiente em nenhuma zona ativa para realizar este consumo.");

                    
                    ViewData["ConsumivelID"] = new SelectList(_context.Consumivel, "ConsumivelId", "Nome", usoConsumivel.ConsumivelId);
                    ViewData["TreatmentRecordId"] = new SelectList(_context.TreatmentRecord, "Id", "Id", usoConsumivel.TreatmentRecordId);
                    return View(usoConsumivel);
                }

                // 2. Configurar o Registo
                usoConsumivel.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                usoConsumivel.ZonaArmazenamentoID = zona.ZonaId; 

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
        public async Task<IActionResult> CreateMultiple(
            int treatmentRecordId,
            int[] ConsumivelID,
            int[] QuantidadeUsada,
            DateTime[] DataConsumo,
            int[] ZonaArmazenamentoID) 
        {
            // 1. VERIFICAÇÃO DE SEGURANÇA
            // Garante que os dados vieram alinhados (o mesmo nº de consumíveis, quantidades e zonas)
            if (ConsumivelID.Length != ZonaArmazenamentoID.Length || ConsumivelID.Length != QuantidadeUsada.Length)
            {
                TempData["ErrorMessage"] = "Erro no envio: Os dados estão desalinhados. Tente novamente.";
                return RedirectToAction("Details", "TreatmentRecords", new { id = treatmentRecordId });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<string> erros = new List<string>();
            for (int i = 0; i < ConsumivelID.Length; i++)
            {
                int qtd = QuantidadeUsada[i];
                int consumivelId = ConsumivelID[i];
                int zonaId = ZonaArmazenamentoID[i]; 
                // 2. BUSCAR A ZONA ESPECÍFICA              
                var zona = await _context.ZonaArmazenamento
                    .FirstOrDefaultAsync(z => z.ZonaId == zonaId);
                // --- VALIDAÇÕES ---

                // Se a zona não existe ou não corresponde ao consumível (segurança contra manipulação de HTML)
                if (zona == null || zona.ConsumivelId != consumivelId)
                {
                    erros.Add($"Linha {i + 1}: Zona inválida para o consumível selecionado.");
                    continue;
                }
                // Se a zona não tem stock suficiente
                if (qtd > zona.QuantidadeAtual)
                {
                    erros.Add($"Linha {i + 1} ({zona.NomeZona}): Tentou retirar {qtd}, mas só existem {zona.QuantidadeAtual}.");
                    continue;
                }
                // 3. CRIAR O REGISTO
                var uso = new UsoConsumivel
                {
                    TreatmentRecordId = treatmentRecordId,
                    ConsumivelId = consumivelId,
                    QuantidadeUsada = qtd,
                    DataConsumo = DataConsumo[i],
                    UserId = userId,
                    ZonaArmazenamentoID = zonaId 
                };
                _context.UsoConsumivel.Add(uso);
                // 4. ATUALIZAR STOCKS
                zona.QuantidadeAtual -= qtd;
                _context.Update(zona);

                var consumivelPai = await _context.Consumivel.FindAsync(consumivelId);
                if (consumivelPai != null)
                {
                    consumivelPai.QuantidadeAtual -= qtd;
                    _context.Update(consumivelPai);
                }
            }
            await _context.SaveChangesAsync();
            // Feedback ao utilizador
            if (erros.Count > 0)
            {
                TempData["ErrorMessage"] = "Atenção: " + string.Join(" | ", erros);
            }
            else
            {
                TempData["SuccessMessage"] = "Consumos registados com sucesso!";
            }

            return RedirectToAction(nameof(Index));
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