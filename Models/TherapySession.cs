using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class TherapySession
    {
        [Key]
        public int SessionId { get; set; }

        [Required]
        public int ClientId { get; set; }

        [Required]
        public int ProfessionalId { get; set; }

        [Required]
        public DateTime ScheduledDateTime { get; set; }

        public int DurationMinutes { get; set; }

        public SessionStatus Status { get; set; }

        public SessionType Type { get; set; }

        [MaxLength(5000)]
        public string? Notes { get; set; }

        public DateTime? CompletedDateTime { get; set; }

        // Navigation properties
        [ForeignKey("ClientId")]
        public virtual Client? Client { get; set; }

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