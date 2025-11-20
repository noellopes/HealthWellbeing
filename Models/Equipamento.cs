using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Equipamento
    {
        public int EquipamentoId { get; set; }

        [Required(ErrorMessage = "O nome do equipamento é obrigatório.")]
        [StringLength(50, ErrorMessage = "O nome do equipamento não pode exceder 50 caracteres.")]
        public string NomeEquipamento { get; set; }

        public ICollection<ExercicioEquipamento>? ExercicioEquipamentos { get; set; }
    }
}
