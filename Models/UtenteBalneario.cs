using System;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class UtenteBalneario
    {
        [Key]
        public int UtenteBalnearioId { get; set; }

        // =========================
        // Dados Pessoais
        // =========================

        [Required(ErrorMessage = "O nome completo é obrigatório.")]
        [StringLength(150)]
        [Display(Name = "Nome Completo")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Nascimento")]
        public DateTime DataNascimento { get; set; }

        [Required(ErrorMessage = "Selecione o género.")]
        [Display(Name = "Género")]
        public int GeneroId { get; set; }

        public Genero? Genero { get; set; } = null!;

        [Required(ErrorMessage = "O NIF é obrigatório.")]
        [RegularExpression(@"^[1-9][0-9]{8}$", ErrorMessage = "NIF inválido.")]
        public string NIF { get; set; } = string.Empty;

        [Required(ErrorMessage = "O contacto é obrigatório.")]
        [RegularExpression(@"^(91|92|93|96)[0-9]{7}$",
            ErrorMessage = "Número inválido. Deve começar por 91, 92, 93 ou 96.")]
        public string Contacto { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Morada { get; set; }

        // =========================
        // Dados Médicos
        // =========================
        public string? HistoricoClinico { get; set; }
        public string? IndicacoesTerapeuticas { get; set; }
        public string? ContraIndicacoes { get; set; }

        [Display(Name = "Terapeuta Responsável")]
        public string? TerapeutaResponsavel { get; set; }


        //Sem haver BD dos Terapeutas
        /*
        public int? FisioterapeutaId { get; set; }
        public Fisioterapeuta? Fisioterapeuta { get; set; }
        */


        // =========================
        // Dados Administrativos
        // =========================
        [DataType(DataType.Date)]
        public DateTime DataInscricao { get; set; } = DateTime.Now;

        public string? SeguroSaude { get; set; }
        public bool Ativo { get; set; } = true;
    }
}
