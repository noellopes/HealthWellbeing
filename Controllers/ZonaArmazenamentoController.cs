using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class ZonaArmazenamentoController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ZonaArmazenamentoController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: ZonaArmazenamento
        public async Task<IActionResult> Index()
        {
            // Se a tabela estiver vazia, insere registos fictícios automaticamente
            if (!_context.ZonaArmazenamento.Any())
            {
                var zonas = new List<ZonaArmazenamento>
                {
                    new ZonaArmazenamento { Nome = "Armazém Central Hospitalar", Descricao = "Zona principal de armazenamento de equipamentos médicos e material clínico", Localizacao = "Bloco A - Piso 0", CapacidadeMaxima = 5000 },
                    new ZonaArmazenamento { Nome = "Depósito Norte Clínico", Descricao = "Armazenamento de material descartável e consumíveis", Localizacao = "Bloco B - Norte", CapacidadeMaxima = 2500 },
                    new ZonaArmazenamento { Nome = "Sala de Higienização Hospitalar", Descricao = "Produtos de limpeza e desinfeção hospitalar", Localizacao = "Bloco C - Piso 0", CapacidadeMaxima = 1200 },
                    new ZonaArmazenamento { Nome = "Farmácia Interna", Descricao = "Medicamentos, vacinas e antibióticos controlados", Localizacao = "Bloco D - Piso 1", CapacidadeMaxima = 3000 },
                    new ZonaArmazenamento { Nome = "Zona Frigorífica Médica", Descricao = "Armazenamento refrigerado de fármacos sensíveis", Localizacao = "Subsolo - Bloco E", CapacidadeMaxima = 800 },
                    new ZonaArmazenamento { Nome = "Depósito de Instrumentos Cirúrgicos", Descricao = "Instrumentos e material cirúrgico esterilizado", Localizacao = "Bloco F - Sul", CapacidadeMaxima = 4000 },
                    new ZonaArmazenamento { Nome = "Sala de Emergência Médica", Descricao = "Equipamentos de resposta rápida a emergências", Localizacao = "Bloco A - Piso 1", CapacidadeMaxima = 600 },
                    new ZonaArmazenamento { Nome = "Depósito de Material de Enfermagem", Descricao = "Luvas, gazes, seringas e kits de primeiros socorros", Localizacao = "Bloco G - Oeste", CapacidadeMaxima = 1000 },
                    new ZonaArmazenamento { Nome = "Zona Técnica de Manutenção Hospitalar", Descricao = "Equipamentos e ferramentas de manutenção hospitalar", Localizacao = "Oficina - Bloco H", CapacidadeMaxima = 2000 },
                    new ZonaArmazenamento { Nome = "Armazém de Oxigénio e Gases Médicos", Descricao = "Cilindros e equipamentos de ventilação", Localizacao = "Exterior - Norte", CapacidadeMaxima = 700 }
                };

                _context.ZonaArmazenamento.AddRange(zonas);
                await _context.SaveChangesAsync();
            }

            return View(await _context.ZonaArmazenamento.ToListAsync());
        }

        // GET: ZonaArmazenamento/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var zonaArmazenamento = await _context.ZonaArmazenamento
                .FirstOrDefaultAsync(m => m.Id == id);

            if (zonaArmazenamento == null)
                return NotFound();

            return View(zonaArmazenamento);
        }

        // GET: ZonaArmazenamento/Create
        public IActionResult Create() => View();

        // POST: ZonaArmazenamento/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Descricao,Localizacao,CapacidadeMaxima,Ativa")] ZonaArmazenamento zonaArmazenamento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(zonaArmazenamento);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "✅ Zona criada com sucesso!";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "❌ Erro ao criar a zona. Verifique os campos e tente novamente.";
            return View(zonaArmazenamento);
        }

        // GET: ZonaArmazenamento/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var zonaArmazenamento = await _context.ZonaArmazenamento.FindAsync(id);
            if (zonaArmazenamento == null)
                return NotFound();

            return View(zonaArmazenamento);
        }

        // POST: ZonaArmazenamento/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Descricao,Localizacao,CapacidadeMaxima,Ativa")] ZonaArmazenamento zonaArmazenamento)
        {
            if (id != zonaArmazenamento.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(zonaArmazenamento);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "💾 Alterações guardadas com sucesso!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ZonaArmazenamentoExists(zonaArmazenamento.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "❌ Erro ao editar a zona. Verifique os campos e tente novamente.";
            return View(zonaArmazenamento);
        }

        // GET: ZonaArmazenamento/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var zonaArmazenamento = await _context.ZonaArmazenamento
                .FirstOrDefaultAsync(m => m.Id == id);

            if (zonaArmazenamento == null)
                return NotFound();

            return View(zonaArmazenamento);
        }

        // POST: ZonaArmazenamento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var zonaArmazenamento = await _context.ZonaArmazenamento.FindAsync(id);
            if (zonaArmazenamento != null)
            {
                _context.ZonaArmazenamento.Remove(zonaArmazenamento);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "🗑️ Zona eliminada com sucesso!";
            }
            else
            {
                TempData["ErrorMessage"] = "❌ Erro ao eliminar a zona. Ela pode já ter sido removida.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ZonaArmazenamentoExists(int id)
        {
            return _context.ZonaArmazenamento.Any(e => e.Id == id);
        }
    }
}
