using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HealthWellBeing.Models
{
    public class Utente
    {
        // 1. Chave Primária (Seguindo a convenção e o diagrama)
        public int UtenteId { get; set; }

        // 2. Outras propriedades
        [Required(ErrorMessage = "O número de utente SNS é obrigatório.")]
        public int UtenteSNS { get; set; }

        [Required(ErrorMessage = "O NIF é obrigatório.")]
        [StringLength(9, ErrorMessage = "O NIF deve ter 9 dígitos.")]
        public string Nif { get; set; } = string.Empty;

        [Required(ErrorMessage = "O número do Cartão de Cidadão é obrigatório.")]
        [StringLength(20)]
        public string NumCC { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Genero { get; set; }

        [StringLength(200)]
        public string? Morada { get; set; }

        [StringLength(10)]
        [Display(Name = "Código Postal")]
        public string? CodigoPostal { get; set; }

        // "Número" no diagrama pode ser o telefone ou o número da porta, assumimos telefone para contato.
        [Phone(ErrorMessage = "Número de telefone inválido.")]
        [Display(Name = "Telefone")]
        public string? Numero { get; set; }

        [EmailAddress(ErrorMessage = "Endereço de e-mail inválido.")]
        public string? Email { get; set; }

        [StringLength(100)]
        [Display(Name = "Nome de Emergência")]
        public string? NomeEmergencia { get; set; }

        [Display(Name = "Telefone de Emergência")]
        public int? NumeroEmergencia { get; set; }

        [StringLength(50)]
        public string? Estado { get; set; } // O estado do utente (ativo/inativo)

        // Propriedade de Navegação (para a relação 1-N com Exame)
        public ICollection<Exame>? Exames { get; set; }
    }
}