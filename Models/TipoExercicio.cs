using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class TipoExercicio
    {
        public int TipoExercicioId { get; set; }

        [Required(ErrorMessage = "A categoria dos exercícios é obrigatória.")]
        [StringLength(50, ErrorMessage = "A categoria deve ter no máximo 50 caracteres.")]
        public string NomeTipoExercicios { get; set; }

        [Required(ErrorMessage = "A descrição do tipo de exercícios é obrigatória.")]
        [StringLength(250, ErrorMessage = "A descrição deve ter no máximo 250 caracteres.")]
        public string DescricaoTipoExercicios { get; set; }

        
        [Required(ErrorMessage = "As características do tipo de exercícios é obrigatória.")]
        [StringLength(200, ErrorMessage = "As características deve ter no máximo 200 caracteres.")]
        public string CaracteristicasTipoExercicios { get; set; }

        // Relacionamento N:N — um tipo de exercício pode ter vários benefícios
        public ICollection<Beneficio>? Beneficios { get; set; }

    }
}
