using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }

        // User account linking
        public string? UserId { get; set; }

        // Personal Information
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 100 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 100 characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }

        // Contact Information
        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(256, ErrorMessage = "Email cannot exceed 256 characters.")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = null!;

        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters.")]
        [Display(Name = "Phone Number")]
        public string? Phone { get; set; }

        // Emergency Contact
        [StringLength(200, ErrorMessage = "Emergency contact name cannot exceed 200 characters.")]
        [Display(Name = "Emergency Contact Name")]
        public string? EmergencyContact { get; set; }

        [Phone(ErrorMessage = "Please enter a valid emergency phone number.")]
        [StringLength(15, ErrorMessage = "Emergency phone cannot exceed 15 characters.")]
        [Display(Name = "Emergency Phone")]
        public string? EmergencyPhone { get; set; }

        // System Fields
        [Display(Name = "Registration Date")]
        [DataType(DataType.Date)]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        [Display(Name = "Active Status")]
        public bool IsActive { get; set; } = true;  // Default to true

        // Computed Property - Full Name
        [NotMapped]  // Not stored in database
        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";

        // Computed Property - Age
        [NotMapped]
        [Display(Name = "Age")]
        public int Age
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - DateOfBirth.Year;
                if (DateOfBirth.Date > today.AddYears(-age)) age--;
                return age;
            }
        }

        // Navigation Properties - Relationships
        public virtual ICollection<TherapySession>? TherapySessions { get; set; }
        public virtual ICollection<MoodEntry>? MoodEntries { get; set; }
        public virtual ICollection<Goal>? Goals { get; set; }
        public virtual ICollection<CrisisAlert>? CrisisAlerts { get; set; }
    }
}