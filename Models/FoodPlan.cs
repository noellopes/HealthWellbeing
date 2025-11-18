using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class FoodPlan
    {
        [Key]
        public int FoodPlanId { get; set; }

        [Required]
        [Display(Name = "Client")]
        public string ClientId { get; set; } = string.Empty;
        public Client? Client { get; set; }

        [Required]
        [Display(Name = "Goal")]
        public int GoalId { get; set; }
        public Goal? Goal { get; set; }

        [Required]
        [Display(Name = "Food")]
        public int FoodId { get; set; }
        public Food? Food { get; set; }


        [Range(0.01, 999999.99, ErrorMessage = "Please enter a valid quantity.")]
        [Column(TypeName = "decimal(9,2)")]
        [Display(Name = "Quantity (g/ml)")]
        public decimal Quantity { get; set; }

        [StringLength(200)]
        [Display(Name = "Description / Notes")]
        public string? Description { get; set; }

        [Display(Name = "Nutritionist")]
        public int? NutritionistId { get; set; }
        public Nutritionist? Nutritionist { get; set; }
    }
}
