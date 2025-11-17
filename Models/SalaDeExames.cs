using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Sala
    {
        public int SalaId { get; set; }

        public string Numero_de_sala { get; set; }
        [Phone(ErrorMessage = "Número Obrigatório")]
        
        [StringLength(100)]
        public string TipoSala { get; set; }

        [StringLength(500)]
        public string? Laboratorio { get; set; }
    }
}
