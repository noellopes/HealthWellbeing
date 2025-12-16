using HealthWellbeing.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class FoodIntake
    {
        [Key]
        public int FoodIntakeId { get; set; }

        public int PlanId { get; set; }
        public int FoodId { get; set; }
        public int PortionId { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public int PortionsPlanned { get; set; }
        public int PortionsEaten { get; set; }

        [NotMapped]
        public bool Eaten => PortionsEaten >= PortionsPlanned;

        public DateTime ScheduledTime { get; set; }

        public Food? Food { get; set; }
        public Plan? Plan { get; set; }
        public Portion? Portion { get; set; }
    }

}

