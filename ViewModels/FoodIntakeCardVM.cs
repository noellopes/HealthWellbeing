using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthWellbeing.ViewModels
{
    public class FoodIntakeCardVM
    {
        public int FoodId { get; set; }

        // Id da linha na tabela FoodIntake
        public int? FoodIntakeId { get; set; }

        public string FoodName { get; set; } = "";
        public string? PortionName { get; set; }
        public int PortionsPlanned { get; set; }
        public int PortionsEaten { get; set; }
        public bool IsConsumed { get; set; }
        public string Note { get; set; } = "";

        // Bootstrap icon
        public string? IconClass { get; set; }
    }
}
