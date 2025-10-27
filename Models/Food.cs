using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace HealthWellbeing.Models
{
    public class Food : IValidatableObject
    {
        [Key]
        public int FoodId { get; set; }

        [Required(ErrorMessage = "The food name is required.")]
        [StringLength(120, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 120 characters.")]
        [Display(Name = "Food name")]
        // Remote uniqueness: Name per Category
        [Remote(action: "CheckNameUnique", controller: "Food",
            AdditionalFields = nameof(FoodId) + "," + nameof(FoodCategoryId),
            ErrorMessage = "A food with this name already exists in the selected category.")]
        public string Name { get; set; } = default!;

        [StringLength(500, ErrorMessage = "Description must have at most 500 characters.")]
        public string? Description { get; set; }

        // Nutritional info per 100g
        [Range(0, 1200, ErrorMessage = "Kcal must be between 0 and 1200 per 100g.")]
        [Display(Name = "Kcal / 100g")]
        public decimal KcalPer100g { get; set; }

        [Range(0, 100, ErrorMessage = "Protein must be between 0 and 100 g per 100g.")]
        [Display(Name = "Protein (g) / 100g")]
        public decimal ProteinPer100g { get; set; }

        [Range(0, 100, ErrorMessage = "Carbs must be between 0 and 100 g per 100g.")]
        [Display(Name = "Carbs (g) / 100g")]
        public decimal CarbsPer100g { get; set; }

        [Range(0, 100, ErrorMessage = "Fat must be between 0 and 100 g per 100g.")]
        [Display(Name = "Fat (g) / 100g")]
        public decimal FatPer100g { get; set; }

        // FK to Category (optional)
        [Display(Name = "Category")]
        public int? FoodCategoryId { get; set; }
        public FoodCategory? Category { get; set; }

        // Domain rules
        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            // Proteins + Carbs + Fat cannot exceed 100g per 100g of food
            var totalMacros = ProteinPer100g + CarbsPer100g + FatPer100g;
            if (totalMacros > 100)
            {
                yield return new ValidationResult(
                    "Protein + Carbs + Fat must not exceed 100 g per 100 g of food.",
                    new[] { nameof(ProteinPer100g), nameof(CarbsPer100g), nameof(FatPer100g) }
                );
            }

            
            // 1g protein ≈ 4 kcal, 1g carbs ≈ 4 kcal, 1g fat ≈ 9 kcal
            if (KcalPer100g < 0 || KcalPer100g > 1200)
            {
                yield return new ValidationResult("Kcal must be realistic (0–1200).", new[] { nameof(KcalPer100g) });
            }
        }
    }
}
