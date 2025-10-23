using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class TypeMaterial
    {
        public int TypeMaterialID {  get; set; }
        [Required(ErrorMessage = "O nome do TipoMaterial é obrigatorio.")]
        [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres")]
        public string Name { get; set; }

        public string Description { get; set; }
        [StringLength(255, ErrorMessage = "A descrição não pode exceder 255 caracteres.")]

        public string Category { get; set; }
        [StringLength(50, ErrorMessage = "A categoria não pode exceder 50 caracteres.")]
       

        public bool Active { get; set; } = true;

       
        public DateTime DataCreate { get; set; } = DateTime.Now;
        [Required(ErrorMessage = "A data de criação é obrigatória.")]

        public DateTime? DataAtualizacao { get; set; }

    }
}
