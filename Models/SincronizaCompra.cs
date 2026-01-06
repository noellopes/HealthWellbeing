using HealthWellbeing.Data;
using HealthWellbeing.Models;

namespace HealthWellbeingRoom.Models
{
    public static class SincronizaCompra
    {
        public static void AtualizarStockAposCompra(
            HealthWellbeingDbContext context,
            int consumivelId)
        {
            // 🔹 Obter consumível atualizado
            var consumivel = context.Consumivel
                .FirstOrDefault(c => c.ConsumivelId == consumivelId);

            if (consumivel == null)
                return;

            // 🔹 Obter stock correspondente
            var stock = context.Stock
                .FirstOrDefault(s => s.ConsumivelID == consumivelId);

            if (stock == null)
                return;

            // 🔒 Stock espelha SEMPRE o consumível
            if (stock.UsaValoresDoConsumivel)
            {
                stock.QuantidadeAtual = consumivel.QuantidadeAtual;
                stock.QuantidadeMinima = consumivel.QuantidadeMinima;
                stock.QuantidadeMaxima = consumivel.QuantidadeMaxima;
                stock.DataUltimaAtualizacao = DateTime.Now;
            }

            context.SaveChanges();
        }
    }
}
