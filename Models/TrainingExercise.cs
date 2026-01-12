using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class TrainingExercise
    {
        [Key]
        public int TrainingExerciseId { get; set; }

        // FK1: Ligação ao Treino
        [ForeignKey("Training")]
        public int TrainingId { get; set; }
        public Training? Training { get; set; }

        // FK2: Ligação ao Exercício
        [ForeignKey("Exercise")]
        public int ExerciseId { get; set; }
        public Exercise? Exercise { get; set; }

        // Atributos específicos desta ligação
        [Required]
        public int Sets { get; set; } // Séries

        [Required]
        public int Reps { get; set; } // Repetições

        [Display(Name = "Rest Time")]
        public string RestTime { get; set; } // Ex: "60s", "90s"
    }
}