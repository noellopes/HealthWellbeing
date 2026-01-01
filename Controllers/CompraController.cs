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
            var fornecedor = _context.Fornecedor_Consumivel
                .First(fc => fc.FornecedorId == model.FornecedorId
                          && fc.ConsumivelId == model.ConsumivelId);

            _context.Compra.Add(new Compra
            {
                ConsumivelId = model.ConsumivelId,
                FornecedorId = fornecedor.FornecedorId,
                Quantidade = model.Quantidade,
                PrecoUnitario = fornecedor.Preco,
                TempoEntrega = fornecedor.TempoEntrega ?? 0
            });

            var consumivel = _context.Consumivel.Find(model.ConsumivelId);
            consumivel.QuantidadeAtual += model.Quantidade;

            _context.SaveChanges();

            return RedirectToAction(nameof(Historico));
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
