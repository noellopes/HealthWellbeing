using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class FoodCategory
    {
        public int FoodCategoryId { get; set; }

        [Required, StringLength(60)]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Description { get; set; }

        public int? ParentCategoryId { get; set; }
        public FoodCategory? ParentCategory { get; set; }

        public ICollection<FoodCategory> SubCategory { get; set; } = new List<FoodCategory>();

        public ICollection<Food>? Food { get; set; }
    }
}
