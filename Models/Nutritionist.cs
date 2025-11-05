using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace HealthWellbeing.Models
{
    public class Nutritionist
    {
        [Key]
        public int NutritionistId { get; set; }

        [Required, StringLength(120)]
        public string Name { get; set; } = string.Empty;

        public ICollection<FoodPlan>? FoodPlan { get; set; }
    }
}
