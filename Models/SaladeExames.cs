using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class SaladeExames
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A instância do Tipo de Sala é obrigatória")]
        [StringLength(100)]
        public string Laboratorio { get; set; }

        [StringLength(500)]


        public string Tipo_de_sala { get; set; }
    }
}
