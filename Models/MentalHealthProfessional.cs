using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [MaxLength(15, ErrorMessage = "Phone number cannot exceed 15 characters.")]
        [Display(Name = "Phone Number")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Professional type is required.")]
        [Display(Name = "Professional Type")]
        public ProfessionalType Type { get; set; }

        [Required(ErrorMessage = "Specialization is required.")]
        [Display(Name = "Specialization")]
        public Specialization Specialization { get; set; }  // CHANGED: Now enum

        [MaxLength(50, ErrorMessage = "License number cannot exceed 50 characters.")]
        [Display(Name = "License Number")]
        public string? LicenseNumber { get; set; }

        [Display(Name = "Active Status")]
        public bool IsActive { get; set; } = true;

        // Computed Property
        [NotMapped]
        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";

        // Navigation properties
        public virtual ICollection<TherapySession>? TherapySessions { get; set; }
    }

    public enum ProfessionalType
    {
        [Display(Name = "Psychologist")]
        Psychologist,

        [Display(Name = "Therapist")]
        Therapist,

        [Display(Name = "Psychiatrist")]
        Psychiatrist,

        [Display(Name = "Life Coach")]
        Coach,

        [Display(Name = "Counselor")]
        Counselor
    }

    public enum Specialization
    {
        [Display(Name = "Anxiety & Stress Management")]
        AnxietyStress,

        [Display(Name = "Depression Treatment")]
        Depression,

        [Display(Name = "Trauma & PTSD")]
        TraumaPTSD,

        [Display(Name = "Addiction Recovery")]
        Addiction,

        [Display(Name = "Family Therapy")]
        FamilyTherapy,

        [Display(Name = "Child & Adolescent Psychology")]
        ChildPsychology,

        [Display(Name = "Couples Counseling")]
        CouplesCounseling,

        [Display(Name = "Eating Disorders")]
        EatingDisorders,

        [Display(Name = "Grief & Loss")]
        GriefLoss,

        [Display(Name = "General Mental Health")]
        General
    }
}