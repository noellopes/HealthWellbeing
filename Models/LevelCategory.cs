using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class LevelCategory
    {
        // Primary Key
        [Key]
        public int LevelCategoryId { get; set; }

        // Category Name (e.g., "Beginner", "Intermediate")
        [Required(ErrorMessage = "Category Name cannot be empty.")]
        [Display(Name = "Category Name")]
        public string Name { get; set; }

        // Navigation Property: One Category has many Levels
        public ICollection<Level>? Levels { get; set; }
    }
}