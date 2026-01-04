using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class TypeMaterial
    {
        public int TypeMaterialID { get; set; }

        [Required(ErrorMessage = "O nome do TipoMaterial é obrigatório.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter no mínimo 3 caracteres e não pode exceder 100 caracteres.")]
        [RegularExpression(@"^(?!\d+$).+", ErrorMessage = "O nome não pode ser apenas números.")]
        public string Name { get; set; }

        [StringLength(255, MinimumLength = 5, ErrorMessage = "A descrição deve ter no mínimo 10 caracteres e não pode exceder 255 caracteres.")]
        public string Description { get; set; }
    }
}
