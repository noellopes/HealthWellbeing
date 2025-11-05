using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class FoodPlan
    {
        [Key]
        public int FoodPlanId { get; set; }

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
