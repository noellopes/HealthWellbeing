using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Sala
    {
        public int SalaId { get; set; }

        [Required(ErrorMessage = "O Tipo de sala é obrigatória")]
        [StringLength(100)]
        public string TipoSala { get; set; }

        [StringLength(500)]
        public string? Laboratorio { get; set; }
    }
}
