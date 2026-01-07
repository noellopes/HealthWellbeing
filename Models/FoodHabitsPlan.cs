using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class FoodHabitsPlan
    {
        [Key]
        public int FoodHabitsPlanId { get; set; }
        public int ClientId { get; set; }

		[Required(ErrorMessage = "The plan name is required")]
		[StringLength(100, ErrorMessage = "The plan name cannot exceed 100 characters")]
		public string Name { get; set; } = default!;

		public string? Description { get; set; }

		[Required(ErrorMessage = "The plan price is required")]
		[Range(1.00, 999.99, ErrorMessage = "The price must be between 1.00 and 999.99")]
		public decimal Price { get; set; }

		[Required(ErrorMessage = "The training duration is required")]
		[Range(1, 365, ErrorMessage = "The duration must be between 1 and 365 days")]
		public int DurationDays { get; set; }
	}
}
