using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class ProfissionalExecutante
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do profissional é obrigatório")]
        [StringLength(100)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A função/cargo é obrigatória")]
        [StringLength(100)]
        public string Funcao { get; set; }

        [Phone(ErrorMessage = "Número de telefone inválido")]
        public string Telefone { get; set; }

        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; }

       
        public virtual ICollection<ProblemaSaude> ProblemaSaudes { get; set; } = new List<ProblemaSaude>();
    }
}