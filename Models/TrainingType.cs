using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class TrainingType
    {
        public int TrainingTypeId { get; set; }

        [Required(ErrorMessage = "The training name is required")]
        [StringLength(100, ErrorMessage = "The name cannot exceed 100 characters")]
        public string Name { get; set; } = default!;

        [StringLength(500, ErrorMessage = "The description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [Range(10, 300, ErrorMessage = "The duration must be between 10 and 300 minutes")]
        public int DurationMinutes { get; set; } = 60;

        [Required(ErrorMessage = "The intensity is required")]
        [StringLength(20)]
        public string Intensity { get; set; } = "Moderate";

        public bool IsActive { get; set; } = true;
    }
}