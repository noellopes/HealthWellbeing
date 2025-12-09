using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Nutritionist
    {
        [Key]
        public int NutritionistId { get; set; }

        [Required(ErrorMessage = "Nutritionist name is required.")]
        [StringLength(80)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gender is required.")]
        [StringLength(20)]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(120)]
        public string Email { get; set; } = string.Empty;

        public ICollection<NutritionistClientPlan>? NutritionistClientPlans { get; set; }
    }
}
