using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Models
{
    [Index(nameof(Nome), IsUnique = true)]
    public class CategoriaConsumivel
    {
        [Key]
        public int CategoriaId { get; set; }

        [Required(ErrorMessage = "O nome da categoria é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres.")]
        [MinLength(3, ErrorMessage = "O nome deve ter pelo menos 3 caracteres.")]
        [RegularExpression(@"^[A-Za-zÀ-ÖØ-öø-ÿ0-9\s.,()-]+$", ErrorMessage = "O nome contém caracteres inválidos.")]
        [Display(Name = "Nome da Categoria")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória.")]
        [StringLength(300, ErrorMessage = "A descrição não pode exceder 300 caracteres.")]
        [MinLength(10, ErrorMessage = "A descrição deve ser mais detalhada.")]
        [RegularExpression(@"^[A-Za-zÀ-ÖØ-öø-ÿ0-9\s.,()-]+$", ErrorMessage = "A descrição contém caracteres inválidos.")]
        public string Descricao { get; set; }
    }
}
