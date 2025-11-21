using HealthWellbeing.Utils.Group1.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class TreatmentRecord : IBaseEntity<Guid>
    {
        // Relationships
        // Patient
        /*
        [Required(ErrorMessage = "The Patient is required.")]
        [Display(Name = "Patient")]
        public int PatientId { get; set; }

        [ForeignKey(nameof(PatientId))]
        public HealthPatient? Patient { get; set; }
        */
        [Required(ErrorMessage = "É necessário um(a) enfermeiro(a) responsável.")]
        [Display(Name = "Enfermeiro(a) Responsável")]
        public int NurseId { get; set; }

        [ForeignKey(nameof(NurseId))]
        [Display(Name = "Enfermeiro(a) Responsável")]
        public Nurse? Nurse { get; set; }

        [Required(ErrorMessage = "É necessário especificar o tipo de tratamento.")]
        [Display(Name = "Tipo de tratamento")]
        public int TreatmentTypeId { get; set; }

        [ForeignKey(nameof(TreatmentTypeId))]
        [Display(Name = "Tipo de tratamento")]
        public TreatmentType? TreatmentType { get; set; }

        [Display(Name = "Patologia associada")]
        public int? PathologyId { get; set; }

        [ForeignKey(nameof(PathologyId))]
        [Display(Name = "Patologia associada")]
        public Pathology? Pathology { get; set; }

        // Treatment information
        [Required(ErrorMessage = "É necessário informar a data pretendida do tratamento.")]
        [Display(Name = "Data pretendida para o tratamento")]
        [DataType(DataType.Date)]
        public DateTime TreatmentDate { get; set; }

        [StringLength(300, ErrorMessage = "As notas adicionais não devem exceder os 300 caracteres.")]
        [Display(Name = "Notas adicionais")]
        public string? AdditionalNotes { get; set; }

        [Display(Name = "Duração estimada do tratamento")]
        public int? EstimatedDuration { get; set; }

        [StringLength(500, ErrorMessage = "As observações não devem exceder os 500 caracteres")]
        [Display(Name = "Observações")]
        public string? Observations { get; set; }

        [Display(Name = "Estado do tratamento")]
        public TreatmentStatus Status { get; set; } = TreatmentStatus.Scheduled;

        [Display(Name = "Duração total do tratamento")]
        public int? CompletedDuration { get; set; }

        // Cancellation details
        [StringLength(500, ErrorMessage = "O motivo do cancelamento não deve exceder 500 caracteres.")]
        [Display(Name = "Motivo do cancelamento")]
        public String? CanceledReason { get; set; }

        [Display(Name = "Data de cancelamento")]
        public DateTime? CanceledAt { get; set; }
    }

    public enum TreatmentStatus
    {
        [Display(Name = "Agendado (Planeado)")]
        Scheduled,

        [Display(Name = "Em progresso")]
        InProgress,

        [Display(Name = "Concluído")]
        Completed,

        [Display(Name = "Cancelado")]
        Canceled
    }
}
