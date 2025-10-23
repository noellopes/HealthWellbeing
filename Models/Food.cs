using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class Food
    {
        [Key]
        public int FoodId { get; set; }

        [Required, StringLength(120)]
        [Display(Name = "Food name")]
        public string Name { get; set; } = default!;

        [StringLength(500)]
        public string? Description { get; set; }

        // Nutritional info per 100g
        [Range(0, 1200)]
        [Display(Name = "Kcal / 100g")]
        public decimal KcalPer100g { get; set; }

        [Range(0, 100)]
        [Display(Name = "Protein (g) / 100g")]
        public decimal ProteinPer100g { get; set; }

        [Range(0, 100)]
        [Display(Name = "Carbs (g) / 100g")]
        public decimal CarbsPer100g { get; set; }

        [Range(0, 100)]
        [Display(Name = "Fat (g) / 100g")]
        public decimal FatPer100g { get; set; }

        // FK to Category (optional)
        [Display(Name = "Category")]
        public int? FoodCategoryId { get; set; }

        public FoodCategory? Category { get; set; }
    }
}
