using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HealthWellbeing.Models.Enums;

namespace HealthWellbeing.Models
{
    [Table("ClienteBalneario", Schema = "dbo")]

    public class ClienteBalnearioModel
    {
        [Key]
        public int ClienteBalnearioId { get; set; }

        [Required(ErrorMessage = "O nome completo é obrigatório")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 150 caracteres")]
        [Display(Name = "Nome Completo")]
        public string NomeCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O telemóvel é obrigatório")]
        [RegularExpression(@"^(91|92|93|96)\d{7}$",
        ErrorMessage = "O telemóvel deve começar por 91, 92, 93 ou 96 e ter 9 dígitos")]
        [Display(Name = "Telemóvel")]
        public string Telemovel { get; set; } = string.Empty;


        [Required(ErrorMessage = "A morada é obrigatória")]
        [StringLength(200, ErrorMessage = "A morada não pode ultrapassar 200 caracteres")]
        public string Morada { get; set; } = string.Empty;

        [Required(ErrorMessage = "O tipo de cliente é obrigatório")]
        [Display(Name = "Tipo de Cliente")]
        public TipoCliente TipoCliente { get; set; }

    }
}
