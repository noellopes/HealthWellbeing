namespace HealthWellbeing.Models
{
    public class Stock
    {
        public int StockId { get; set; }
        public int ConsumivelID { get; set; }
        public int ZonaID { get; set; }
        public int QuantidadeAtual { get; set; }

        public int QuantidadeMinima { get; set; }
        public DateTime DataUltimaAtualizacao { get; set; }
    }
}
