using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class TipoExercicio
    {
        [Required(ErrorMessage = "O ID do tipo de exercícios é obrigatório.")]
        public int TiposExerciciosId { get; set; }

        [Required(ErrorMessage = "A categoria dos exercícios é obrigatória.")]
        public string CategoriaTipoExercicios { get; set; }

        [Required(ErrorMessage = "A descrição do tipo de exercícios é obrigatória.")]
        public string DescricaoTipoExercicios { get; set; }

        [Required(ErrorMessage = "O nível de dificuldade dos exercícios é obrigatória.")]
        public string NivelDificuldadeTipoExercicios { get; set; }

        [Required(ErrorMessage = "O beneficio dos exercicios é obrigatória.")]
        public string BeneficioTipoExercicios { get; set; }

        [Required(ErrorMessage = "O grupo muscular dos exercicios é obrigatório.")]
        public string GruposMuscularesTrabalhadosTipoExercicios { get; set; }
        
    }
}
