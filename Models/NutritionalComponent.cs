using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class NutritionalComponent
    {
        [Key]
        public int NutritionalComponentId { get; set; }

        [Required(ErrorMessage = "Component name is required.")]
        [StringLength(80)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Unit is required (e.g., g, mg, kcal).")]
        [StringLength(20)]
        public string Unit { get; set; } = string.Empty;

        [Required(ErrorMessage = "Basis is required (e.g., per 100 g).")]
        [StringLength(50)]
        public string Basis { get; set; } = string.Empty;

        public ICollection<FoodNutritionalComponent>? FoodNutritionalComponents { get; set; }
    }
}
