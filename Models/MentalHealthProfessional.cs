using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class MentalHealthProfessional
    {
        [Key]
        public int ProfessionalId { get; set; }

        public string? UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string? Phone { get; set; }

        public ProfessionalType Type { get; set; }

        [MaxLength(200)]
        public string? Specialization { get; set; }

        [MaxLength(100)]
        public string? LicenseNumber { get; set; }

        public bool IsActive { get; set; }

        // Navigation properties
        public virtual ICollection<TherapySession>? TherapySessions { get; set; }
    }

    public enum ProfessionalType
    {
        Psychologist,
        Therapist,
        Psychiatrist,
        Coach,
        Counselor
    }
}