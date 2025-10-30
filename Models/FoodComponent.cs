using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class FoodComponent
    {
        [Key]
        public int FoodComponentId { get; set; }

        [Required(ErrorMessage = "The name of the food is mandatory!")]
        [StringLength(50)]
        public string Name { get; set; } = default!;

        [StringLength(200)]
        public string Description { get; set; } = default!;
    }
}
