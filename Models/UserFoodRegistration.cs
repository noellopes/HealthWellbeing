using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class UserFoodRegistration
    {
        [Key]
        public int UserFoodRegId { get; set; } = default;

        [Required(ErrorMessage = "Client is required")]
        [StringLength(450)]
        [Column(TypeName = "nvarchar(450)")]
        public string ClientId { get; set; } = string.Empty;

        // Propriedade de navegação
        public Client? Client { get; set; }

        [Required(ErrorMessage = "Meal date and time is required")]
        [Display(Name = "Meal Date and Time")]
        [DataType(DataType.DateTime)]
        public DateTime MealDateTime { get; set; }

        [Required(ErrorMessage = "Meal type is required")]
        [StringLength(20, ErrorMessage = "Meal type must be at most 20 characters long")]
        public string MealType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Food name is required")]
        [StringLength(100, ErrorMessage = "Food name must be at most 100 characters long")]
        public string FoodName { get; set; } = string.Empty;

        [Range(0.1, 1000, ErrorMessage = "Quantity must be greater than 0")]
        public double Quantity { get; set; }

        
        public string? Notes { get; set; }
    }
}
