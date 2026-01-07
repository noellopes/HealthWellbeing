using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class Consulta
    {
        [Key] public int IdConsulta { get; set; }

        [Required(ErrorMessage = "Por favor introduza a data de marcação da consulta")]
        public DateTime DataMarcacao { get; set; }

        [Required(ErrorMessage = "Por favor introduza a data da consulta")]
        public DateTime DataConsulta { get; set; }

        public DateTime? DataCancelamento { get; set; }

        [Required(ErrorMessage = "Por favor introduza a hora de início da consulta")]
        public TimeOnly HoraInicio { get; set; }

        [Required(ErrorMessage = "Por favor introduza a hora de fim da consulta")]
        public TimeOnly HoraFim { get; set; }

        public string Estado =>
            DataCancelamento.HasValue ? "Cancelada" :
            (DataConsulta.Date < DateTime.Now) ? "Expirada" :
            (DataConsulta.Date == DateTime.Today) ? "Hoje" : "Agendada";

        public string? SearchTerm { get; set; }

        [ForeignKey(nameof(Doctor))]
        public int IdMedico { get; set; }
        public Doctor? Doctor { get; set; }

        [ForeignKey(nameof(Speciality))]
        public int IdEspecialidade { get; set; }
        public Specialities? Speciality { get; set; }

        [ForeignKey(nameof(UtenteSaude))]
        public int IdUtenteSaude { get; set; }
        public UtenteSaude? UtenteSaude { get; set; }

        [Display(Name = "Observações")]
        [MaxLength(4000, ErrorMessage = "As observações não podem ter mais de 4000 caracteres.")]
        public string? Observacoes { get; set; }

        [ForeignKey(nameof(Room))]
        public int IdSala { get; set; }
        public Room? sala { get; set; }
    }
}
