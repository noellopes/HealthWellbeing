using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Material
    {
        public int MaterialId { get; set; }

        [Required(ErrorMessage ="Nome é obrigatório.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Deve conter no min 3 letras e 100 no max.")]
        public string Nome { get; set; }
        
        [Required(ErrorMessage = "Especificação é obrigatório.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Deve conter no min 5 letras e 200 no max.")]
        public string Especificacao { get; set; }

        [Required]
        public int TipoMaterialId {  get; set; }

        [Required]
        public int Quantidade { get; set; }

        public DateTime DataCriacao { get; set; }

    }
}
