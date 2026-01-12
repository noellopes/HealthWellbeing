using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.Models.ViewModels;
using HealthWellbeingRoom.Models;
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

        // INDEX
        
        [HttpGet]
        public IActionResult Index(int? consumivelId)
        {
            var consumiveis = _context.Consumivel.ToList();

            ViewBag.Consumiveis = consumiveis;

            if (consumivelId == null)
                return View();

            var consumivel = _context.Consumivel
                .FirstOrDefault(c => c.ConsumivelId == consumivelId);

            if (consumivel == null)
                return NotFound();

            ViewBag.QuantidadeAtual = consumivel.QuantidadeAtual;
            ViewBag.QuantidadeMinima = consumivel.QuantidadeMinima;
            ViewBag.QuantidadeMaxima = consumivel.QuantidadeMaxima;

            ViewBag.QuantidadeSugerida =
                consumivel.QuantidadeMaxima - consumivel.QuantidadeAtual;

            ViewBag.ConsumivelSelecionadoId = consumivelId;

            return View();
        }

        // CRIAR REGISTO 
        
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

        // ESCOLHA DE FORNECEDOR
        
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

        // CONFIRMAR COMPRA

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmarRegisto(RegistoCompra model)
        {
            if (!ModelState.IsValid)
            {
                model.Fornecedores = _context.Fornecedor_Consumivel
                    .Where(fc => fc.ConsumivelId == model.ConsumivelId)
                    .Include(fc => fc.Fornecedor)
                    .OrderBy(fc => fc.Preco)
                    .ThenBy(fc => fc.TempoEntrega)
                    .ToList();

                return View("RegistoCompra", model);
            }

            // 🔹 Consumível
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

            // 🔹 Fornecedor
            var fornecedor = _context.Fornecedor_Consumivel
                .Include(fc => fc.Fornecedor)
                .First(fc =>
                    fc.FornecedorId == model.FornecedorId &&
                    fc.ConsumivelId == model.ConsumivelId
                );

            // 🔹 Registar COMPRA
            var compra = new Compra
            {
                ConsumivelId = model.ConsumivelId,
                FornecedorId = fornecedor.FornecedorId,
                Quantidade = model.Quantidade,
                PrecoUnitario = fornecedor.Preco,
                TempoEntrega = fornecedor.TempoEntrega ?? 0,
                DataCompra = DateTime.Now,
                //Recebida = false // só será marcada quando o stock for recebido
            };
            _context.Compra.Add(compra);

            var stock = _context.Stock
    .Where(s => s.ConsumivelID == model.ConsumivelId)
    .FirstOrDefault();

            if (stock == null)
                return BadRequest("Não existe stock para este consumível.");

            // Criar o histórico de compras associado ao stock
            var historico = new HistoricoCompras
            {
                Quantidade = model.Quantidade,
                FornecedorId = fornecedor.FornecedorId,
                Tipo = "Entrada",
                Data = DateTime.Now,
                StockId = stock.StockId // agora não é null
            };

            _context.HistoricoCompras.Add(historico);
            _context.SaveChanges();

            TempData["EncomendaRealizada"] = true;
            TempData["ConsumivelIdEncomendado"] = model.ConsumivelId;
            TempData["QuantidadeEncomendada"] = model.Quantidade;
            TempData["SuccessMessage"] = "✅ Encomenda registada com sucesso!";

            return RedirectToAction("Index", "HistoricoCompras");
        }



        // HISTÓRICO SIMPLES
        public IActionResult Historico()
        {
            return View(_context.Compra
                .Include(c => c.Consumivel)
                .Include(c => c.Fornecedor)
                .ToList());
        }
    }
}
