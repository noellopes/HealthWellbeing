using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace HealthWellbeing.Models
{
    public class Food
    {
        [Key]
        public int FoodId { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Food name is required.")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public FoodCategory? Category { get; set; }
        public ICollection<FoodNutritionalComponent>? FoodNutritionalComponents { get; set; }
        public ICollection<FoodPlan>? FoodPlans { get; set; }
    }
}
