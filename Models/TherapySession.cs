using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class TherapySession
    {
        [Key]
        public int SessionId { get; set; }

        [Required(ErrorMessage = "Patient is required.")]
        [Display(Name = "Patient")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Professional is required.")]
        [Display(Name = "Mental Health Professional")]
        public int ProfessionalId { get; set; }

        [Required(ErrorMessage = "Scheduled date and time is required.")]
        [Display(Name = "Scheduled Date/Time")]
        public DateTime ScheduledDateTime { get; set; }

        [Required(ErrorMessage = "Duration is required.")]
        [Range(15, 480, ErrorMessage = "Duration must be between 15 and 480 minutes.")]
        [Display(Name = "Duration (minutes)")]
        public int DurationMinutes { get; set; }

        [Display(Name = "Status")]
        public SessionStatus Status { get; set; }

        [Display(Name = "Session Type")]
        public SessionType Type { get; set; }

        [MaxLength(5000, ErrorMessage = "Notes cannot exceed 5000 characters.")]
        [Display(Name = "Session Notes")]
        public string? Notes { get; set; }

        [Display(Name = "Completed Date/Time")]
        public DateTime? CompletedDateTime { get; set; }

        [ForeignKey("PatientId")]
        public virtual Patient? Patient { get; set; }

        [ForeignKey("ProfessionalId")]
        public virtual MentalHealthProfessional? Professional { get; set; }
    }

    public enum SessionStatus
    {
        Scheduled,
        Completed,
        Cancelled,
        NoShow,
        Rescheduled
    }

    public enum SessionType
    {
        CBT,
        Mindfulness,
        GroupTherapy,
        Individual,
        FamilyTherapy
    }
}