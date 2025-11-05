using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Nutritionist
    {
        [Key]
        public int NutritionistId { get; set; }

        [Required, StringLength(80)]
        public string Name { get; set; } = string.Empty;

        
        [EmailAddress, StringLength(120)]
        public string? Email { get; set; }

        [Phone, StringLength(30)]
        public string? Phone { get; set; }
    }
}
