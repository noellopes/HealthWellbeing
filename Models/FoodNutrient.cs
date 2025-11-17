using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class FoodNutrient
    {
        [Key]
        public int FoodNutrientId { get; set; }

        [Required]
        public int FoodId { get; set; }
        public Food? Food { get; set; }

        [Required]
        public int NutrientComponentId { get; set; }
        public NutrientComponent? NutrientComponent { get; set; }

        [Range(0, 999999.999)]
        [Column(TypeName = "decimal(9,3)")]
        [Display(Name = "Value")]
        public decimal Value { get; set; }

        [StringLength(15)]
        [Display(Name = "Unit (e.g. g, mg, kcal)")]
        public string? Unit { get; set; }

        [StringLength(25)]
        [Display(Name = "Basis (e.g. per100g, perServing)")]
        public string? Basis { get; set; }
    }
}
