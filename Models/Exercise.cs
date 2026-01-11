using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Exercise
    {
        public int ExerciseId { get; set; }

        [Required(ErrorMessage = "Exercise name is required.")]
        [StringLength(100)]
        [Display(Name = "Exercise Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Muscle group is required.")]
        [Display(Name = "Muscle Group")]
        public string MuscleGroup { get; set; } = string.Empty; // Ex: Chest, Legs, Back

        [Display(Name = "Equipment")]
        public string? Equipment { get; set; } // Ex: Dumbbells, Machine, None

        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }
    }
}