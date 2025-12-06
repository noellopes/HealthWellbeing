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

        // ================================
        // LISTAGEM (Index)
        // ================================
        public IActionResult Index()
        {
            var opcoes = _context.CompraOpcoes
                .Include(c => c.Consumivel)
                .Include(c => c.Fornecedor)
                .OrderBy(c => c.Consumivel.Nome)
                .ToList();

            return View(opcoes);
        }

        // ================================
        // FORMULÁRIO (GET)
        // ================================
        public IActionResult Create()
        {
            ViewBag.Consumiveis = _context.Consumivel.ToList();
            ViewBag.Fornecedores = _context.Fornecedor.ToList();

            return View(new CompraOpcao());
        }

        // ================================
        // GRAVAR (POST)
        // ================================
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

            _context.CompraOpcoes.Add(opcao);
            _context.SaveChanges();

            TempData["Success"] = "Opção de compra registada com sucesso!";
            return RedirectToAction(nameof(Index));
        }
    }
}
