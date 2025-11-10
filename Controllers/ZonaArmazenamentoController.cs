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
            // Se a tabela estiver vazia, insere 20 registos fictícios automaticamente
            if (!_context.ZonaArmazenamento.Any())
            {
                var zonas = new List<ZonaArmazenamento>
                {
                    new ZonaArmazenamento { Nome = "Armazém Central", Descricao = "Zona principal de armazenamento geral", Localizacao = "Bloco A - Piso 0", CapacidadeMaxima = 5000 },
                    new ZonaArmazenamento { Nome = "Depósito Norte", Descricao = "Materiais não perecíveis", Localizacao = "Bloco B - Norte", CapacidadeMaxima = 2500 },
                    new ZonaArmazenamento { Nome = "Sala de Higienização", Descricao = "Produtos de limpeza", Localizacao = "Bloco C - Piso 0", CapacidadeMaxima = 1200 },
                    new ZonaArmazenamento { Nome = "Farmácia Interna", Descricao = "Medicamentos e material médico", Localizacao = "Bloco D - Piso 1", CapacidadeMaxima = 3000 },
                    new ZonaArmazenamento { Nome = "Zona Frigorífica", Descricao = "Armazenamento refrigerado", Localizacao = "Subsolo - Bloco E", CapacidadeMaxima = 800 },
                    new ZonaArmazenamento { Nome = "Depósito Sul", Descricao = "Materiais e ferramentas", Localizacao = "Bloco F - Sul", CapacidadeMaxima = 4000 },
                    new ZonaArmazenamento { Nome = "Sala de Emergência", Descricao = "Equipamentos de primeiros socorros", Localizacao = "Bloco A - Piso 1", CapacidadeMaxima = 600 },
                    new ZonaArmazenamento { Nome = "Depósito Oeste", Descricao = "Materiais de escritório", Localizacao = "Bloco G - Oeste", CapacidadeMaxima = 1000 },
                    new ZonaArmazenamento { Nome = "Zona Técnica", Descricao = "Peças e componentes técnicos", Localizacao = "Oficina - Bloco H", CapacidadeMaxima = 2000 },
                    new ZonaArmazenamento { Nome = "Armazém Exterior", Descricao = "Materiais volumosos", Localizacao = "Exterior - Norte", CapacidadeMaxima = 7000 },
                    new ZonaArmazenamento { Nome = "Depósito de Resíduos", Descricao = "Zona controlada para resíduos", Localizacao = "Bloco I - Lateral", CapacidadeMaxima = 500 },
                    new ZonaArmazenamento { Nome = "Sala de Esterilização", Descricao = "Materiais esterilizados", Localizacao = "Bloco D - Piso 2", CapacidadeMaxima = 900 },
                    new ZonaArmazenamento { Nome = "Armazém Hospitalar", Descricao = "Materiais médicos de uso diário", Localizacao = "Bloco H - Piso 1", CapacidadeMaxima = 3500 },
                    new ZonaArmazenamento { Nome = "Depósito de Emergência", Descricao = "Equipamentos para emergências", Localizacao = "Bloco B - Subsolo", CapacidadeMaxima = 1500 },
                    new ZonaArmazenamento { Nome = "Zona de Equipamentos Pesados", Descricao = "Máquinas e equipamentos grandes", Localizacao = "Bloco F - Piso -1", CapacidadeMaxima = 6000 },
                    new ZonaArmazenamento { Nome = "Sala de Armazenamento Temporário", Descricao = "Materiais em trânsito ou devolução", Localizacao = "Bloco J - Piso 0", CapacidadeMaxima = 1800 },
                    new ZonaArmazenamento { Nome = "Depósito de Segurança", Descricao = "Equipamentos de proteção individual", Localizacao = "Bloco E - Piso 1", CapacidadeMaxima = 900 },
                    new ZonaArmazenamento { Nome = "Armazém de Consumíveis Médicos", Descricao = "Luvas, seringas e consumíveis", Localizacao = "Bloco C - Piso 1", CapacidadeMaxima = 2200 },
                    new ZonaArmazenamento { Nome = "Zona de Calibração", Descricao = "Instrumentos calibrados", Localizacao = "Bloco G - Piso 2", CapacidadeMaxima = 1300 },
                    new ZonaArmazenamento { Nome = "Zona Experimental", Descricao = "Espaço para testes e protótipos", Localizacao = "Bloco J - Piso 1", CapacidadeMaxima = 1600 }
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
        public IActionResult Create()
        {
            return View();
        }

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
