using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class FoodCategory
    {
        [Key]
        public int FoodCategoryId { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(60, ErrorMessage = "Name must have at most 60 characters.")]
        [Display(Name = "Category Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Parent Category")]
        public int? ParentCategoryId { get; set; }

        [ForeignKey(nameof(ParentCategoryId))]
        public FoodCategory? ParentCategory { get; set; }

        [Display(Name = "Subcategories")]
        public ICollection<FoodCategory> SubCategory { get; set; } = new List<FoodCategory>();

        public ICollection<Food>? Foods { get; set; }
    }
}
