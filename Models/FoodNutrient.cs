using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class FoodNutrient
    {
        [Key]
        public int FoodNutrientId { get; set; }

        [Required]
        public int NutrientComponentId { get; set; }
        public NutrientComponent? NutrientComponent { get; set; }

        [Required]
        public int FoodId { get; set; }
        public Food? Food { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Value { get; set; }
    }
}
