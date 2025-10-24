using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class FoodPortion
    {
        public int UnitId { get; set; }

        [Required, StringLength(60)]
        public string FoodName { get; set; } = string.Empty;

        [Required, StringLength(30)]
        public string Amount { get; set; } = string.Empty;
    }
}
