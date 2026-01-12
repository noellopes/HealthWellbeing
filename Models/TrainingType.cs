using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class TrainingType
    {
        public int TrainingTypeId { get; set; }

        [Required(ErrorMessage = "Training type name is required.")]
        [StringLength(100, ErrorMessage = "The name cannot exceed 100 characters.")]
        public string Name { get; set; } = default!;

        public string? Description { get; set; }

        [Range(45, 120, ErrorMessage = "Duration must be between 45 and 120 minutes.")]
        [Display(Name = "Default Duration (minutes)")]
        public int DurationMinutes { get; set; } = 60;

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Max Participants")]
        public int MaxParticipants { get; set; }
    }
}
