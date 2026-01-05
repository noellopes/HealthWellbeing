using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HealthWellbeing.Models;

namespace HealthWellbeing.ViewModel
{
    public class UtenteSaudeFormVM : IValidatableObject
    {
        public bool IsEdit { get; set; }

        public int UtenteSaudeId { get; set; }

        // No Create pode ser null (quando vais criar Client + Utente)
        public int? ClientId { get; set; }

        // =======================
        // CLIENT
        // =======================
        [StringLength(100, MinimumLength = 6, ErrorMessage = "O nome deve ter entre 6 e 100 caracteres.")]
        [RegularExpression(
            @"^[A-Za-zÁÉÍÓÚÂÊÔÃÕÇáéíóúâêôãõç]+(?:\s[A-Za-zÁÉÍÓÚÂÊÔÃÕÇáéíóúâêôãõç]+)+$",
            ErrorMessage = "Introduz pelo menos Primeiro e Último nome."
        )]
        [Display(Name = "Nome")]
        public string? Name { get; set; }

        [EmailAddress(ErrorMessage = "Introduz um email válido.")]
        [StringLength(100)]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Introduz um telefone válido.")]
        [StringLength(15)]
        [Display(Name = "Telefone")]
        public string? Phone { get; set; }

        [StringLength(200)]
        [Display(Name = "Morada")]
        public string? Address { get; set; }

        [DataType(DataType.Date)]
        [DateNotInFuture(ErrorMessage = "A data de nascimento não pode estar no futuro.")]
        [Display(Name = "Data de nascimento")]
        public DateTime? BirthDate { get; set; }

        [StringLength(10)]
        [Display(Name = "Género")]
        public string? Gender { get; set; }

        [Display(Name = "Idade")]
        public int Age
        {
            get
            {
                if (!BirthDate.HasValue) return 0;
                var today = DateTime.Today;
                int age = today.Year - BirthDate.Value.Year;
                if (BirthDate.Value.Date > today.AddYears(-age)) age--;
                return Math.Max(0, age);
            }
        }

        // =======================
        // UTENTE
        // =======================
        [Required(ErrorMessage = "O NIF é obrigatório.")]
        [Display(Name = "NIF")]
        [PortugueseNif(ErrorMessage = "NIF inválido.")]
        [StringLength(9)]
        public string Nif { get; set; } = string.Empty;

        [Required(ErrorMessage = "O NISS é obrigatório.")]
        [Display(Name = "Número de Segurança Social")]
        [StringLength(11)]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "O NISS deve ter 11 dígitos.")]
        public string Niss { get; set; } = string.Empty;

        [Required(ErrorMessage = "O NUS é obrigatório.")]
        [Display(Name = "Número de Utente de Saúde")]
        [StringLength(9)]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "O NUS deve ter 9 dígitos.")]
        public string Nus { get; set; } = string.Empty;

        // =======================
        // Validação condicional
        // =======================
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // No Edit: ClientId tem de existir e não pode mudar
            if (IsEdit && (!ClientId.HasValue || ClientId.Value <= 0))
                yield return new ValidationResult("Cliente inválido no Edit.", new[] { nameof(ClientId) });

            // No Create:
            // - Se NÃO escolheu cliente (ClientId null), então estás a criar Client + Utente, logo Name/Email/Phone obrigatórios.
            if (!IsEdit && (!ClientId.HasValue || ClientId.Value <= 0))
            {
                if (string.IsNullOrWhiteSpace(Name))
                    yield return new ValidationResult("O nome é obrigatório.", new[] { nameof(Name) });

                if (string.IsNullOrWhiteSpace(Email))
                    yield return new ValidationResult("O email é obrigatório.", new[] { nameof(Email) });

                if (string.IsNullOrWhiteSpace(Phone))
                    yield return new ValidationResult("O telefone é obrigatório.", new[] { nameof(Phone) });
            }

            // No Edit: como estás a editar o Client, obriga também Email/Phone/Name
            if (IsEdit)
            {
                if (string.IsNullOrWhiteSpace(Name))
                    yield return new ValidationResult("O nome é obrigatório.", new[] { nameof(Name) });

                if (string.IsNullOrWhiteSpace(Email))
                    yield return new ValidationResult("O email é obrigatório.", new[] { nameof(Email) });

                if (string.IsNullOrWhiteSpace(Phone))
                    yield return new ValidationResult("O telefone é obrigatório.", new[] { nameof(Phone) });
            }
        }
    }
}
