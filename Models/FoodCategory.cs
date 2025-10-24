using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class FoodCategory
    {
        [Key]
        public int FoodCategoryId { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Category name")]
        public string Name { get; set; } = default!;

        [StringLength(250)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        public ICollection<Food>? Foods { get; set; }
    }
}
