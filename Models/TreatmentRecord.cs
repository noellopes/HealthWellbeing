using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class TreatmentRecord
    {
        [Key]
        public int Id { get; set; }

        // Relationship with the Patient
        /*[Required(ErrorMessage = "The Patient is required.")]
        [Display(Name = "Patient")]
        public int PatientId { get; set; }

        [ForeignKey(nameof(PatientId))]
        public HealthPatient? Patient { get; set; }*/

        // Relationship with the Nurse
        [Required(ErrorMessage = "The responsible Nurse is required.")]
        [Display(Name = "Responsible Nurse")]
        public int NurseId { get; set; }

        [ForeignKey(nameof(NurseId))]
        public Nurse? Nurse { get; set; }

        // Treatment Type
        [Required(ErrorMessage = "The treatment type is required.")]
        [Display(Name = "Treatment Type")]
        public int TreatmentId { get; set; }

        [ForeignKey(nameof(TreatmentId))]
        public TreatmentType? TreatmentType { get; set; }

        // Associated pathology (optional)
        [Display(Name = "Pathology")]
        public int? PathologyId { get; set; }

        [ForeignKey(nameof(PathologyId))]
        public Pathology? Pathology { get; set; }

        // Date and time of session
        [Required(ErrorMessage = "The treatment date is required.")]
        [Display(Name = "Treatment Date")]
        [DataType(DataType.Date)]
        public DateTime TreatmentDate { get; set; }

        // Duration (minutes)
        [Display(Name = "Duration (minutes)")]
        public int? DurationMinutes { get; set; }

        // Description and remarks
        [StringLength(500, ErrorMessage = "Remarks must be at most 500 characters long.")]
        [Display(Name = "Remarks / Description")]
        public string? Remarks { get; set; }

        // Treatment result
        [StringLength(300, ErrorMessage = "The result must be at most 300 characters long.")]
        [Display(Name = "Result / Progress")]
        public string? Result { get; set; }

        // Treatment status
        [Display(Name = "Treatment Status")]
        public TreatmentStatus Status { get; set; } = TreatmentStatus.Scheduled;

        // Record creation date
        [Display(Name = "Record Date")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public enum TreatmentStatus
    {
        Scheduled,
        InProgress,
        Completed,
        Canceled
    }
}
