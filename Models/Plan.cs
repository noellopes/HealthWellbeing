using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Plan
    {
        [Key]
        public int PlanId { get; set; }

        [Required(ErrorMessage = "Starting date is required.")]
        [DataType(DataType.Date)]
        public DateTime StartingDate { get; set; }

        [Required(ErrorMessage = "Ending date is required.")]
        [DataType(DataType.Date)]
        public DateTime EndingDate { get; set; }

        public bool Done { get; set; }

        public ICollection<NutritionistClientPlan>? NutritionistClientPlans { get; set; }
        public ICollection<FoodPlan>? FoodPlans { get; set; }
    }
}
