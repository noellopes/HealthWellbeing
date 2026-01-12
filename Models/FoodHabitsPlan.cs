using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class FoodHabitsPlan
    {
        [Key]
        public int FoodHabitsPlanId { get; set; }

        [Required(ErrorMessage = "Client is required.")]
        public int ClientId { get; set; }

        [Required(ErrorMessage = "Goal is required.")]
        public int GoalId { get; set; }

        [Required(ErrorMessage = "Starting date is required.")]
        [DataType(DataType.Date)]
        public DateTime StartingDate { get; set; }

        [Required(ErrorMessage = "Ending date is required.")]
        [DataType(DataType.Date)]
        public DateTime EndingDate { get; set; }

        public Client? Client { get; set; }
        public Goal? Goal { get; set; }

        public ICollection<NutritionistClientPlan>? NutritionistClientPlans { get; set; }
        public ICollection<PlanFood>? PlanFoods { get; set; }

        [NotMapped]
        public string StatusText
        {
            get
            {
                var today = DateTime.Today.Date;
                if (today < StartingDate.Date) return "Pending";
                if (today > EndingDate.Date) return "Finished";
                return "Completed";
            }
        }
    }
}
