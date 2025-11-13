using System;

namespace HealthWellbeing.Models
{
    public class Stock
    {
        public int StockId { get; set; }

        // FK para Consumível e Zona
        public int ConsumivelID { get; set; }
        public int ZonaID { get; set; }

        // Quantidade atual (pode ser futuramente calculada)
        public int QuantidadeAtual { get; set; }

        // Quantidade mínima para alerta
        public int QuantidadeMinima { get; set; }

        // Data da última atualização
        public DateTime DataUltimaAtualizacao { get; set; }

        public Consumivel Consumivel { get; set; }
        public ZonaArmazenamento Zona { get; set; }
    }
}
