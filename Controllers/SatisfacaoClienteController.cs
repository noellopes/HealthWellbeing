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
    //[Authorize(Roles = "Admin,Rececionista")]
    public class SatisfacaoClienteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ClienteService _clienteService;
        private readonly VoucherService _voucherService;

        public SatisfacaoClienteController(
            ApplicationDbContext context,
            ClienteService clienteService,
            VoucherService voucherService)
        {
            _context = context;
            _clienteService = clienteService;
            _voucherService = voucherService;
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
        public async Task<IActionResult> Create(
         [Bind("ClienteBalnearioId,Avaliacao,Comentario")]
            SatisfacaoCliente satisfacao)
        {
            var cliente = await _context.ClientesBalneario
                .FirstOrDefaultAsync(c => c.ClienteBalnearioId == satisfacao.ClienteBalnearioId);

            if (cliente == null)
                return NotFound();

            // ❌ Impedir mais do que uma avaliação por dia
            bool jaAvaliadoHoje = await _context.SatisfacoesClientes.AnyAsync(s =>
                s.ClienteBalnearioId == satisfacao.ClienteBalnearioId &&
                s.DataRegisto.Date == DateTime.Today
            );

            if (jaAvaliadoHoje)
            {
                ModelState.AddModelError("", "Este cliente já foi avaliado hoje.");
                ViewBag.Cliente = cliente;
                return View(satisfacao);
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Cliente = cliente;
                return View(satisfacao);
            }

            // ✅ Avaliação
            satisfacao.DataRegisto = DateTime.Now;
            _context.SatisfacoesClientes.Add(satisfacao);

            // ✅ Pontos
            _context.HistoricoPontos.Add(new HistoricoPontos
            {
                ClienteBalnearioId = cliente.ClienteBalnearioId,
                Pontos = 10,
                Motivo = "Avaliação de satisfação",
                Data = DateTime.Now
            });

            // Guardar avaliação + pontos
            await _context.SaveChangesAsync();

            // Serviços
            await _clienteService.AtualizarNivelClienteAsync(cliente.ClienteBalnearioId);
            await _voucherService.VerificarECriarVoucherAsync(cliente.ClienteBalnearioId);

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
