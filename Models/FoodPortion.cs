using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class FoodPortion : IValidatableObject
    {
        [Key]
        public int FoodPortionId { get; set; }

        [Required(ErrorMessage = "The food name is required.")]
        [StringLength(60, MinimumLength = 2, ErrorMessage = "Food name must be between 2 and 60 characters.")]
        [Display(Name = "Food name")]
        public string FoodName { get; set; } = string.Empty;

        [Required(ErrorMessage = "The amount is required.")]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "Amount must be between 1 and 30 characters.")]
        [Display(Name = "Amount (e.g., \"1 slice (30 g)\", \"200 ml\")")]
        public string Amount { get; set; } = string.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            if (FoodName.Trim().Length < 2)
                yield return new ValidationResult("Food name is too short.", new[] { nameof(FoodName) });

            if (Amount.Trim().Length < 1)
                yield return new ValidationResult("Amount cannot be blank.", new[] { nameof(Amount) });
        }
    }
}
