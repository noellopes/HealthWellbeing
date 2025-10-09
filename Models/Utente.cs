using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Utente
    {
        public int Id { get; set; } 

        [Required(ErrorMessage = "O nome do utente é obrigatório")]
        [StringLength(150)]
        public string Nome { get; set; } = string.Empty;

        [StringLength(9, ErrorMessage = "O número de identificação civil deve ter 9 dígitos")]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "O número de identificação civil deve conter apenas 9 dígitos")]
        public string NumeroIdentificacaoCivil { get; set; } = string.Empty;

        [StringLength(9, ErrorMessage = "O NIF deve ter 9 dígitos")]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "O NIF deve conter apenas 9 dígitos")]
        public string NIF { get; set; } = string.Empty; 

        [Phone(ErrorMessage = "Número de telefone inválido")]
        public string Telefone { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = string.Empty;

        [StringLength(200)]
        public string Morada { get; set; } = string.Empty;

        [StringLength(10)]
        public string CodigoPostal { get; set; } = string.Empty;

        [StringLength(50)]
        public string Localidade { get; set; } = string.Empty; 

    }
}
