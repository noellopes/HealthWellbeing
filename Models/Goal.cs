using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace HealthWellbeing.Models
{
    public class Goal
    {
        [Key]
        public int GoalId { get; set; }

        [Required]
        public int PatientId { get; set; }  // FK to UtenteSaude

        [Required, StringLength(60)]
        public string GoalType { get; set; } = string.Empty;

        public int? DailyCalories { get; set; }
        public int? DailyProtein { get; set; }
        public int? DailyFats { get; set; }
        public int? DailyCarbs { get; set; }
        public int? DailyVitamins { get; set; }
        public int? DailyMinerals { get; set; }
        public int? DailyFibers { get; set; }

        public ICollection<Plan>? Plans { get; set; }
    }
}
