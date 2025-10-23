using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class FoodCategory
    {
        [Key]
        public int FoodCategoryId { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Category")]
        public string Name { get; set; } = default!;

        public ICollection<Food>? Foods { get; set; }
    }
}
