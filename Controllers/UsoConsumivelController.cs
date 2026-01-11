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

        public UsoConsumivelController(HealthWellbeingDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        private readonly UserManager<IdentityUser> _userManager;


        // GET: UsoConsumivel
        public async Task<IActionResult> Index()
        {
            var healthWellbeingDbContext = _context.UsoConsumivel.Include(u => u.Consumivel).Include(u => u.TreatmentRecord);
            return View(await healthWellbeingDbContext.ToListAsync());
        }

        // GET: UsoConsumivel/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usoConsumivel = await _context.UsoConsumivel
                .Include(u => u.Consumivel)
                .Include(u => u.TreatmentRecord)
                .FirstOrDefaultAsync(m => m.UsoConsumivelId == id);
            if (usoConsumivel == null)
            {
                return NotFound();
            }

            // Buscar o nome do user pelo UserId
            var user = await _userManager.FindByIdAsync(usoConsumivel.UserId);
            ViewBag.UserName = user?.UserName ?? "Desconhecido";

            return View(usoConsumivel);

        }



        // GET: UsoConsumivel/Create
        public IActionResult Create(int treatmentRecordId)
        {
            ViewData["ConsumivelID"] = new SelectList(_context.Consumivel, "ConsumivelId", "Nome");

            // Todas as zonas ativas
            var zonas = _context.ZonaArmazenamento
                                .Where(z => z.Ativa)
                                .Select(z => new {
                                    z.ZonaId,
                                    z.NomeZona,
                                    z.ConsumivelId
                                })
                                .ToList();

            // Passar para ViewBag como JSON para JS
            ViewBag.Zonas = Newtonsoft.Json.JsonConvert.SerializeObject(zonas);

            // envia o id para a view
            ViewBag.TreatmentRecordId = treatmentRecordId;

            return View();
        }

        // POST: UsoConsumivel/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UsoConsumivelId,TreatmentRecordId,ConsumivelID,QuantidadeUsada,DataConsumo")] UsoConsumivel usoConsumivel)
        {
            if (ModelState.IsValid)
            {
                // Pega o UserId do login atual
                usoConsumivel.UserId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);

                // Opcional: definir DataConsumo se quiseres automático
                usoConsumivel.DataConsumo = DateTime.Now;

                _context.Add(usoConsumivel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ConsumivelID"] = new SelectList(_context.Consumivel, "ConsumivelId", "Nome", usoConsumivel.ConsumivelId);
            ViewData["TreatmentRecordId"] = new SelectList(_context.TreatmentRecord, "Id", "Id", usoConsumivel.TreatmentRecordId);
            return View(usoConsumivel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMultiple(int treatmentRecordId, int[] ConsumivelID, int[] QuantidadeUsada, DateTime[] DataConsumo)
        {
            var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);

            for (int i = 0; i < ConsumivelID.Length; i++)
            {
                // Seleciona a primeira zona ativa para o consumível
                var zona = await _context.ZonaArmazenamento
                                         .Where(z => z.ConsumivelId == ConsumivelID[i] && z.Ativa)
                                         .FirstOrDefaultAsync();

                if (zona == null)
                {
                    // Se não houver zona, podes escolher ignorar ou devolver erro
                    return BadRequest($"Não existe zona de armazenamento ativa para o consumível ID {ConsumivelID[i]}");
                }

                var uso = new UsoConsumivel
                {
                    TreatmentRecordId = treatmentRecordId,
                    ConsumivelId = ConsumivelID[i],
                    QuantidadeUsada = QuantidadeUsada[i],
                    DataConsumo = DataConsumo[i],
                    UserId = userId,
                    ZonaArmazenamentoID = zona.ZonaId
                };

                _context.UsoConsumivel.Add(uso);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "TreatmentRecords", new { id = treatmentRecordId });
        }
        // GET: UsoConsumivel/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usoConsumivel = await _context.UsoConsumivel.FindAsync(id);
            if (usoConsumivel == null)
            {
                return NotFound();
            }
            ViewData["ConsumivelId"] = new SelectList(_context.Consumivel, "ConsumivelId", "Nome", usoConsumivel.ConsumivelId);
            ViewData["TreatmentRecordId"] = new SelectList(_context.TreatmentRecord, "Id", "Id", usoConsumivel.TreatmentRecordId);
            return View(usoConsumivel);
        }

        // POST: UsoConsumivel/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UsoConsumivelId,TreatmentRecordId,ConsumivelID,QuantidadeUsada,DataConsumo")] UsoConsumivel usoConsumivel)
        {
            if (id != usoConsumivel.UsoConsumivelId)
            {
                return NotFound();
            }

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
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ConsumivelID"] = new SelectList(_context.Consumivel, "ConsumivelId", "Nome", usoConsumivel.ConsumivelId);
            ViewData["TreatmentRecordId"] = new SelectList(_context.TreatmentRecord, "Id", "Id", usoConsumivel.TreatmentRecordId);
            return View(usoConsumivel);
        }

        // GET: UsoConsumivel/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usoConsumivel = await _context.UsoConsumivel
                .Include(u => u.Consumivel)
                .Include(u => u.TreatmentRecord)
                .FirstOrDefaultAsync(m => m.UsoConsumivelId == id);
            if (usoConsumivel == null)
            {
                return NotFound();
            }

            return View(usoConsumivel);
        }

        // POST: UsoConsumivel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usoConsumivel = await _context.UsoConsumivel.FindAsync(id);
            if (usoConsumivel != null)
            {
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
