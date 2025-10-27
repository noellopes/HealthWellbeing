using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class UserFoodRegistration
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "The name must be at most 100 characters long")]
        public string Name { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "User ID must be greater than 0")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Meal date and time is required")]
        [Display(Name = "Meal Date and Time")]
        [DataType(DataType.DateTime)]
        public DateTime MealDateTime { get; set; }

        [Required(ErrorMessage = "Meal type is required")]
        [StringLength(20, ErrorMessage = "Meal type must be at most 20 characters long")]
        public string MealType { get; set; }

        [Required(ErrorMessage = "Food name is required")]
        [StringLength(100, ErrorMessage = "Food name must be at most 100 characters long")]
        public string FoodName { get; set; }

        [Range(0.1, 1000, ErrorMessage = "Quantity must be greater than 0")]
        public double Quantity { get; set; }

        [StringLength(250, ErrorMessage = "Notes must be at most 250 characters long")]
        public string? Notes { get; set; }
    }
}
