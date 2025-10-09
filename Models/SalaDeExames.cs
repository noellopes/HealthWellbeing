using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class ProfissionalExecutante
    {
        public int Id { get; set; }

        public string Numero_de_sala { get; set; }
        [Phone(ErrorMessage = "Número Obrigatório")]
        public string Laboratorio { get; set; }
        [StringLength(100)]

        [Phone(ErrorMessage = "Laboratório Obrigatório")]

        public string Tipo_de_sala { get; set; }

    }
}
