using HealthWellbeing.Models.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class UtenteBalneario
    {
        [Key]
        public int UtenteBalnearioId { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100)]
        public string NomeCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "A data de nascimento é obrigatória")]
        [DataType(DataType.Date)]
        public DateTime DataNascimento { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Selecione o sexo")]
        public Sexo Sexo { get; set; }

        [Required(ErrorMessage = "O NIF é obrigatório")]
        [Nif(ErrorMessage = "O NIF é inválido.")]
        public string NIF { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^(91|92|93|96)\d{7}$",
            ErrorMessage = "Número inválido (91, 92, 93 ou 96 + 7 dígitos)")]
        public string Contacto { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Morada { get; set; } = string.Empty;

        public DateTime DataInscricao { get; set; }
        public bool Ativo { get; set; } = true;

        // FKs nullable
        [ValidateNever]
        public int DadosMedicosId { get; set; }

        [ValidateNever]
        public DadosMedicos DadosMedicos { get; set; }

        [ValidateNever]
        public int SeguroSaudeId { get; set; }

        [ValidateNever]
        public SeguroSaude SeguroSaude { get; set; }
    }

    public enum Sexo
    {
        Nenhum = 0,
        Masculino = 1,
        Feminino = 2,
        Outro = 3
    }

    public class DadosMedicos
    {
        [Key]
        public int DadosMedicosId { get; set; }

        public string HistoricoClinico { get; set; } = string.Empty;
        public string IndicacoesTerapeuticas { get; set; } = string.Empty;
        public string ContraIndicacoes { get; set; } = string.Empty;

        [StringLength(100)]
        public string MedicoResponsavel { get; set; } = string.Empty;

        public UtenteBalneario UtenteBalneario { get; set; }
    }

    public class SeguroSaude
    {
        [Key]
        public int SeguroSaudeId { get; set; }

        [StringLength(100)]
        public string NomeSeguradora { get; set; } = string.Empty;

        [StringLength(50)]
        public string NumeroApolice { get; set; } = string.Empty;

        public UtenteBalneario UtenteBalneario { get; set; }
    }
}
