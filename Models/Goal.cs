using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Goal
    {
        [Key]
        public int GoalId { get; set; }

        // Ligação ao Cliente (substitui Patient)
        [Required]
        [Display(Name = "Client")]
        public string ClientId { get; set; } = string.Empty;
        public Client? Client { get; set; }

        [Required, StringLength(60)]
        [Display(Name = "Goal Type")]
        public string GoalType { get; set; } = string.Empty;

        [Display(Name = "Daily Calories (kcal)")]
        public int? DailyCalories { get; set; }

        [Display(Name = "Daily Protein (g)")]
        public int? DailyProtein { get; set; }

        [Display(Name = "Daily Fat (g)")]
        public int? DailyFat { get; set; }

        [Display(Name = "Daily Carbohydrates (g)")]
        public int? DailyCarbs { get; set; }

        [Display(Name = "Daily Fiber (g)")]
        public int? DailyFiber { get; set; }

        [Display(Name = "Daily Vitamins (mg)")]
        public int? DailyVitamins { get; set; }

        [Display(Name = "Daily Minerals (mg)")]
        public int? DailyMinerals { get; set; }

        // Planos alimentares associados a este objetivo
        public ICollection<FoodPlan>? FoodPlans { get; set; }
    }
}
