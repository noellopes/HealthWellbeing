using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace HealthWellbeing.Models
{
    public class Food
    {
        [Key]
        public int FoodId { get; set; }

        [Required, StringLength(120)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public int FoodCategoryId { get; set; }
        public FoodCategory? FoodCategory { get; set; }

        public ICollection<FoodNutrient>? FoodNutrients { get; set; }
        public ICollection<FoodPlan>? FoodPlans { get; set; }
    }
}
