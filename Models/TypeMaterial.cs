using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class TypeMaterial
    {
        public int TypeMaterialID { get; set; }

        [Required(ErrorMessage = "O nome do TipoMaterial é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres.")]
        public string Name { get; set; }

        [StringLength(255, ErrorMessage = "A descrição não pode exceder 255 caracteres.")]
        public string Description { get; set; }
    }
}
