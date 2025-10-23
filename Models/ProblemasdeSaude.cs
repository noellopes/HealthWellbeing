using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class ProblemasdeSaude
    {
        public int ProblemaId { get; set; }

        [Required(ErrorMessage = "A categoria do problema é obrigatória")]
        [StringLength(100)]
        public string ProblemaCategoria { get; set; }

        [Required(ErrorMessage = "O nome do problema é obrigatório")]
        [StringLength(150)]
        public string ProblemaNome { get; set; }

        [Required(ErrorMessage = "A zona atingida é obrigatória")]
        [StringLength(100)]
        public string ZonaAtingida { get; set; }

        [Required(ErrorMessage = "Os profissionais de apoio são obrigatórios")]
        [StringLength(200)]
        public string ProfissionaisDeApoio { get; set; }

        [Range(1, 10, ErrorMessage = "A gravidade deve estar entre 1 e 10")]
        public int Gravidade { get; set; }
    }
}
