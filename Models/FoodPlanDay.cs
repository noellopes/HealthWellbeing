using System;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class FoodPlanDay
    {
        [Key]
        public int FoodPlanDayId { get; set; }

        [Required]
        public int PlanId { get; set; }

        [Required]
        public int FoodId { get; set; }

        [Required]
        public int PortionId { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Range(1, 50)]
        public int PortionsPlanned { get; set; } = 1;

        public DateTime? ScheduledTime { get; set; }
        public string? MealType { get; set; }

        public FoodHabitsPlan? Plan { get; set; }
        public Food? Food { get; set; }
        public Portion? Portion { get; set; }
    }
}
