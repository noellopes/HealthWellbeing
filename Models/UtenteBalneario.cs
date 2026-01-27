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

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Nascimento")]
        public DateTime DataNascimento { get; set; }

        [Required(ErrorMessage = "O género é obrigatório.")]
        [Display(Name = "Género")]
        public string Genero { get; set; }

        [Required(ErrorMessage = "O NIF é obrigatório.")]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "O NIF deve ter 9 dígitos.")]
        public string NIF { get; set; }

        [Required(ErrorMessage = "O contacto é obrigatório.")]
        [Phone]
        public string Contacto { get; set; }

        [StringLength(200)]
        public string Morada { get; set; }

        // =========================
        // Dados Médicos
        // =========================

        [Display(Name = "Histórico Clínico")]
        public string HistoricoClinico { get; set; }

        [Display(Name = "Indicações Terapêuticas")]
        public string IndicacoesTerapeuticas { get; set; }

        [Display(Name = "Contraindicações / Alergias")]
        public string ContraIndicacoes { get; set; }

        [Display(Name = "Médico Responsável")]
        public string MedicoResponsavel { get; set; }

        // =========================
        // Dados Administrativos
        // =========================

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Inscrição")]
        public DateTime DataInscricao { get; set; } = DateTime.Now;

        [Display(Name = "Seguro de Saúde")]
        public string SeguroSaude { get; set; }

        [Display(Name = "Ativo")]
        public bool Ativo { get; set; } = true;
    }
}
