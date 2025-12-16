using Microsoft.AspNetCore.Mvc;

namespace HealthWellbeing.Controllers
{
    using global::HealthWellbeing.Data;
    using global::HealthWellbeing.Models;
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

            // =====================================================
            // INDEX — Página principal de Compras
            // =====================================================
            public IActionResult Index()
            {
                ViewBag.Consumiveis = _context.Consumivel.ToList();
                ViewBag.Zonas = _context.ZonaArmazenamento.ToList();

                return View(new Compra());

               
            }

            // =====================================================
            // SUBMETER COMPRA (POST)
            // =====================================================
            [HttpPost]
            [ValidateAntiForgeryToken]
            public IActionResult SubmeterCompra(Compra model)
            {
                if (!ModelState.IsValid)
                    return View("Index", model);

                // 🔹 Fornecedor TEMPORÁRIO (mais tarde automático via CompraOpcao)
                var fornecedor = _context.Fornecedor.FirstOrDefault();
                if (fornecedor == null)
                {
                    ModelState.AddModelError("", "Não existe fornecedor registado.");
                    return View("Index", model);
                }

                // 🔹 Criar Compra
                var compra = new Compra
                {
                    ConsumivelId = model.ConsumivelId,
                    ZonaId = model.ZonaId,
                    FornecedorId = fornecedor.FornecedorId,
                    Quantidade = model.Quantidade,
                    PrecoUnitario = 0,
                    TempoEntrega = 0,
                    DataCompra = DateTime.Now
                };

                _context.Compra.Add(compra);

                // 🔹 Atualizar Stock da Zona
                var stock = _context.Stock.FirstOrDefault(s =>
                    s.ConsumivelID == model.ConsumivelId &&
                    s.ZonaID == model.ZonaId);

                if (stock == null)
                {
                    ModelState.AddModelError("", "Stock não encontrado para a zona selecionada.");
                    return View("Index", model);
                }

                stock.QuantidadeAtual += model.Quantidade;
                stock.DataUltimaAtualizacao = DateTime.Now;

                // 🔹 (Opcional) Registo em StockMovimento — auditoria
                var movimento = new StockMovimento
                {
                    StockId = stock.StockId,
                    Quantidade = model.Quantidade,
                    Tipo = "Entrada",
                    Data = DateTime.Now,
                    Descricao = "Compra registada via módulo de compras"
                };

                _context.StockMovimento.Add(movimento);

                // 🔹 Guardar tudo
                _context.SaveChanges();

                return RedirectToAction(nameof(Historico));
            }

            // =====================================================
            // HISTÓRICO DE COMPRAS
            // =====================================================
            public IActionResult Historico()
            {
                var compras = _context.Compra
                    .Include(c => c.Consumivel)
                    .Include(c => c.Zona)
                    .Include(c => c.Fornecedor)
                    .OrderByDescending(c => c.DataCompra)
                    .ToList();

                return View(compras);
            }
        }
    }

}
