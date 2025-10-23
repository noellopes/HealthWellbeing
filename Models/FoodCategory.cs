using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class FoodCategory
    {
        [Key]
        public int FoodCategoryId { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Category")]
        public string Name { get; set; } = default!;

        [StringLength(255)]
        public string Description { get; set; }

        // --- Hierarquia categoria pai/filho ---
        public int? ParentCategoryId { get; set; }

        [ForeignKey(nameof(ParentCategoryId))]
        public FoodCategory? ParentCategory { get; set; }

        // Filhos (subcategorias)
        [InverseProperty(nameof(ParentCategory))]
        public ICollection<FoodCategory> SubCategories { get; set; } = new List<FoodCategory>(); 

        public ICollection<Food>? Foods { get; set; } = new List<Food>();
    }
}
