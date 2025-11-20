using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class Consumivel
    {
        [Key]
        public int ConsumivelId { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome não pode ter mais de 100 caracteres.")]
        public string Nome { get; set; }

        [Display(Name = "Categoria")]
        [ForeignKey("CategoriaConsumivel")]
        public int CategoriaId { get; set; }

        [StringLength(500, ErrorMessage = "A descrição não pode ter mais de 500 caracteres.")]
        public string? Descricao { get; set; }

        [Display(Name = "Quantidade Máxima")]
        [Range(0, int.MaxValue, ErrorMessage = "A quantidade máxima deve ser um número positivo.")]
        public int QuantidadeMaxima { get; set; }

        [Display(Name = "Quantidade Atual")]
        [Range(0, int.MaxValue, ErrorMessage = "A quantidade atual deve ser um número positivo.")]
        public int QuantidadeAtual { get; set; }

        [Display(Name = "Quantidade Mínima")]
        [Range(0, int.MaxValue, ErrorMessage = "A quantidade mínima deve ser um número positivo.")]
        public int QuantidadeMinima { get; set; }

        // Navegação
        public CategoriaConsumivel? CategoriaConsumivel { get; set; }
    }
}
