using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class UtentesSaude
    {
        [Key]
        public int IdUtente { get; set; }

        // Nome completo
        [Required(ErrorMessage = "O nome do Utente é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        [Display(Name = "Nome do Utente")]
        public string Nome { get; set; } = default!;

        // Data de Nascimento
        [Required(ErrorMessage = "A Data de Nascimento é obrigatória")]
        [Display(Name = "Data de Nascimento")]
        public DateTime DataDeNascimento { get; set; }

        // Identificação Fiscal (NIF)
        [Required(ErrorMessage = "O NIF é obrigatório")]
        [Display(Name = "NIF")]
        [Range(100000000, 999999999, ErrorMessage = "O NIF deve ter 9 dígitos")]
        public int NIF { get; set; }

        // Número de Segurança Social
        [Required(ErrorMessage = "O Número de Segurança Social é obrigatório")]
        [Display(Name = "Nº Segurança Social")]
        public int NSS { get; set; }

        // Número de Utente de Saúde
        [Required(ErrorMessage = "O Número de Utente de Saúde é obrigatório")]
        [Display(Name = "Nº Utente de Saúde")]
        public int NUS { get; set; }

        // Idade (pode ser calculada a partir da Data de Nascimento)
        [Display(Name = "Idade")]
        public int Idade { get; set; }

        // Email
        [Required(ErrorMessage = "O Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [Display(Name = "Email")]
        public string Email { get; set; } = default!;

        // Telefone
        [Required(ErrorMessage = "O Telefone é obrigatório")]
        [Phone(ErrorMessage = "Número de telefone inválido")]
        [StringLength(15, ErrorMessage = "O número de telefone não deve exceder 15 caracteres")]
        [Display(Name = "Telefone")]
        public string Telefone { get; set; } = default!;

        // Morada
        [Required(ErrorMessage = "A Morada é obrigatória")]
        [StringLength(200, ErrorMessage = "A morada deve ter no máximo 200 caracteres")]
        [Display(Name = "Morada")]
        public string Morada { get; set; } = default!;

        // Descrição
        [StringLength(300, ErrorMessage = "A descrição deve ter no máximo 300 caracteres")]
        [Display(Name = "Descrição (opcional)")]
        public string? Descricao { get; set; }
    }

}
