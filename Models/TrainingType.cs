using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class TrainingType
    {
        public int TrainingTypeId { get; set; }

        [Required(ErrorMessage = "The training name is required")]
        [StringLength(100, ErrorMessage = "The name cannot exceed 100 characters")]
        public string Name { get; set; } = default!;

        public string? Description { get; set; }

        [Range(45, 120, ErrorMessage = "The duration must be between 45 and 120 minutes")]
        public int DurationMinutes { get; set; } = 60;

        [Required(ErrorMessage = "The 'Active' status is required.")]
        public bool IsActive { get; set; } = true;
    }
}