using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellbeing.Services
{
    public class VoucherService
    {
        private readonly ApplicationDbContext _context;

        public VoucherService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task VerificarECriarVoucherAsync(int clienteId)
        {
            var pontos = await _context.HistoricoPontos
                .Where(p => p.ClienteBalnearioId == clienteId)
                .SumAsync(p => p.Pontos);

            // regra exemplo: 100 pontos = voucher 10€
            if (pontos >= 100)
            {
                bool jaExiste = await _context.VouchersCliente
                    .AnyAsync(v => v.ClienteBalnearioId == clienteId && !v.Usado);

                if (jaExiste)
                    return;

                _context.VouchersCliente.Add(new VoucherCliente
                {
                    ClienteBalnearioId = clienteId,
                    Titulo = "Voucher Fidelização",
                    Descricao = "Voucher atribuído automaticamente",
                    PontosNecessarios = 100,
                    Valor = 10m,
                    DataValidade = DateTime.Now.AddMonths(1)
                });

                _context.HistoricoPontos.Add(new HistoricoPontos
                {
                    ClienteBalnearioId = clienteId,
                    Pontos = -100,
                    Motivo = "Conversão automática em voucher",
                    Data = DateTime.Now
                });

                await _context.SaveChangesAsync();
            }
        }
    }
}
