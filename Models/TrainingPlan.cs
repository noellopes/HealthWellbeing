using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class TrainingPlan
    {
        public int TrainingPlanId { get; set; }

        // Chaves Estrangeiras (Foreign Keys)
        public int TrainingId { get; set; }
        public int PlanId { get; set; }
        public Training? Training { get; set; }
        public Plan? Plan { get; set; }

        // Atributos Específicos
        [Required(ErrorMessage = "Days Per Week is required.")]
        [Range(1, 7, ErrorMessage = "Days per week must be between 1 and 7.")]
        [Display(Name = "Days Per Week")]
        public int DaysPerWeek { get; set; }
    }
}