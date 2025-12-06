using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Controllers
{
    public class RegistoComprasController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public RegistoComprasController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // INDEX — Lista das opções de compra
        // =====================================================
        public IActionResult Index()
        {
            var opcoes = _context.CompraOpcao
                .Include(c => c.Consumivel)
                .Include(c => c.Fornecedor)
                .OrderBy(c => c.Consumivel.Nome)
                .ToList();

            return View(opcoes);
        }

        // =====================================================
        // CREATE (GET)
        // =====================================================
        public IActionResult Create()
        {
            ViewBag.Consumiveis = _context.Consumivel
                .OrderBy(c => c.Nome)
                .ToList();

            ViewBag.Fornecedores = _context.Fornecedor
                .OrderBy(f => f.NomeEmpresa)
                .ToList();

            return View(new CompraOpcao());
        }

        // =====================================================
        // CREATE (POST)
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CompraOpcao opcao)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Consumiveis = _context.Consumivel.ToList();
                ViewBag.Fornecedores = _context.Fornecedor.ToList();
                return View(opcao);
            }

            opcao.DataRegisto = DateTime.Now;

            _context.CompraOpcao.Add(opcao);
            _context.SaveChanges();

            TempData["Success"] = "Opção de compra registada com sucesso!";
            return RedirectToAction(nameof(Index));
        }
    }
}
