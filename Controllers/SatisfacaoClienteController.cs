using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellbeing.Controllers
{
    [Authorize]
    public class SatisfacaoClienteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SatisfacaoClienteController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // CREATE
        // =========================
        public IActionResult Create(int clienteId)
        {
            ViewBag.Cliente = _context.ClientesBalneario.Find(clienteId);
            return View(new SatisfacaoCliente
            {
                ClienteBalnearioId = clienteId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SatisfacaoCliente satisfacao)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Cliente = await _context.ClientesBalneario
                    .FindAsync(satisfacao.ClienteBalnearioId);

                return View(satisfacao);
            }

            satisfacao.DataRegisto = DateTime.Now;

            _context.SatisfacoesClientes.Add(satisfacao);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Avaliação registada com sucesso.";

            return RedirectToAction("Details", "ClienteBalneario",
                new { id = satisfacao.ClienteBalnearioId });
        }
    }
}
