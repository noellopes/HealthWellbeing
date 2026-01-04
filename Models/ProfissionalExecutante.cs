using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class ProfissionalExecutante
    {
        public int Id { get; set; } //Id do Profissional

        [Required(ErrorMessage = "O nome do profissional é obrigatório")]
        [StringLength(100)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A função/cargo é obrigatória")]
        [StringLength(100)]
        public string Funcao { get; set; } //Funcao que o mesmo tem ex tipo "Técnico de Radiologia"

        [Phone(ErrorMessage = "Número de telefone inválido")]
        public string Telefone { get; set; }

        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; }
        public object ProfissionalExecutanteId { get; internal set; }
    }
}