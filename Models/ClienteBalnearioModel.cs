using HealthWellbeing.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class ClienteBalnearioModel
    {
        [Key]
        public int ClienteBalnearioId { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "Nome Completo")]
        public string NomeCompleto { get; set; }


        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, StringLength(9)]
        public string Telemovel { get; set; }

        [Required, StringLength(200)]
        public string Morada { get; set; }

        [Required]
        public string TipoCliente { get; set; }

        // FK clara
        public int UtenteBalnearioId { get; set; }

        [ForeignKey(nameof(UtenteBalnearioId))]
        public UtenteBalneario UtenteBalneario { get; set; }
    }
}