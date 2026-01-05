using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Controllers
{
    public class CompraController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public CompraController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index(int? consumivelId)
        {
            var consumiveis = _context.Consumivel.ToList();

            if (consumivelId == null)
            {
                ViewBag.Consumiveis = consumiveis;
                return View();
            }

            var consumivel = _context.Consumivel
                .FirstOrDefault(c => c.ConsumivelId == consumivelId);

            if (consumivel == null)
                return NotFound();

            ViewBag.Consumiveis = consumiveis;

            ViewBag.QuantidadeAtual = consumivel.QuantidadeAtual;
            ViewBag.QuantidadeMinima = consumivel.QuantidadeMinima;
            ViewBag.QuantidadeMaxima = consumivel.QuantidadeMaxima;

            ViewBag.QuantidadeSugerida =
                consumivel.QuantidadeMaxima - consumivel.QuantidadeAtual;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CriarRegistoCompra(int consumivelId, int quantidade)
        {
            if (consumivelId == 0 || quantidade <= 0)
            {
                ViewBag.Consumiveis = _context.Consumivel.ToList();
                return View("Index");
            }

            return RedirectToAction("RegistoCompra", new { consumivelId, quantidade });
        }

        [HttpGet]
        public IActionResult RegistoCompra(int consumivelId, int quantidade)
        {
            var fornecedores = _context.Fornecedor_Consumivel
                .Where(fc => fc.ConsumivelId == consumivelId)
                .Include(fc => fc.Fornecedor)
                .OrderBy(fc => fc.Preco)
                .ThenBy(fc => fc.TempoEntrega)
                .ToList();

            var vm = new RegistoCompra
            {
                ConsumivelId = consumivelId,
                Quantidade = quantidade,
                Fornecedores = fornecedores
            };

            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmarRegisto(RegistoCompra model)
        {
            if (!ModelState.IsValid)
            {
                // Recarregar fornecedores caso haja erro
                model.Fornecedores = _context.Fornecedor_Consumivel
                    .Where(fc => fc.ConsumivelId == model.ConsumivelId)
                    .Include(fc => fc.Fornecedor)
                    .OrderBy(fc => fc.Preco)
                    .ThenBy(fc => fc.TempoEntrega)
                    .ToList();

                return View("RegistoCompra", model);
            }

            // 🔹 Consumível (fonte única de verdade)
            var consumivel = _context.Consumivel
                .FirstOrDefault(c => c.ConsumivelId == model.ConsumivelId);

            if (consumivel == null)
                return NotFound();

            int limitePermitido = consumivel.QuantidadeMaxima - consumivel.QuantidadeAtual;

            if (model.Quantidade > limitePermitido)
            {
                ModelState.AddModelError(
                    "Quantidade",
                    $"Não é possível comprar mais de {limitePermitido} unidades."
                );

                model.Fornecedores = _context.Fornecedor_Consumivel
                    .Where(fc => fc.ConsumivelId == model.ConsumivelId)
                    .Include(fc => fc.Fornecedor)
                    .OrderBy(fc => fc.Preco)
                    .ThenBy(fc => fc.TempoEntrega)
                    .ToList();

                return View("RegistoCompra", model);
            }

            // 🔹 Fornecedor selecionado
            var fornecedor = _context.Fornecedor_Consumivel
                .Include(fc => fc.Fornecedor)
                .First(fc =>
                    fc.FornecedorId == model.FornecedorId &&
                    fc.ConsumivelId == model.ConsumivelId
                );

            // 🟢 1 — Registar COMPRA
            var compra = new Compra
            {
                ConsumivelId = model.ConsumivelId,
                FornecedorId = fornecedor.FornecedorId,
                Quantidade = model.Quantidade,
                PrecoUnitario = fornecedor.Preco,
                TempoEntrega = fornecedor.TempoEntrega ?? 0,
                DataCompra = DateTime.Now
            };
            
            _context.Compra.Add(compra);

            // 🔹 Obter o stock associado ao consumível
            var stock = _context.Stock
                .FirstOrDefault(s => s.ConsumivelID == model.ConsumivelId);

            if (stock == null)
                return BadRequest("Stock não encontrado para este consumível.");

            _context.HistoricoCompras.Add(new HistoricoCompras
            {
                StockId = stock.StockId,          // 🔗 ligação correta
                Quantidade = model.Quantidade,
                FornecedorId = fornecedor.FornecedorId, // opcional mas recomendado
                Tipo = "Entrada",
                Data = DateTime.Now,
                
            });


            // 🟢 2 — Atualizar APENAS o Consumível
            consumivel.QuantidadeAtual += model.Quantidade;

            _context.SaveChanges();

            // ✅ 3 — Ir para o HISTÓRICO DE COMPRAS
            return RedirectToAction("Index", "HistoricoCompras");
        }



        public IActionResult Historico()
        {
            return View(_context.Compra
                .Include(c => c.Consumivel)
                .Include(c => c.Fornecedor)
                .ToList());
        }
    }
}
