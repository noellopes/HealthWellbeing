using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class FoodCategory
    {
        public int FoodCategoryId { get; set; }

<<<<<<< Updated upstream
        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Category name must be between 2 and 100 characters.")]
        [Display(Name = "Category name")]
        public string Name { get; set; } = string.Empty!;

        [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters.")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        // --- Hierarquia categoria pai/filho ---
        [Display(Name = "Parent category")]
        public int? ParentCategoryId { get; set; }

        [ForeignKey(nameof(ParentCategoryId))]
        public FoodCategory? ParentCategory { get; set; }

        // Filhos (subcategorias)
        [InverseProperty(nameof(ParentCategory))]
        public ICollection<FoodCategory> SubCategory { get; set; } = new List<FoodCategory>();

        public ICollection<Food>? Foods { get; set; } = new List<Food>();

        // ===== Model-level validation =====
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // 1) Name cannot be blank/whitespace
            if (string.IsNullOrWhiteSpace(Name))
            {
                yield return new ValidationResult(
                    "Category name cannot be empty or whitespace.",
                    new[] { nameof(Name) });
            }

            // 2) Normalize spaces to avoid “ABC ” != “ABC”
            var trimmedName = Name?.Trim();
            if (!string.Equals(Name, trimmedName, StringComparison.Ordinal))
            {
                yield return new ValidationResult(
                    "Remove leading/trailing spaces from the category name.",
                    new[] { nameof(Name) });
            }

            // 3) Self-parent is not allowed
            if (ParentCategoryId.HasValue && ParentCategoryId.Value == FoodCategoryId)
            {
                yield return new ValidationResult(
                    "A category cannot be its own parent.",
                    new[] { nameof(ParentCategoryId) });
            }

            // 4) Optional: Description must differ from Name (basic sanity)
            if (!string.IsNullOrWhiteSpace(Description) &&
                trimmedName != null &&
                string.Equals(Description.Trim(), trimmedName, StringComparison.OrdinalIgnoreCase))
            {
                yield return new ValidationResult(
                    "Description should add information beyond the category name.",
                    new[] { nameof(Description) });
            }
        }
=======
        [Required, StringLength(60)]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Description { get; set; }

        public int? ParentCategoryId { get; set; }
        public FoodCategory? ParentCategory { get; set; }

        public ICollection<FoodCategory> SubCategory { get; set; } = new List<FoodCategory>();

        public ICollection<Food>? Food { get; set; }
>>>>>>> Stashed changes
    }
}
