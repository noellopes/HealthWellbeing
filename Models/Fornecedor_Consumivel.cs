using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class Fornecedor_Consumivel
    {
        [Key]
        public int FornecedorConsumivelId { get; set; }

        // Foreign Key para Fornecedor
        [Required]
        public int FornecedorId { get; set; }

        [ForeignKey(nameof(FornecedorId))]
        public Fornecedor? Fornecedor { get; set; }

        // Foreign Key para Consumivel
        [Required]
        public int ConsumivelId { get; set; }

        [ForeignKey(nameof(ConsumivelId))]
        public Consumivel? Consumivel { get; set; }

        public int? TempoEntrega { get; set; } // em dias

        [Required(ErrorMessage = "O preço é obrigatório.")]
        [Range(0, double.MaxValue, ErrorMessage = "O preço deve ser um valor positivo.")]
        public float Preco { get; set; }
    }
}
