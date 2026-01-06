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
        [RegularExpression(@"^9\d{8}$", ErrorMessage = "O telemóvel deve ter 9 dígitos e começar por 9")]
        [Display(Name = "Telemóvel")]
        public string Telemovel { get; set; } = string.Empty;

        [Required(ErrorMessage = "A morada é obrigatória")]
        [StringLength(200, ErrorMessage = "A morada não pode ultrapassar 200 caracteres")]
        public string Morada { get; set; } = string.Empty;

        [Required(ErrorMessage = "O tipo de cliente é obrigatório")]
        [Display(Name = "Tipo de Cliente")]
        public TipoCliente TipoCliente { get; set; }

        /*
        // Foreign Key
        [Required]
        [Display(Name = "Utente do Balneário")]
        public int UtenteBalnearioId { get; set; }
        /*
        // Navegação
        [ForeignKey(nameof(UtenteBalnearioId))]
        public UtenteBalneario? UtenteBalneario { get; set; } = null!;
        */
    }
}
