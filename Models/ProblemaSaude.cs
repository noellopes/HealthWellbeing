using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class ProblemaSaude
    {
        public int ProblemaSaudeId { get; set; }

        [Required(ErrorMessage = "A categoria do problema é obrigatória")]
        [StringLength(50, ErrorMessage = "A Categoria do problema deve ter no máximo 50 caracteres")]
        public string ProblemaCategoria { get; set; }

        [Required(ErrorMessage = "O nome do problema é obrigatório")]
        [StringLength(50, ErrorMessage = "O Nome do problema deve ter no máximo 50 caracteres")]
        public string ProblemaNome { get; set; }

        [Required(ErrorMessage = "A zona atingida é obrigatória")]
        [StringLength(50, ErrorMessage = "A Zona Atingida do problema deve ter no máximo 50 caracteres")]
        public string ZonaAtingida { get; set; }

        [Range(1, 10, ErrorMessage = "A gravidade deve estar entre 1 e 10")]
        public int Gravidade { get; set; }

        public ICollection<TipoExercicioProblemaSaude>? TipoExercicioAfetado { get; set; }
        public ICollection<ExercicioProblemaSaude>? ExercicioAfetado { get; set; }
        public ICollection<UtenteGrupo7ProblemaSaude>? UtenteProblemasSaude { get; set; }
    }
}
