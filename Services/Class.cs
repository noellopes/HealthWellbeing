using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Services
{
    public class StockService
    {
        private readonly HealthWellbeingDbContext _context;

        public StockService(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // ---------------------------
        //  SAÍDA DE STOCK AUTOMÁTICA
        // ---------------------------
        public async Task<bool> RegistarSaidaAsync(int consumivelId, int quantidade)
        {
            var stock = await _context.Stock
                .Include(s => s.Consumivel)
                .FirstOrDefaultAsync(s => s.ConsumivelID == consumivelId);

            if (stock == null)
                return false;

            if (quantidade <= 0)
                return false;

            if (stock.QuantidadeAtual < quantidade)
                return false;

            stock.QuantidadeAtual -= quantidade;
            stock.DataUltimaAtualizacao = DateTime.Now;

            var movimento = new StockMovimento
            {
                StockId = stock.StockId,
                Quantidade = quantidade,
                Tipo = "Saida",
                Data = DateTime.Now
            };

            _context.StockMovimento.Add(movimento);

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
