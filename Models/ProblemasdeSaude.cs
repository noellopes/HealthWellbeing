using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class ProblemasdeSaude
    {
        [Key] // Identificador único
        public int ProblemaId { get; set; }

        [Required(ErrorMessage = "A categoria do problema é obrigatória")]
        [StringLength(100, ErrorMessage = "A categoria não pode ter mais de 100 caracteres")]
        public string ProblemaCategoria { get; set; }

        [Required(ErrorMessage = "O nome do problema é obrigatório")]
        [StringLength(150, ErrorMessage = "O nome não pode ter mais de 150 caracteres")]
        public string ProblemaNome { get; set; }

        [Required(ErrorMessage = "A zona atingida é obrigatória")]
        [StringLength(100, ErrorMessage = "A zona atingida não pode ter mais de 100 caracteres")]
        public string ZonaAtingida { get; set; }

        [Required(ErrorMessage = "Os profissionais de apoio são obrigatórios")]
        [StringLength(200, ErrorMessage = "O campo de profissionais de apoio não pode ter mais de 200 caracteres")]
        public string ProfissionaisDeApoio { get; set; }

        [Range(1, 10, ErrorMessage = "A gravidade deve estar entre 1 e 10")]
        public int Gravidade { get; set; }
    }
}
