using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class NutrientComponent
    {
        public int NutrientComponentId { get; set; }

        [Required, StringLength(60)]
        public string Name { get; set; } = string.Empty;

        [StringLength(10)]
        public string? DefaultUnit { get; set; } = "g";

        [StringLength(24)]
        public string? Code { get; set; }

        public bool IsMacro { get; set; }                 

        [StringLength(200)]
        public string? Description { get; set; }
        public ICollection<FoodNutrient>? FoodNutrient { get; set; }

    }
}
