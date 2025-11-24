using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class CategoriaConsumivel
    {
        [Key]
        public int CategoriaId { get; set; }

        [Required(ErrorMessage = "O nome da categoria é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres.")]
        [Display(Name = "Nome da Categoria")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória.")]
        [StringLength(300, ErrorMessage = "A descrição não pode exceder 300 caracteres.")]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }
    }
}
