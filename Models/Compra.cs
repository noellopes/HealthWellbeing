namespace HealthWellbeing.Models
{
    public class Compra
    {
        public int CompraId { get; set; }

        public int ConsumivelId { get; set; }
        public Consumivel Consumivel { get; set; }

        public int ZonaId { get; set; }
        
        public int FornecedorId { get; set; }
        public Fornecedor Fornecedor { get; set; }

        public int Quantidade { get; set; }

        public float PrecoUnitario { get; set; }
        public int TempoEntrega { get; set; }


        public DateTime DataCompra { get; set; } = DateTime.Now;
    }

}
