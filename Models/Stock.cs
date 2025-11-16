using System;

namespace HealthWellbeing.Models
{
    public class Stock
    {
        public int StockId { get; set; }

        public int ConsumivelID { get; set; }
        public int ZonaID { get; set; }

        public int QuantidadeAtual { get; set; }

        public int QuantidadeMinima { get; set; }



        // Data da última atualização
        public DateTime DataUltimaAtualizacao { get; set; }

        public Consumivel Consumivel { get; set; }
        public ZonaArmazenamento Zona { get; set; }
    }
}
