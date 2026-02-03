using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
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

        // =========================
        // CRIAR VOUCHER A PARTIR DE PONTOS
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(int clienteId, int pontos)
        {
            var cliente = await _context.ClientesBalneario
                .Include(c => c.HistoricoPontos)
                .FirstOrDefaultAsync(c => c.ClienteBalnearioId == clienteId);

            if (cliente == null)
                return NotFound();

            int totalPontos = cliente.HistoricoPontos.Sum(p => p.Pontos);

            if (totalPontos < pontos)
            {
                TempData["Error"] = "Pontos insuficientes.";
                return RedirectToAction("Details", "ClienteBalneario", new { id = clienteId });
            }

            decimal valor = pontos / 10m; // 10 pontos = 1€

            cliente.HistoricoPontos.Add(new HistoricoPontos
            {
                ClienteBalnearioId = clienteId,
                Pontos = -pontos,
                Motivo = "Conversão em voucher",
                Data = DateTime.Now
            });

            _context.VouchersCliente.Add(new VoucherCliente
            {
                ClienteBalnearioId = clienteId,
                Descricao = $"Voucher de {valor:0.00}€",
                Valor = valor,
                DataCriacao = DateTime.Now,
                Usado = false
            });

            await _context.SaveChangesAsync();

            TempData["Success"] = "Voucher criado com sucesso.";
            return RedirectToAction("Details", "ClienteBalneario", new { id = clienteId });
        }

        // =========================
        // USAR VOUCHER
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Usar(int id)
        {
            var voucher = await _context.VouchersCliente.FindAsync(id);
            if (voucher == null)
                return NotFound();

            if (voucher.Usado)
            {
                TempData["Error"] = "Este voucher já foi utilizado.";
                return RedirectToAction("Details", "ClienteBalneario",
                    new { id = voucher.ClienteBalnearioId });
            }

            voucher.Usado = true;
            voucher.DataUtilizacao = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Voucher utilizado com sucesso.";

            return RedirectToAction("Details", "ClienteBalneario",
                new { id = voucher.ClienteBalnearioId });
        }

    }
}
