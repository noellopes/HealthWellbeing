using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class SalaDeExames
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O Tipo de sala é obrigatória")]
        [StringLength(100)]
        public string Tipo_de_sala { get; set; }

        [StringLength(500)]
        
        public string Laboratorio { get; set; }

        [StringLength(500)]
    }
}
