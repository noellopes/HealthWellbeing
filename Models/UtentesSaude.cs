using System;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class UtentesSaude
    {
        [Key]
        public int IdUtente { get; set; }

        // Nome completo
        [Required(ErrorMessage = "O nome do utente é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        [Display(Name = "Nome completo")]
        public string NomeCompleto { get; set; } = default!;

        // Data de nascimento
        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        [Display(Name = "Data de nascimento")]
        public DateTime DataNascimento { get; set; }

        // Número de Identificação Fiscal (NIF)
        [Required(ErrorMessage = "O NIF é obrigatório.")]
        [Display(Name = "NIF")]
        [Range(100000000, 999999999, ErrorMessage = "O NIF deve ter 9 dígitos.")]
        public int NumeroFiscal { get; set; }

        // Número de Segurança Social
        [Required(ErrorMessage = "O número de segurança social é obrigatório.")]
        [Display(Name = "Número de Segurança Social")]
        public int NumeroSegurancaSocial { get; set; }

        // Número de Utente do SNS
        [Required(ErrorMessage = "O número de utente de saúde é obrigatório.")]
        [Display(Name = "Número de Utente de Saúde")]
        public int NumeroUtenteSaude { get; set; }

        // Idade (calculada)
        [Display(Name = "Idade")]
        public int Idade { get; set; }

        // Email
        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de email inválido.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = default!;

        // Telefone
        [Required(ErrorMessage = "O telefone é obrigatório.")]
        [Phone(ErrorMessage = "Número de telefone inválido.")]
        [StringLength(15, ErrorMessage = "O número de telefone não deve exceder 15 caracteres.")]
        [Display(Name = "Telefone")]
        public string Telefone { get; set; } = default!;

        // Morada
        [Required(ErrorMessage = "A morada é obrigatória.")]
        [StringLength(200, ErrorMessage = "A morada deve ter no máximo 200 caracteres.")]
        [Display(Name = "Morada")]
        public string Morada { get; set; } = default!;

        // Observações / descrição
        [StringLength(300, ErrorMessage = "As observações devem ter no máximo 300 caracteres.")]
        [Display(Name = "Observações (opcional)")]
        public string? Observacoes { get; set; }
    }
}
