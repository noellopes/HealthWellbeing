using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Plan
    {
        [Key]
        public int PlanId { get; set; }

        [Required]
        public int GoalId { get; set; }
        public Goal? Goal { get; set; }

        [Required]
        public int PatientId { get; set; }  // FK to UtenteSaude

        [Required]
        public int FoodId { get; set; }
        public Food? Food { get; set; }

        [StringLength(300)]
        public string? Description { get; set; }

        public int Quantity { get; set; }

        public int? NutritionistId { get; set; }
        public Nutritionist? Nutritionist { get; set; }
    }
}
