using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class TrainingType
    {
        public int TrainingTypeId { get; set; }

        [Required(ErrorMessage = "Training type name is required.")]
        [StringLength(100, ErrorMessage = "The name cannot exceed 100 characters.")]
        public string Name { get; set; } = default!;

        [StringLength(300, ErrorMessage = "Description cannot exceed 300 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Maximum participants is required.")]
        [Range(1, 50, ErrorMessage = "Max participants must be between 1 and 50.")]
        [Display(Name = "Max Participants")]
        public int MaxParticipants { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Max Participants")]
        public int MaxParticipants { get; set; }
    }
}
