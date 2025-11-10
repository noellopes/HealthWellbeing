using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class ConsumivelController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ConsumivelController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Consumivel
        public async Task<IActionResult> Index()
        {
            // Se a tabela de consumíveis estiver vazia, insere registos fictícios automaticamente
            if (!_context.Consumivel.Any())
            {
                var consumiveis = new List<Consumivel>
                {
                    new Consumivel { Nome = "Luva Cirúrgica Estéril", Descricao = "Luva estéril de látex para procedimentos cirúrgicos.", CategoriaId = 1 },
                    new Consumivel { Nome = "Máscara N95", Descricao = "Máscara respiratória de alta filtragem para uso hospitalar.", CategoriaId = 2 },
                    new Consumivel { Nome = "Seringa de 10ml", Descricao = "Seringa descartável com graduação precisa.", CategoriaId = 3 },
                    new Consumivel { Nome = "Cateter Venoso Periférico", Descricao = "Cateter para acesso venoso em terapias intravenosas.", CategoriaId = 4 },
                    new Consumivel { Nome = "Compressa Estéril 10x10", Descricao = "Compressa cirúrgica esterilizada para curativos.", CategoriaId = 5 },
                    new Consumivel { Nome = "Gaze Hidrófila", Descricao = "Rolo de gaze estéril para curativos e assepsia.", CategoriaId = 6 },
                    new Consumivel { Nome = "Bandagem Elástica 10cm", Descricao = "Faixa de compressão para imobilização ou suporte.", CategoriaId = 7 },
                    new Consumivel { Nome = "Micropore 25mm", Descricao = "Fita adesiva médica hipoalergénica para fixação de pensos.", CategoriaId = 8 },
                    new Consumivel { Nome = "Soro Fisiológico 0.9% 500ml", Descricao = "Solução estéril para irrigação e diluição de medicamentos.", CategoriaId = 9 },
                    new Consumivel { Nome = "Álcool Etílico 70%", Descricao = "Desinfetante hospitalar para superfícies e antissepsia.", CategoriaId = 10 },
                    new Consumivel { Nome = "Tubo de Aspiração 40cm", Descricao = "Tubo flexível para aspiração de secreções.", CategoriaId = 11 },
                    new Consumivel { Nome = "Máscara de Oxigénio Adulto", Descricao = "Máscara anatómica para oxigenoterapia.", CategoriaId = 12 },
                    new Consumivel { Nome = "Equipo de Infusão Simples", Descricao = "Equipamento para administração de soluções intravenosas.", CategoriaId = 13 },
                    new Consumivel { Nome = "Kit de Curativo Estéril", Descricao = "Conjunto com gaze, pinça e campo estéril.", CategoriaId = 14 },
                    new Consumivel { Nome = "Agulha de Punção 21G", Descricao = "Agulha hipodérmica estéril para punções venosas.", CategoriaId = 15 },
                    new Consumivel { Nome = "Lâmina de Bisturi nº 11", Descricao = "Lâmina cirúrgica esterilizada para incisões precisas.", CategoriaId = 16 },
                    new Consumivel { Nome = "Campo Cirúrgico Estéril 100x150", Descricao = "Cobertura cirúrgica para procedimentos invasivos.", CategoriaId = 17 },
                    new Consumivel { Nome = "Touca Descartável Azul", Descricao = "Touca para proteção capilar em ambientes estéreis.", CategoriaId = 18 },
                    new Consumivel { Nome = "Indicador Químico de Esterilização", Descricao = "Fita indicadora de mudança de cor em autoclave.", CategoriaId = 19 },
                    new Consumivel { Nome = "Frasco Coletor de Urina 100ml", Descricao = "Recipiente estéril para coleta de amostras biológicas.", CategoriaId = 20 }
                };

                _context.Consumivel.AddRange(consumiveis);
                await _context.SaveChangesAsync();
            }

            var healthWellbeingDbContext = _context.Consumivel.Include(c => c.CategoriaConsumivel);
            return View(await healthWellbeingDbContext.ToListAsync());
        }

        // GET: Consumivel/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consumivel = await _context.Consumivel
                .Include(c => c.CategoriaConsumivel)
                .FirstOrDefaultAsync(m => m.ConsumivelId == id);
            if (consumivel == null)
            {
                return NotFound();
            }

            return View(consumivel);
        }

        // GET: Consumivel/Create
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.CategoriaConsumivel, "CategoriaId", "Nome");
            return View();
        }

        // POST: Consumivel/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ConsumivelId,Nome,Descricao,CategoriaId")] Consumivel consumivel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(consumivel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.CategoriaConsumivel, "CategoriaId", "Nome", consumivel.CategoriaId);
            return View(consumivel);
        }

        // GET: Consumivel/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consumivel = await _context.Consumivel.FindAsync(id);
            if (consumivel == null)
            {
                return NotFound();
            }
            ViewData["CategoriaId"] = new SelectList(_context.CategoriaConsumivel, "CategoriaId", "Nome", consumivel.CategoriaId);
            return View(consumivel);
        }

        // POST: Consumivel/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ConsumivelId,Nome,Descricao,CategoriaId")] Consumivel consumivel)
        {
            if (id != consumivel.ConsumivelId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(consumivel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConsumivelExists(consumivel.ConsumivelId))
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
            ViewData["CategoriaId"] = new SelectList(_context.CategoriaConsumivel, "CategoriaId", "Nome", consumivel.CategoriaId);
            return View(consumivel);
        }

        // GET: Consumivel/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consumivel = await _context.Consumivel
                .Include(c => c.CategoriaConsumivel)
                .FirstOrDefaultAsync(m => m.ConsumivelId == id);
            if (consumivel == null)
            {
                return NotFound();
            }

            return View(consumivel);
        }

        // POST: Consumivel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var consumivel = await _context.Consumivel.FindAsync(id);
            if (consumivel != null)
            {
                _context.Consumivel.Remove(consumivel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ConsumivelExists(int id)
        {
            return _context.Consumivel.Any(e => e.ConsumivelId == id);
        }
    }
}
