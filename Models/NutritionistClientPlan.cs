using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class NutritionistClientPlan
    {
        [Key]
        public int PlanClientId { get; set; }

        [Required(ErrorMessage = "Client is required.")]
        public int ClientId { get; set; }

        [Required(ErrorMessage = "Plan is required.")]
        public int PlanId { get; set; }

        [Required(ErrorMessage = "Nutritionist is required.")]
        public int NutritionistId { get; set; }

        public Client? Client { get; set; }
        public FoodHabitsPlan? Plan { get; set; }
        public Nutritionist? Nutritionist { get; set; }
    }
}
