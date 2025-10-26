using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Client
    {
        [Key]
        public int ClientId { get; set; }

        public string? UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string? Phone { get; set; }

        [MaxLength(200)]
        public string? EmergencyContact { get; set; }

        [Phone]
        public string? EmergencyPhone { get; set; }

        public DateTime RegistrationDate { get; set; }

        public bool IsActive { get; set; }

        // Navigation properties
        public virtual ICollection<TherapySession>? TherapySessions { get; set; }
        public virtual ICollection<MoodEntry>? MoodEntries { get; set; }
        public virtual ICollection<Goal>? Goals { get; set; }
        public virtual ICollection<CrisisAlert>? CrisisAlerts { get; set; }
    }
}