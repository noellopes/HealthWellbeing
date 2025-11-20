using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class TipoExercicio
    {
        public int TipoExercicioId { get; set; }

        [Required(ErrorMessage = "O nome do tipo de exercicios é obrigatório.")]
        [StringLength(50, ErrorMessage = "O nome deve ter no máximo 50 caracteres.")]
        public string NomeTipoExercicios { get; set; }

        [Required(ErrorMessage = "A descrição do tipo de exercícios é obrigatória.")]
        [StringLength(250, ErrorMessage = "A descrição deve ter no máximo 250 caracteres.")]
        public string DescricaoTipoExercicios { get; set; }

        [Required(ErrorMessage = "As caracetrísticas do tipo de exercicios são obrigatórias.")]
        [StringLength(250, ErrorMessage = "As caracetrísticas devem ter no máximo 250 caracteres.")]
        public string CaracteristicasTipoExercicios { get; set; }

        public ICollection<TipoExercicioBeneficio>? TipoExercicioBeneficios { get; set; }

        public ICollection<TipoExercicioProblemaSaude>? Contraindicacao { get; set; }

        public ICollection<ObjetivoTipoExercicio>? ObjetivoTipoExercicio { get; set; }
    }
}

