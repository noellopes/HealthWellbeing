using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class FoodIntake
    {
        [Key]
        public int FoodIntakeId { get; set; }
        public int PlanId { get; set; }
        public int FoodId { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public bool Eaten { get; set; }
        public ICollection<Food>? Foods { get; set; }
        public ICollection<Plan>? Plans { get; set; }

    }
}
