using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Goal
    {
        [Key]
        public int GoalId { get; set; }

        [Required(ErrorMessage = "Client is required.")]
        public int ClientId { get; set; }

        public Client? Client { get; set; }

        [Required(ErrorMessage = "Goal name is required.")]
        [StringLength(200)]
        public string GoalName { get; set; } = string.Empty;


        [Range(0, 20000, ErrorMessage = "Daily calories must be a positive value.")]
        public int DailyCalories { get; set; }

        [Range(0, 1000, ErrorMessage = "Daily protein must be a positive value.")]
        public int DailyProtein { get; set; }

        [Range(0, 1000, ErrorMessage = "Daily fat must be a positive value.")]
        public int DailyFat { get; set; }

        [Range(0, 1000, ErrorMessage = "Daily hydrates must be a positive value.")]
        public int DailyHydrates { get; set; }

        [Range(0, 1000, ErrorMessage = "Daily vitamins must be a positive value.")]
        public int DailyVitamins { get; set; }

    }
}
