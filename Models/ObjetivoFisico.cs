using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class ObjetivoFisico
    {
        public int ObjetivoFisicoId { get; set; }

        [Required(ErrorMessage = "O nome do objetivo é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        public string NomeObjetivo { get; set; }

        public ICollection<ObjetivoTipoExercicio>? ObjetivoTipoExercicio { get; set; }
        public ICollection<UtenteGrupo7>? Utentes { get; set; }
    }
}
