using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System; 

namespace HealthWellbeing.Models
{
    public class Training
    {
        public int TrainingId { get; set; }

        [Required(ErrorMessage = "The Trainer is required.")]
        [Display(Name = "Trainer")] 
        public int TrainerId { get; set; }

        [ForeignKey("TrainerId")]
        public Trainer Trainer { get; set; } = default!;

        [Required(ErrorMessage = "The Training Type is required.")]
        [Display(Name = "Training Type")]
        public int TrainingTypeId { get; set; }

        [ForeignKey("TrainingTypeId")]
        public TrainingType TrainingType { get; set; } = default!;

        [Required(ErrorMessage = "The Session Name is required.")]
        [StringLength(150, ErrorMessage = "The name cannot exceed 150 characters.")]
        public string Name { get; set; } = default!;

        public string? Description { get; set; }

        [Required(ErrorMessage = "The duration is required.")]
        [Range(45, 120, ErrorMessage = "The duration must be between 45 and 120 minutes.")]
        public int Duration { get; set; }

        [Required(ErrorMessage = "The Day of the Week is required.")]
        [Display(Name = "Day of the Week")] 
        public string DayOfWeek { get; set; } = default!;

        [Required(ErrorMessage = "The Start Time is required.")]
        [Display(Name = "Start Time")] 
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Display(Name = "Maximum Participants")] 
        [Range(1, 50, ErrorMessage = "Capacity must be between 1 and 50.")]
        public int MaxParticipants { get; set; } = 10;
    }
}
