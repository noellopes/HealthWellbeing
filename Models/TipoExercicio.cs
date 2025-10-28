using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class TipoExercicio
    {
        public int TipoExercicioId { get; set; }

        // Futuramente pode se tornar uma entidade
        [Required(ErrorMessage = "A categoria dos exercícios é obrigatória.")]
        [StringLength(50, ErrorMessage = "A categoria deve ter no máximo 50 caracteres.")]
        public string CategoriaTipoExercicios { get; set; }

        [Required(ErrorMessage = "A descrição do tipo de exercícios é obrigatória.")]
        [StringLength(250, ErrorMessage = "A descrição deve ter no máximo 250 caracteres.")]
        public string DescricaoTipoExercicios { get; set; }

        // Futuramente pode se tornar uma entidade
        [Required(ErrorMessage = "O nível de dificuldade dos exercícios é obrigatória.")]
        [StringLength(50, ErrorMessage = "O nível de dificuldade deve ter no máximo 50 caracteres.")]
        public string NivelDificuldadeTipoExercicios { get; set; }

        // Futuramente pode se tornar uma entidade
        [Required(ErrorMessage = "O beneficio dos exercicios é obrigatória.")]
        [StringLength(250, ErrorMessage = "O benefício deve ter no máximo 250 caracteres.")]
        public string BeneficioTipoExercicios { get; set; }

        // Futuramente pode se tornar uma entidade
        [Required(ErrorMessage = "O grupo muscular dos exercicios é obrigatório.")]
        [StringLength(150, ErrorMessage = "O grupo muscular deve ter no máximo 150 caracteres.")]
        public string GruposMuscularesTrabalhadosTipoExercicios { get; set; }
        
    }
}
