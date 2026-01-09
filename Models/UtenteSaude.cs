using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Models
{
    // Índices únicos para identificadores
    [Index(nameof(Nif), IsUnique = true)]
    [Index(nameof(Nus), IsUnique = true)]
    [Index(nameof(Niss), IsUnique = true)]
    [Index(nameof(ClientId), IsUnique = true)] // 1-para-1: 1 Client -> no máximo 1 UtenteSaude
    public class UtenteSaude : IValidatableObject
    {
        // CHAVE
        [Key]
        public int UtenteSaudeId { get; set; }

        // FK para Client (OBRIGATÓRIO)
        [Required]
        [Display(Name = "Cliente")]
        public int ClientId { get; set; }

        // Navegação
        public Client? Client { get; set; }

        // =========================
        // DADOS DO UTENTE (PRÓPRIOS)
        // =========================

        // NIF (string + validação algorítmica Mod 11)
        [Required(ErrorMessage = "O NIF é obrigatório.")]
        [Display(Name = "NIF")]
        [PortugueseNif(ErrorMessage = "NIF inválido.")]
        [StringLength(9)]
        public string Nif { get; set; } = default!;

        // Número de Segurança Social (NISS) — 11 dígitos
        //[Required(ErrorMessage = "O NISS é obrigatório.")]
        [Display(Name = "Número de Segurança Social")]
        [StringLength(11)]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "O NISS deve ter 11 dígitos.")]
        public string? Niss { get; set; }

        // Número de Utente de Saúde (SNS/NUS) — 9 dígitos
        [Required(ErrorMessage = "O número de utente de saúde é obrigatório.")]
        [Display(Name = "Número de Utente de Saúde")]
        [StringLength(9)]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "O NUS deve ter 9 dígitos.")]
        public string Nus { get; set; } = default!;

        // ==========================================
        // CAMPOS “VINDOS” DO CLIENT (NÃO MAPEADOS)
        // ==========================================

        [NotMapped]
        [Display(Name = "Nome completo")]
        public string NomeCompleto => Client?.Name ?? string.Empty;

        [NotMapped]
        [Display(Name = "Email")]
        public string? Email => Client?.Email;

        [NotMapped]
        [Display(Name = "Telefone")]
        public string? Telefone => Client?.Phone;

        [NotMapped]
        [Display(Name = "Morada")]
        public string? Morada => Client?.Address;

        [NotMapped]
        [DataType(DataType.Date)]
        [Display(Name = "Data de nascimento")]
        public DateTime? DataNascimento => Client?.BirthDate;

        [NotMapped]
        [Display(Name = "Idade")]
        public int Idade
        {
            get
            {
                if (Client?.BirthDate == null) return 0;

                var today = DateTime.Today;
                int age = today.Year - Client.BirthDate.Value.Year;
                if (Client.BirthDate.Value.Date > today.AddYears(-age)) age--;
                return Math.Max(0, age);
            }
        }

        // Validações do UtenteSaude
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Garantir que FK não fica 0
            if (ClientId <= 0)
            {
                yield return new ValidationResult(
                    "Selecione um cliente válido.",
                    new[] { nameof(ClientId) });
            }
        }

        public ICollection<ConsultaUtente>? UtenteConsultas { get; set; }

    }

    // --------------------------
    // Validação Data no futuro
    // (mantida, caso uses noutros models)
    // --------------------------
    public sealed class DateNotInFutureAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is not DateTime dt) return true;
            return dt.Date <= DateTime.Today;
        }
    }

    // --------------------------
    // Validação oficial do NIF (Portugal) - Módulo 11
    // --------------------------
    public sealed class PortugueseNifAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is null) return true;
            var s = value.ToString()!.Trim();
            if (!System.Text.RegularExpressions.Regex.IsMatch(s, @"^\d{9}$")) return false;

            if ("1235689".IndexOf(s[0]) < 0) return false;

            int sum = 0;
            for (int i = 0; i < 8; i++)
            {
                int digit = s[i] - '0';
                int weight = 9 - i;
                sum += digit * weight;
            }

            int remainder = sum % 11;
            int check = remainder < 2 ? 0 : 11 - remainder;
            return check == (s[8] - '0');
        }
    }
}
