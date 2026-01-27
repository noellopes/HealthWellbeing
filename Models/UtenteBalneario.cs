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
        public string Nome { get; set; }


        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Nascimento")]
        public DateTime DataNascimento { get; set; }

        [Required(ErrorMessage = "Selecione o género.")]
        [Display(Name = "Género")]
        public string Genero { get; set; }



        [Required(ErrorMessage = "O NIF é obrigatório.")]
        [RegularExpression(@"^[1-9][0-9]{8}$", ErrorMessage = "NIF inválido.")]
        [Display(Name = "NIF")]
        public string NIF { get; set; }


        [Required(ErrorMessage = "O contacto é obrigatório.")]
        [RegularExpression(@"^(91|92|93|96)[0-9]{7}$",
        ErrorMessage = "Número inválido. Deve começar por 91, 92, 93 ou 96.")]
        [Display(Name = "Contacto")]
        public string Contacto { get; set; }



        [StringLength(200)]
        public string Morada { get; set; }

        // =========================
        // Dados Médicos
        // =========================

        [Display(Name = "Histórico Clínico")]
        public string? HistoricoClinico { get; set; }

        [Display(Name = "Indicações Terapêuticas")]
        public string? IndicacoesTerapeuticas { get; set; }

        [Display(Name = "Contraindicações / Alergias")]
        public string? ContraIndicacoes { get; set; }

        [Display(Name = "Médico Responsável")]
        public string? MedicoResponsavel { get; set; }

        // =========================
        // Dados Administrativos
        // =========================

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Inscrição")]
        public DateTime DataInscricao { get; set; } = DateTime.Now;

        [Display(Name = "Seguro de Saúde")]
        public string? SeguroSaude { get; set; }

        [Display(Name = "Ativo")]
        public bool Ativo { get; set; } = true;
    }
}
