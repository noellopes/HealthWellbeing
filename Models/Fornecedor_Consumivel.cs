using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class Fornecedor_Consumivel
    {
        [Key]
        public int FornecedorConsumivelId { get; set; }

        public int FornecedorId { get; set; }
        public Fornecedor Fornecedor { get; set; }

        public int ConsumivelId { get; set; }
        public Consumivel Consumivel { get; set; }

        public int? TempoEntrega { get; set; }
        public float Preco { get; set; }
    }
}
