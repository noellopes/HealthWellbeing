using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class Portion
    {
        [Key]
        public int PortionId { get; set; }
        public string PortionName { get; set; } = string.Empty;
        public ICollection<FoodPlan>? FoodPlans { get; set; }
    }
}
