using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class MentalHealthProfessional
    {
        [Key]
        public int ProfessionalId { get; set; }

        public string? UserId { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(100, ErrorMessage = "First name cannot exceed 100 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(100, ErrorMessage = "Last name cannot exceed 100 characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = null!;

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