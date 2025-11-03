using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class FoodCategory
    {
        [Key]
        public int FoodCategoryId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(300)]
        public string? Description { get; set; }

        // Hierarquia (self-reference)
        public int? ParentCategoryId { get; set; }
        public FoodCategory? ParentCategory { get; set; }
        public ICollection<FoodCategory>? Subcategories { get; set; }

        // Relação com alimentos
        public ICollection<Food>? Foods { get; set; }
    }
}
