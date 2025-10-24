using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace HealthWellbeing.Models
{
    public class Objective : IValidatableObject
    {
        [Key]
        public int ObjectiveId { get; set; }

        [Required(ErrorMessage = "The objective name is required.")]
        [StringLength(120, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 120 characters.")]
        [RegularExpression(@"^[A-Za-z0-9\s\-\’,']+$", ErrorMessage = "Name can only contain letters, numbers, spaces and - ' , characters.")]
        [Display(Name = "Objective name")]
        // Remote check for uniqueness within the selected Category
        [Remote(action: "CheckNameUnique", controller: "Objective", AdditionalFields = nameof(ObjectiveId) + "," + nameof(Category),
            ErrorMessage = "An objective with this name already exists in the selected category.")]
        public string Name { get; set; } = default!;

        // Allowed values validated in IValidatableObject below
        [Required(ErrorMessage = "The category is required.")]
        [StringLength(50, ErrorMessage = "Category must have at most 50 characters.")]
        [Display(Name = "Category")]
        public string Category { get; set; } = default!;

        [StringLength(500, ErrorMessage = "Details must have at most 500 characters.")]
        [Display(Name = "Details")]
        public string? Details { get; set; }

        // Domain validation beyond attributes
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var allowed = new HashSet<string>
            {
                "Lose weight","Gain weight","Build muscle","Lose fat","Maintain weight","Improve endurance"
            };

            if (!allowed.Contains(Category))
            {
                yield return new ValidationResult(
                    $"Category must be one of: {string.Join(", ", allowed)}.",
                    new[] { nameof(Category) });
            }

            // Optional: simple guidance rule
            if (!string.IsNullOrWhiteSpace(Details) && Details.Trim().Length < 5)
            {
                yield return new ValidationResult(
                    "If provided, details should be descriptive (at least 5 characters).",
                    new[] { nameof(Details) });
            }
        }
    }
}
