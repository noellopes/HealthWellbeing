using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class CategoryFood
    {
        public int CategoryID { get; set; }

        [Required, StringLength(80)]
        public string Name { get; set; } = default!;

        [StringLength(255)]
        public string Description { get; set; }

        public Alimento FoodID { get; set; } = null;

    }
}
