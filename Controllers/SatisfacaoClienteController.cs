using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace HealthWellbeing.Controllers
{
    [Authorize(Roles = "Admin,Rececionista")]
    public class SatisfacaoClienteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ClienteService _clienteService;

        
        public SatisfacaoClienteController(
            ApplicationDbContext context,
            ClienteService clienteService)
        {
            _context = context;
            _clienteService = clienteService;
        }

        // =========================
        // CREATE (GET)
        // =========================
        public async Task<IActionResult> Create(int clienteId)
        {
            var cliente = await _context.ClientesBalneario.FindAsync(clienteId);
            if (cliente == null)
                return NotFound();

            ViewBag.Cliente = cliente;

            return View(new SatisfacaoCliente
            {
                ClienteBalnearioId = clienteId
            });
        }

        // =========================
        // CREATE (POST)
        // =========================
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

            var cliente = await _context.ClientesBalneario
                .FirstOrDefaultAsync(c => c.ClienteBalnearioId == satisfacao.ClienteBalnearioId);

            if (cliente == null)
                return NotFound();

            // ✅ Registar satisfação
            satisfacao.DataRegisto = DateTime.Now;
            _context.SatisfacoesClientes.Add(satisfacao);

            // ✅ Atribuir pontos
            _context.HistoricoPontos.Add(new HistoricoPontos
            {
                ClienteBalnearioId = cliente.ClienteBalnearioId,
                Pontos = 10,
                Motivo = "Avaliação de satisfação",
                Data = DateTime.Now
            });

            // ✅ Atualizar nível do cliente (SERVICE)
            await _clienteService.AtualizarNivelClienteAsync(cliente.ClienteBalnearioId);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Avaliação registada e pontos atribuídos com sucesso.";

            return RedirectToAction(
                "Details",
                "ClienteBalneario",
                new { id = cliente.ClienteBalnearioId }
            );
        }
    }
}
