using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace HealthWellbeing.Controllers
{
    [Authorize]
    public class VoucherClienteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VoucherClienteController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Criar voucher a partir de pontos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(int clienteId, int pontos)
        {
            var cliente = await _context.ClientesBalneario
                .Include(c => c.HistoricoPontos)
                .FirstOrDefaultAsync(c => c.ClienteBalnearioId == clienteId);

            if (cliente == null)
                return NotFound();

            if (cliente.HistoricoPontos.Sum(p => p.Pontos) < pontos)
            {
                TempData["Error"] = "Pontos insuficientes.";
                return RedirectToAction("Details", "ClienteBalneario", new { id = clienteId });
            }

            var valor = pontos / 10m; // regra simples: 10 pontos = 1€

            cliente.HistoricoPontos.Add(new HistoricoPontos
            {
                Pontos = -pontos,
                Motivo = "Conversão em voucher"
            });

            _context.VouchersCliente.Add(new VoucherCliente
            {
                ClienteBalnearioId = clienteId,
                Descricao = $"Voucher de {valor:0.00}€",
                Valor = valor
            });

            await _context.SaveChangesAsync();

            TempData["Success"] = "Voucher criado com sucesso.";
            return RedirectToClub();
        }

        private IActionResult RedirectToClub() => RedirectToAction("Index", "ClienteBalneario");
    }
}
