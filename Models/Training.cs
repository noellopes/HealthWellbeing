using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public enum WeekDay
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday
    }

    public class Training
    {
        public int TrainingId { get; set; }

        [Required(ErrorMessage = "Trainer is required.")]
        [Display(Name = "Trainer")]
        public int TrainerId { get; set; }

        [ForeignKey(nameof(TrainerId))]
        public Trainer Trainer { get; set; } = default!;

        [Required(ErrorMessage = "Training type is required.")]
        [Display(Name = "Training Type")]
        public int TrainingTypeId { get; set; }

        [ForeignKey(nameof(TrainingTypeId))]
        public TrainingType TrainingType { get; set; } = default!;

        [Required(ErrorMessage = "Training name is required.")]
        [StringLength(150, ErrorMessage = "The name cannot exceed 150 characters.")]
        public string Name { get; set; } = default!;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Duration is required.")]
        [Range(45, 120, ErrorMessage = "Duration must be between 45 and 120 minutes.")]
        public int Duration { get; set; }

        [Required(ErrorMessage = "Day of the week is required.")]
        [Display(Name = "Day of the Week")]
        public WeekDay DayOfWeek { get; set; }

        [Required(ErrorMessage = "Start time is required.")]
        [Display(Name = "Start Time")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }
    }
}
