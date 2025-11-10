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

        [StringLength(500, ErrorMessage = "A descrição não pode ter mais de 500 caracteres.")]
        public string? Descricao { get; set; }

        // Chave estrangeira
        [ForeignKey("CategoriaConsumivel")]
        [Display(Name = "Categoria")]
        public int CategoriaId { get; set; }

        // Propriedade de navegação (importante para o EF)
        public CategoriaConsumivel? CategoriaConsumivel { get; set; }
    }
}