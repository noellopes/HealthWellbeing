using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class FoodNutritionalComponent
    {
        [Key]
        public int FoodNutritionalComponentId { get; set; }

        [Required(ErrorMessage = "Component is required.")]
        public int NutritionalComponentId { get; set; }

        [Required(ErrorMessage = "Food is required.")]
        public int FoodId { get; set; }

        [Range(0, 100000, ErrorMessage = "Value must be a positive number.")]
        public double Value { get; set; }

        public NutritionalComponent? NutritionalComponent { get; set; }
        public Food? Food { get; set; }
    }
}
