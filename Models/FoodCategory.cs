using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class FoodCategory
    {
        [Key]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(80)]
        public string Category { get; set; } = string.Empty;

        public string? Description { get; set; }

        public ICollection<Food>? Foods { get; set; }
    }
}
