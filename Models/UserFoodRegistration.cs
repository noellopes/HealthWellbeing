using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class UserFoodRegistration
    {
        [Key]
        public int UserFoodRegistrationId { get; set; }

        [Required]
        [Display(Name = "Client")]
        public string ClientId { get; set; } = string.Empty;
        public Client? Client { get; set; }

        [Required]
        [Display(Name = "Food")]
        public int FoodId { get; set; }
        public Food? Food { get; set; }

        [Required]
        [Display(Name = "Portion")]
        public int FoodPortionId { get; set; }
        public FoodPortion? FoodPortion { get; set; }

        [Range(0.1, 99.9)]
        [Display(Name = "Portions Count")]
        public decimal PortionsCount { get; set; }

        [Required, StringLength(30)]
        [Display(Name = "Meal Type")]
        public string MealType { get; set; } = string.Empty;  // Breakfast, Lunch, etc.

        [Display(Name = "Date & Time of Meal")]
        public DateTime MealDateTime { get; set; } = DateTime.Now;

        [Display(Name = "Estimated Energy (kcal)")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal? EstimatedEnergyKcal { get; set; }

        [StringLength(200)]
        public string? Notes { get; set; }
    }
}
