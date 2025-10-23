using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class FoodComponent
    {
        public int FoodComponentId { get; set; } = default!;

        [Required(ErrorMessage = "The name of the Food is mandatory!")]
        [StringLength(20)]
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "The Food has to have a mandatory description!")]
        [StringLength(100)]
        public string Description { get; set; } = default!;
    }
}
