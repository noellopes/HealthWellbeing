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
        public int DailyCalories { get; set; }
        public int DailyProtein { get; set; }
 
        public int DailyFat { get; set; }

      
        public int DailyHydrates { get; set; }

       
        public int DailyVitamins { get; set; }

    }
}
