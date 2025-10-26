using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace HealthWellbeing.Models
{
    // Índices únicos para identificadores nacionais (opcional)
    [Index(nameof(Nif), IsUnique = true)]
    [Index(nameof(Nus), IsUnique = true)]
    [Index(nameof(Niss), IsUnique = true)]
    public class UtenteSaude : IValidatableObject
    {
        // CHAVE
        [Key]
        public int UtenteSaudeId { get; set; }

        // Nome completo
        [Required(ErrorMessage = "O nome do utente é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        [Display(Name = "Nome completo")]
        public string NomeCompleto { get; set; } = default!;

        // Data de nascimento (não pode permitir datas do futuro)
        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        [DataType(DataType.Date)]
        [DateNotInFuture(ErrorMessage = "A data de nascimento não pode ser no futuro.")]
        //[Range(typeof(DateTime), "01-01-1900", DateTime.Today, ErrorMessage = "Data fora do intervalo válido.")]
        [Display(Name = "Data de nascimento")]
        public DateTime DataNascimento { get; set; }

        // NIF (string + validação algorítmica Mod 11)
        [Required(ErrorMessage = "O NIF é obrigatório.")]
        [Display(Name = "NIF")]
        [PortugueseNif(ErrorMessage = "NIF inválido.")]
        [StringLength(9)]
        public string Nif { get; set; } = default!;

        // Número de Segurança Social (NISS) — 11 dígitos
        [Required(ErrorMessage = "O NISS é obrigatório.")]
        [Display(Name = "Número de Segurança Social")]
        [StringLength(11)]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "O NISS deve ter 11 dígitos.")]
        public string Niss { get; set; } = default!;

        // Número de Utente de Saúde (SNS/NUS) — 9 dígitos
        [Required(ErrorMessage = "O número de utente de saúde é obrigatório.")]
        [Display(Name = "Número de Utente de Saúde")]
        [StringLength(9)]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "O NUS deve ter 9 dígitos.")]
        public string Nus { get; set; } = default!;

        // Idade (calculada) — não mapeada na BD
        [NotMapped]
        [Display(Name = "Idade")]
        public int Idade
        {
            get
            {
                var today = DateTime.Today;
                int age = today.Year - DataNascimento.Year;
                if (DataNascimento.Date > today.AddYears(-age)) age--;
                return Math.Max(0, age);
            }
        }

        // Email (opcional, mas exigimos pelo menos um contacto: Email ou Telefone)
        [EmailAddress(ErrorMessage = "Formato de email inválido.")]
        [StringLength(254, ErrorMessage = "O email é demasiado longo.")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        // Telefone (opcional, lê o comentario acima)
        [Phone(ErrorMessage = "Número de telefone inválido.")]
        [StringLength(20, ErrorMessage = "O número de telefone não deve exceder 20 caracteres.")]
        [Display(Name = "Telefone")]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "O telefone deve ter 9 dígitos.")]
        public string? Telefone { get; set; }

        // Morada (opcional)
        [StringLength(200, ErrorMessage = "A morada deve ter no máximo 200 caracteres.")]
        [Display(Name = "Morada")]
        public string? Morada { get; set; }

      
        // Exigir pelo menos um meio de contacto (Email ou Telefone).
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Email) && string.IsNullOrWhiteSpace(Telefone))
            {
                yield return new ValidationResult(
                    "Indique pelo menos um contacto: Email ou Telefone.",
                    new[] { nameof(Email), nameof(Telefone) });
            }
        }
    }

    /*
    Esta classe cria uma validação personalizada chamada [DateNotInFuture], 
    que garante que uma data não é posterior à data atual.
    Ou seja, não permite datas no futuro.
    */
    public sealed class DateNotInFutureAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is not DateTime dt) return true; // [Required] trata ausência
            return dt.Date <= DateTime.Today;
        }
    }

    // Validação oficial do NIF (Portugal) com algoritmo Módulo 11.
    // Regras:
    //  - 9 dígitos
    //  - 1.º dígito ∈ {1,2,3,5,6,8,9}
    // - Dígito de controlo calculado por pesos 9..2 e Mod 11.
 
    public sealed class PortugueseNifAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is null) return true; // [Required] trata ausência
            var s = value.ToString()!.Trim();
            if (!System.Text.RegularExpressions.Regex.IsMatch(s, @"^\d{9}$")) return false;//Verifica com uma expressão regular que o NIF tem exatamente 9 dígitos numéricos.

            // Primeiro dígito válido
            if ("1235689".IndexOf(s[0]) < 0) return false;//O primeiro dígito do NIF indica o tipo de entidade (pessoa singular, empresa, etc.)e só pode ser: 1, 2, 3, 5, 6, 8 ou 9.

            // Cálculo do dígito de controlo (Mod 11)
            //Calcula a soma ponderada dos 8 primeiros dígitos do NIF com pesos de 9 até 2.
            int sum = 0;
            for (int i = 0; i < 8; i++)
            {
                int digit = s[i] - '0';
                int weight = 9 - i;
                sum += digit * weight;
            }
            /*
            Aplica o algoritmo Módulo 11:
            Divide a soma por 11;
            Se o resto for menor que 2, o dígito de controlo é 0;
            Caso contrário, o dígito de controlo é 11 - resto.
            */
            int remainder = sum % 11;
            int check = remainder < 2 ? 0 : 11 - remainder;
            /*
             Verifica se o 9.º dígito (último) do NIF coincide com o dígito de controlo calculado.
             Se for igual →NIF válido.
             Se for diferente →NIF inválido.
             */
            return check == (s[8] - '0');
        }
    }
}
