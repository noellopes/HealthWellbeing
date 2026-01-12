using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class PlanFood
    {
        [Key]
        public int PlanFoodId { get; set; }

        [Required(ErrorMessage = "Portion is required.")]
        public int PortionId { get; set; }

        [Required(ErrorMessage = "Plan is required.")]
        public int PlanId { get; set; }

        [Required(ErrorMessage = "Food is required.")]
        public int FoodId { get; set; }

        public Portion? Portion { get; set; }
        public FoodHabitsPlan? Plan { get; set; }
        public Food? Food { get; set; }
    }
}
