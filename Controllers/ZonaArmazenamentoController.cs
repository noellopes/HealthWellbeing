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
        public async Task<IActionResult> Index(string searchNome = "", string searchLocalizacao = "")
        {
            // Criar 60 zonas fictícias se a base de dados estiver vazia
            if (!_context.ZonaArmazenamento.Any())
            {
                var zonas = new List<ZonaArmazenamento>
                {
                    new ZonaArmazenamento { Nome = "Armazém Central Hospitalar", Descricao = "Equipamentos médicos e material clínico", Localizacao = "Bloco A - Piso 0", CapacidadeMaxima = 5000 },
                    new ZonaArmazenamento { Nome = "Depósito Norte Clínico", Descricao = "Material descartável e consumíveis", Localizacao = "Bloco B - Norte", CapacidadeMaxima = 2500 },
                    new ZonaArmazenamento { Nome = "Farmácia Interna", Descricao = "Medicamentos e vacinas", Localizacao = "Bloco D - Piso 1", CapacidadeMaxima = 3000 },
                    new ZonaArmazenamento { Nome = "Zona Frigorífica Médica", Descricao = "Armazenamento refrigerado de fármacos", Localizacao = "Subsolo - Bloco E", CapacidadeMaxima = 800 },
                    new ZonaArmazenamento { Nome = "Depósito Sul Hospitalar", Descricao = "Ferramentas e manutenção", Localizacao = "Bloco F - Sul", CapacidadeMaxima = 4000 },
                    new ZonaArmazenamento { Nome = "Sala de Emergência Médica", Descricao = "Equipamentos de primeiros socorros", Localizacao = "Bloco A - Piso 1", CapacidadeMaxima = 600 },
                    new ZonaArmazenamento { Nome = "Depósito Oeste", Descricao = "Material administrativo e de escritório", Localizacao = "Bloco G - Oeste", CapacidadeMaxima = 1000 },
                    new ZonaArmazenamento { Nome = "Zona Técnica de Manutenção", Descricao = "Peças e componentes técnicos", Localizacao = "Oficina - Bloco H", CapacidadeMaxima = 2000 },
                    new ZonaArmazenamento { Nome = "Armazém de Oxigénio e Gases Médicos", Descricao = "Cilindros e ventiladores", Localizacao = "Exterior - Norte", CapacidadeMaxima = 700 },
                    new ZonaArmazenamento { Nome = "Depósito de Resíduos Biológicos", Descricao = "Resíduos hospitalares controlados", Localizacao = "Bloco I - Lateral", CapacidadeMaxima = 500 },
                    new ZonaArmazenamento { Nome = "Sala de Esterilização Central", Descricao = "Materiais esterilizados", Localizacao = "Bloco D - Piso 2", CapacidadeMaxima = 900 },
                    new ZonaArmazenamento { Nome = "Armazém de Nutrição Clínica", Descricao = "Suplementos alimentares", Localizacao = "Bloco H - Piso 1", CapacidadeMaxima = 3500 },
                    new ZonaArmazenamento { Nome = "Depósito de Emergência Sanitária", Descricao = "Kits e material de crise", Localizacao = "Bloco B - Subsolo", CapacidadeMaxima = 1500 },
                    new ZonaArmazenamento { Nome = "Zona de Equipamentos Pesados", Descricao = "Máquinas médicas de grande porte", Localizacao = "Bloco F - Piso -1", CapacidadeMaxima = 6000 },
                    new ZonaArmazenamento { Nome = "Sala Temporária", Descricao = "Materiais em trânsito", Localizacao = "Bloco J - Piso 0", CapacidadeMaxima = 1800 },
                    new ZonaArmazenamento { Nome = "Depósito de EPI", Descricao = "Máscaras e luvas", Localizacao = "Bloco E - Piso 1", CapacidadeMaxima = 900 },
                    new ZonaArmazenamento { Nome = "Armazém de Consumíveis Médicos", Descricao = "Luvas e seringas", Localizacao = "Bloco C - Piso 1", CapacidadeMaxima = 2200 },
                    new ZonaArmazenamento { Nome = "Zona de Calibração", Descricao = "Instrumentos calibrados", Localizacao = "Bloco G - Piso 2", CapacidadeMaxima = 1300 },
                    new ZonaArmazenamento { Nome = "Zona Experimental", Descricao = "Testes laboratoriais", Localizacao = "Bloco J - Piso 1", CapacidadeMaxima = 1600 },
                    new ZonaArmazenamento { Nome = "Depósito de Fisioterapia", Descricao = "Equipamentos de reabilitação", Localizacao = "Bloco K - Piso 1", CapacidadeMaxima = 1800 },
                    new ZonaArmazenamento { Nome = "Armazém de Laboratório", Descricao = "Reagentes e material de análises", Localizacao = "Bloco L - Piso 2", CapacidadeMaxima = 1000 },
                    new ZonaArmazenamento { Nome = "Zona de Radiologia", Descricao = "Componentes radiológicos", Localizacao = "Bloco M - Piso 0", CapacidadeMaxima = 2500 },
                    new ZonaArmazenamento { Nome = "Depósito de Diagnóstico", Descricao = "Materiais de diagnóstico rápido", Localizacao = "Bloco N - Piso 1", CapacidadeMaxima = 1900 },
                    new ZonaArmazenamento { Nome = "Armazém de Higiene Pessoal", Descricao = "Produtos de bem-estar", Localizacao = "Bloco O - Piso 0", CapacidadeMaxima = 1200 },
                    new ZonaArmazenamento { Nome = "Depósito de Vacinas", Descricao = "Armazenamento controlado", Localizacao = "Bloco P - Frigorífico Médico", CapacidadeMaxima = 800 },
                    new ZonaArmazenamento { Nome = "Sala Cirúrgica Preparatória", Descricao = "Materiais para operações", Localizacao = "Bloco Q - Piso 2", CapacidadeMaxima = 1500 },
                    new ZonaArmazenamento { Nome = "Armazém de Roupa Hospitalar", Descricao = "Uniformes e lençóis", Localizacao = "Bloco R - Piso -1", CapacidadeMaxima = 2000 },
                    new ZonaArmazenamento { Nome = "Depósito de Produtos Naturais", Descricao = "Óleos e ervas medicinais", Localizacao = "Bloco S - Piso 1", CapacidadeMaxima = 900 },
                    new ZonaArmazenamento { Nome = "Armazém de Psicologia Clínica", Descricao = "Material terapêutico", Localizacao = "Bloco T - Piso 0", CapacidadeMaxima = 800 },
                    new ZonaArmazenamento { Nome = "Zona de Cuidados Paliativos", Descricao = "Equipamentos de suporte", Localizacao = "Bloco U - Piso 1", CapacidadeMaxima = 1000 },
                    new ZonaArmazenamento { Nome = "Depósito Cardiológico", Descricao = "Monitores e desfibriladores", Localizacao = "Bloco V - Piso 2", CapacidadeMaxima = 1800 },
                    new ZonaArmazenamento { Nome = "Armazém Pediátrico", Descricao = "Brinquedos e consumíveis infantis", Localizacao = "Bloco W - Pediatria", CapacidadeMaxima = 1500 },
                    new ZonaArmazenamento { Nome = "Depósito de Ginecologia", Descricao = "Instrumentos ginecológicos", Localizacao = "Bloco X - Piso 1", CapacidadeMaxima = 1300 },
                    new ZonaArmazenamento { Nome = "Armazém de Ortodontia", Descricao = "Materiais dentários", Localizacao = "Bloco Y - Piso 1", CapacidadeMaxima = 1000 },
                    new ZonaArmazenamento { Nome = "Depósito Neurológico", Descricao = "EEG e estimulação cerebral", Localizacao = "Bloco Z - Piso 2", CapacidadeMaxima = 1500 },
                    new ZonaArmazenamento { Nome = "Zona de Terapias Alternativas", Descricao = "Acupuntura e aromaterapia", Localizacao = "Bloco AA - Piso 0", CapacidadeMaxima = 800 },
                    new ZonaArmazenamento { Nome = "Depósito de Dietética", Descricao = "Produtos alimentares clínicos", Localizacao = "Bloco AB - Cozinha Hospitalar", CapacidadeMaxima = 2000 },
                    new ZonaArmazenamento { Nome = "Armazém de Cuidados Intensivos", Descricao = "Equipamentos de suporte vital", Localizacao = "Bloco AC - UCI", CapacidadeMaxima = 2200 },
                    new ZonaArmazenamento { Nome = "Depósito de Ortopedia", Descricao = "Talas e próteses", Localizacao = "Bloco AD - Piso 0", CapacidadeMaxima = 1600 },
                    new ZonaArmazenamento { Nome = "Zona de Cuidados Domiciliários", Descricao = "Apoio a pacientes em casa", Localizacao = "Bloco AE - Piso 1", CapacidadeMaxima = 1700 },
                    new ZonaArmazenamento { Nome = "Depósito Oftalmológico", Descricao = "Lentes e equipamentos visuais", Localizacao = "Bloco AF - Piso 2", CapacidadeMaxima = 900 },
                    new ZonaArmazenamento { Nome = "Armazém de Reabilitação", Descricao = "Equipamento de treino motor", Localizacao = "Bloco AG - Fisioterapia", CapacidadeMaxima = 2000 },
                    new ZonaArmazenamento { Nome = "Depósito Auditivo", Descricao = "Aparelhos auditivos", Localizacao = "Bloco AH - Piso 1", CapacidadeMaxima = 600 },
                    new ZonaArmazenamento { Nome = "Zona Neonatal", Descricao = "Equipamentos para recém-nascidos", Localizacao = "Bloco AI - Maternidade", CapacidadeMaxima = 1400 },
                    new ZonaArmazenamento { Nome = "Depósito de Saúde Mental", Descricao = "Material de terapia ocupacional", Localizacao = "Bloco AJ - Piso 2", CapacidadeMaxima = 1000 },
                    new ZonaArmazenamento { Nome = "Armazém de Sangue", Descricao = "Equipamentos de transfusão", Localizacao = "Bloco AK - Hemoterapia", CapacidadeMaxima = 800 },
                    new ZonaArmazenamento { Nome = "Depósito de Primeiros Socorros", Descricao = "Kits de emergência", Localizacao = "Bloco AL - Piso 0", CapacidadeMaxima = 1200 },
                    new ZonaArmazenamento { Nome = "Zona de Cuidados Respiratórios", Descricao = "Nebulizadores e oxigénio", Localizacao = "Bloco AM - Piso 1", CapacidadeMaxima = 1900 },
                    new ZonaArmazenamento { Nome = "Armazém de Dermatologia", Descricao = "Produtos para cuidados da pele", Localizacao = "Bloco AN - Piso 2", CapacidadeMaxima = 1300 },
                    new ZonaArmazenamento { Nome = "Depósito Odontológico", Descricao = "Consumíveis dentários", Localizacao = "Bloco AO - Odontologia", CapacidadeMaxima = 1000 },
                    new ZonaArmazenamento { Nome = "Zona Terapia Ocupacional", Descricao = "Materiais de reabilitação", Localizacao = "Bloco AP - Piso 1", CapacidadeMaxima = 1100 },
                    new ZonaArmazenamento { Nome = "Armazém Geriátrico", Descricao = "Equipamentos de apoio a idosos", Localizacao = "Bloco AQ - Piso 0", CapacidadeMaxima = 2100 },
                    new ZonaArmazenamento { Nome = "Depósito Laboratorial", Descricao = "Microscópios e reagentes", Localizacao = "Bloco AR - Piso 2", CapacidadeMaxima = 1500 },
                    new ZonaArmazenamento { Nome = "Armazém de Diagnóstico", Descricao = "Ecógrafos e monitores", Localizacao = "Bloco AS - Diagnóstico", CapacidadeMaxima = 2500 },
                    new ZonaArmazenamento { Nome = "Zona Reabilitação Física", Descricao = "Aparelhos de fisioterapia", Localizacao = "Bloco AT - Fisiatria", CapacidadeMaxima = 2000 },
                    new ZonaArmazenamento { Nome = "Depósito Psicológico", Descricao = "Material de terapia cognitiva", Localizacao = "Bloco AU - Psicologia", CapacidadeMaxima = 900 },
                    new ZonaArmazenamento { Nome = "Armazém Hospitalar Avançado", Descricao = "Monitores e desfibriladores", Localizacao = "Bloco AV - UCI", CapacidadeMaxima = 2800 },
                    new ZonaArmazenamento { Nome = "Zona Cuidados Integrados", Descricao = "Equipamentos de múltiplas terapias", Localizacao = "Bloco AW - Piso 0", CapacidadeMaxima = 2200 },
                };

                _context.ZonaArmazenamento.AddRange(zonas);
                await _context.SaveChangesAsync();
            }

            // Pesquisa por nome/localização
            var zonasQuery = _context.ZonaArmazenamento.AsQueryable();

            if (!string.IsNullOrEmpty(searchNome))
                zonasQuery = zonasQuery.Where(z => z.Nome.Contains(searchNome));

            if (!string.IsNullOrEmpty(searchLocalizacao))
                zonasQuery = zonasQuery.Where(z => z.Localizacao.Contains(searchLocalizacao));

            ViewBag.SearchNome = searchNome;
            ViewBag.SearchLocalizacao = searchLocalizacao;

            return View(await zonasQuery.ToListAsync());
        }

        // GET: ZonaArmazenamento/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var zona = await _context.ZonaArmazenamento.FirstOrDefaultAsync(m => m.Id == id);
            if (zona == null)
                return NotFound();

            return View(zona);
        }

        // GET: Create
        public IActionResult Create() => View();

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Descricao,Localizacao,CapacidadeMaxima,Ativa")] ZonaArmazenamento zona)
        {
            if (ModelState.IsValid)
            {
                _context.Add(zona);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "✅ Zona criada com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "❌ Erro ao criar a zona.";
            return View(zona);
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var zona = await _context.ZonaArmazenamento.FindAsync(id);
            if (zona == null)
                return NotFound();

            return View(zona);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Descricao,Localizacao,CapacidadeMaxima,Ativa")] ZonaArmazenamento zona)
        {
            if (id != zona.Id)
                return NotFound();

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
                    if (!_context.ZonaArmazenamento.Any(e => e.Id == id))
                        return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "❌ Erro ao editar a zona.";
            return View(zona);
        }

        // GET: Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var zona = await _context.ZonaArmazenamento.FirstOrDefaultAsync(m => m.Id == id);
            if (zona == null)
                return NotFound();

            return View(zona);
        }

        // POST: Delete
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
