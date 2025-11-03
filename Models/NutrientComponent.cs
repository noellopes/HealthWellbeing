using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class NutrientComponent
    {
        [Key]
        public int NutrientComponentId { get; set; }

        [Required, StringLength(120)]
        public string Name { get; set; } = string.Empty;

        [StringLength(300)]
        public string? Description { get; set; }

        public ICollection<FoodNutrient>? FoodNutrients { get; set; }
    }
}
