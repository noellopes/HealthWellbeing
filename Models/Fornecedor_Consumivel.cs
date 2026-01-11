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

        [Required(ErrorMessage = "O tempo de entrega é obrigatório.")]
        [Range(0, int.MaxValue, ErrorMessage = "O tempo de entrega deve ser um valor positivo.")]
        public int? TempoEntrega { get; set; }

        [Required(ErrorMessage = "O preço é obrigatório.")]
        [Range(0, float.MaxValue, ErrorMessage = "O preço deve ser um valor positivo.")]
        public float Preco { get; set; }
    }
}
