using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class CompraOpcao
    {
        [Key]
        public int CompraOpcaoId { get; set; }

        public int ConsumivelId { get; set; }
        public Consumivel? Consumivel { get; set; }

        public int FornecedorId { get; set; }
        public Fornecedor? Fornecedor { get; set; }

        public decimal Preco { get; set; }
        public int TempoEntrega { get; set; }
        public int Quantidade { get; set; }

        public DateTime DataRegisto { get; set; } = DateTime.Now;
    }
}
