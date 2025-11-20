using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class ProfissionalExecutante
    {
        [Key]
        public int ProfissionalExecutanteId { get; set; } // Chave primária

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100)]
        public string Nome { get; set; }


        [Required(ErrorMessage = "A função é obrigatória")]
        [StringLength(150)]
        public string Funcao { get; set; } 


        [Required(ErrorMessage = "O numero é obrigatório")]
        [Phone(ErrorMessage = "Formato de telefone inválido.")]
        [StringLength(15)]
        public string Telefone { get; set; }

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        [StringLength(250)]
        public string Email { get; set; }

        

    }
}
